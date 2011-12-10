using System;
using System.ComponentModel;
using System.Diagnostics;

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
            //Start a process
            var args = (string[])e.Argument;
            var process = new Process { StartInfo = { FileName = _ffpmegPath , Arguments = string.Join(" ", args) , CreateNoWindow = true, RedirectStandardError = true, UseShellExecute = false} };
            if (!process.Start())
            {
                _errorMessage = "Error starting ffmpeg.exe";
                return;
            }
            var reader = process.StandardError;
            
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
                        duration = ConvertTimeToInt(line.Substring(line.IndexOf('0'), 11));
                    continue;
                }

                if (!line.StartsWith("fr")) continue;
                var value = ConvertTimeToInt(line.Substring(line.IndexOf("time=") + 5, 11));
                ((BackgroundWorker)sender).ReportProgress(value, duration);

            } while (!reader.EndOfStream);
            if (started)
                _errorMessage = string.Empty;
            process.Close();
        }

        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _progressChanged(e.ProgressPercentage,(int)e.UserState);
        }

        #endregion

        #endregion

        public void Convert(params string[] args)
        {
            var backgroundWorker = new BackgroundWorker {WorkerReportsProgress = true};
            backgroundWorker.ProgressChanged += BackgroundWorkerProgressChanged;
            backgroundWorker.DoWork += BackgroundWorkerDoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
            backgroundWorker.RunWorkerAsync(args);
        }

    }
}
