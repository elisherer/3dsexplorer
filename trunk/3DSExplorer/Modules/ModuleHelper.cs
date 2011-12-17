using System.IO;

namespace _3DSExplorer.Modules
{
    public enum ModuleType
    {
        Unknown = -1,
        Banner = 0,
        CIA,
        CGFX,
        CWAV,
        ICN,
        MPO,
        CCI,
        CXI,
        SaveFlash_Decrypted,
        SaveFlash,
        TMD //Contains certificates & ticket so they can't be recognized
    }

    public static class ModuleHelper
    {
        //TODO: get this from the modules
        public const string OpenString = 
            @"All Supported|" +
                "*.3ds;*.cci;*.bin;*.sav;*.tmd;*.cia;*.mpo;*.bnr;*.bcwav;*.cwav;*.cgfx;*.icn|" +
            "CTR Cartridge Images (*.cci/3ds/csu)|*.3ds;*.cci;*.csu|"+
            "CTR Executable (*.cxi)|*.cxi|" +
            "CTR Importable Archives (*.cia)|*.cia|"+
            "CTR Icons (*.icn)|*.icn|" +
            "CTR Banners (*.bnr)|*.bnr|"+
            "CTR Waves (*.b/cwav)|*.bcwav;*.cwav|"+
            "CTR Graphics (*.cgfx)|*.cgfx|"+
            "Save Flash Files (*.bin,*.sav)|*.bin;*.sav|" +
            "Title Metadata (*.tmd)|*.tmd|" +
            "MPO (3D Images) Files (*.mpo)|*.mpo|" +
            "All Files|*.*";

        public static IContext CreateByType(ModuleType type)
        {
            switch (type)
            {
                case ModuleType.Banner:
                    return new BannerContext();
                case ModuleType.CGFX:
                    return new CGFXContext();
                case ModuleType.CIA:
                    return new CIAContext();
                case ModuleType.CWAV:
                    return new CWAVContext();
                case ModuleType.ICN:
                    return new ICNContext();
                case ModuleType.MPO:
                    return new MPOContext();
                case ModuleType.CCI:
                    return new CCIContext();
                case ModuleType.CXI:
                    return new CXIContext();
                case ModuleType.SaveFlash_Decrypted:
                case ModuleType.SaveFlash:
                    return new SaveFlashContext();
                case ModuleType.TMD:
                    return new TMDContext();
            }
            return null;
        }

        public static ModuleType GetModuleType(string filePath, FileStream fs)
        {
            var type = ModuleType.Unknown;
            var magic = new byte[4];
            var extension = Path.GetExtension(filePath);
            if (extension != null)
                extension = extension.ToLower();

            switch (extension)
            {
                case ".cci":
                case ".csu":
                case ".3ds":
                    type = ModuleType.CCI;
                    break;
                case ".cxi":
                    type = ModuleType.CXI;
                    break;
                case ".bin":
                case ".sav":
                    type = ModuleType.SaveFlash_Decrypted;
                    break;
                case ".tmd":
                    type = ModuleType.TMD;
                    break;
                case ".cia":
                    type = ModuleType.CIA;
                    break;
                case ".icn":
                    type = ModuleType.ICN;
                    break;
                case ".mpo":
                    type = ModuleType.MPO;
                    break;
                case ".bnr":
                    type = ModuleType.Banner;
                    break;
                case ".cgfx":
                    type = ModuleType.CGFX;
                    break;
                case ".cwav":
                case ".bcwav":
                    type = ModuleType.CWAV;
                    break;
                default:
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Read(magic, 0, 4);
                    if (magic[0] < 5 && magic[1] == 0 && magic[2] == 1 && magic[3] == 0)
                        type = ModuleType.TMD;
                    else if (magic[0] == 0x20 && magic[1] == 0x20 && magic[2] == 0 && magic[3] == 0)
                        type = ModuleType.CIA;
                    else if (magic[0] == 0xFF && magic[1] == 0xD8 && magic[2] == 0xFF && magic[3] == 0xE1)
                        type = ModuleType.MPO;
                    else if (magic[0] == 'C' && magic[1] == 'B' && magic[2] == 'M' && magic[3] == 'D')
                        type = ModuleType.Banner;
                    else if (magic[0] == 'C' && magic[1] == 'G' && magic[2] == 'F' && magic[3] == 'X')
                        type = ModuleType.CGFX;
                    else if (magic[0] == 'C' && magic[1] == 'W' && magic[2] == 'A' && magic[3] == 'V')
                        type = ModuleType.CWAV;
                    else if (magic[0] == 'S' && magic[1] == 'M' && magic[2] == 'D' && magic[3] == 'H')
                        type = ModuleType.ICN;
                    else if (magic[0] == 'N' && magic[1] == 'C' && magic[2] == 'C' && magic[3] == 'H')
                        type = ModuleType.CXI;
                    else if (fs.Length >= 0x104) // > 256+4
                    {
                        //CCI CHECK
                        fs.Seek(0x100, SeekOrigin.Current);
                        fs.Read(magic, 0, 4);
                        if (magic[0] == 'N' && magic[1] == 'C' && magic[2] == 'S' && magic[3] == 'D')
                            type = ModuleType.CCI;
                        else if (fs.Length >= 0x10000) // > 64kb
                        {
                            //SAVE Check
                            fs.Seek(0, SeekOrigin.Begin);
                            var crcCheck = new byte[8 + 10 * (fs.Length / 0x1000 - 1)];
                            fs.Read(crcCheck, 0, crcCheck.Length);
                            fs.Read(magic, 0, 2);
                            var calcCheck = CRC16.GetCRC(crcCheck);
                            if (magic[0] == calcCheck[0] && magic[1] == calcCheck[1]) //crc is ok then save
                                type = ModuleType.SaveFlash_Decrypted; //SAVE
                        }
                    }
                    break;
            }
            if (type == ModuleType.SaveFlash_Decrypted)
            {
                if (SaveFlashContext.IsEncrypted(fs))
                    type = ModuleType.SaveFlash;
            }
            return type;
        }
    }
}
