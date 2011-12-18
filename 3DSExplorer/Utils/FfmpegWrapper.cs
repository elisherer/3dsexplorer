using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace _3DSExplorer
{
    public class FfmpegWrapper
    {
        private readonly string _ffpmegPath;

        public delegate void ProgressChanged(int value, int max);
        public delegate void ProcessFinished(string errorString);

        private readonly ProgressChanged _progressChanged;
        private readonly ProcessFinished _processFinished;

        private string _errorMessage;
        private int _timeLimit;

        public FfmpegWrapper(string ffpmegPath, ProgressChanged progressChanged, ProcessFinished processFinished)
        {
            _ffpmegPath = ffpmegPath;
            _progressChanged = progressChanged;
            _processFinished = processFinished;
        }

        #region Private methods

        private static int ConvertTimeToInt(string timeString)
        {
            return (int)TimeSpan.Parse(timeString).TotalSeconds;
        }

        #region Background Worker methods

        private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _processFinished(_errorMessage);
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            //Start a process for part number 1
            var ts = TimeSpan.FromSeconds(_timeLimit);
            var limited = _timeLimit > 0;
            var args = (limited ? "-t " + ts : string.Empty) + string.Join(" ", (string[])e.Argument);

            var process = new Process { StartInfo = { FileName = _ffpmegPath , Arguments = args, CreateNoWindow = true, RedirectStandardError = true, UseShellExecute = false} };
            if (!process.Start())
            {
                _errorMessage = "Error starting ffmpeg.exe";
                return;
            }
            var reader = process.StandardError;
            var needMoreParts = false;

            //Start reading the output
            var started = false;
            var accepted = false;
            var duration = 0;
            do
            {
                var line = reader.ReadLine();
                if (line == null)
                    break;
                _errorMessage = line;

                if (!accepted && line.StartsWith("Input #"))
                    accepted = true;
                if (!accepted) continue;
                if (!started && line.StartsWith("frame"))
                    started = true;
                else
                {
                    //get duration
                    if (line.StartsWith("  Duration:"))
                    {
                        duration = ConvertTimeToInt(line.Substring(line.IndexOf('0'), 11));
                        if (limited && duration > _timeLimit)
                            needMoreParts = true;
                    }
                    continue;
                }

                if (!line.StartsWith("fr")) continue;
                var value = ConvertTimeToInt(line.Substring(line.IndexOf("time=") + 5, 11));
                ((BackgroundWorker)sender).ReportProgress(value, duration);

            } while (!reader.EndOfStream);
            if (started)
                _errorMessage = string.Empty;
            process.Close();


            if (!needMoreParts) return;
            //start creating the splits
            var durationTime = TimeSpan.FromSeconds(duration);
            var startTime = TimeSpan.FromSeconds(_timeLimit);
            //cut the filename and enter a new file name
            var filenameIndex = args.LastIndexOf(" \"") + 2;
            var filename = args.Substring(filenameIndex,args.Length-filenameIndex-1);
            args = args.Remove(filenameIndex);
            var filePrefix = Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename);
            var filePostfix = Path.GetExtension(filename);
            var counter = 2;
            while (startTime.CompareTo(durationTime) < 0)
            {
                var newArgs = string.Format("-ss {0} {1}\"{2}\"", ts, args, filePrefix + (counter++) + filePostfix);
                process = new Process
                              {
                                  StartInfo =
                                      {
                                          FileName = _ffpmegPath,
                                          Arguments = newArgs,
                                          CreateNoWindow = true,
                                          RedirectStandardError = true,
                                          UseShellExecute = false
                                      }
                              };
                if (!process.Start())
                {
                    _errorMessage = "Error starting ffmpeg.exe";
                    return;
                }
                reader = process.StandardError;
                //Start reading the output
                started = false;
                accepted = false;
                do
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        break;
                    _errorMessage = line;

                    if (!accepted && line.StartsWith("Input #"))
                        accepted = true;
                    if (!accepted) continue;
                    if (!started && line.StartsWith("frame"))
                        started = true;
                    else
                        continue; //BUG

                    if (!line.StartsWith("fr")) continue;
                    var value = ConvertTimeToInt(line.Substring(line.IndexOf("time=") + 5, 11));
                    ((BackgroundWorker) sender).ReportProgress(value, duration);

                } while (!reader.EndOfStream);
                if (started)
                    _errorMessage = string.Empty;
                process.Close();
                startTime = startTime.Add(ts);
            }
        }

        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _progressChanged(e.ProgressPercentage,(int)e.UserState);
        }

        #endregion

        #endregion

        public void Convert(int timeLimit, params string[] args)
        {
            _timeLimit = timeLimit;
            var backgroundWorker = new BackgroundWorker {WorkerReportsProgress = true};
            backgroundWorker.ProgressChanged += BackgroundWorkerProgressChanged;
            backgroundWorker.DoWork += BackgroundWorkerDoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
            backgroundWorker.RunWorkerAsync(args);
        }

    }
}
