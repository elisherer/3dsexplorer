using System.IO;

namespace _3DSExplorer
{
    public enum ModuleType
    {
        Unknown = -1,
        Banner = 0,
        CIA,
        CGFX,
        CWAV,
        Rom,
        SRAM_Decrypted,
        SRAM,
        TMD //Contains certificates & ticket so they can't be recognized
    }

    public static class ModuleHelper
    {

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
                    case ModuleType.Rom:
                    return new RomContext();
                    case ModuleType.SRAM_Decrypted:
                    case ModuleType.SRAM:
                    return new SRAMContext();
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
                case ".3ds":
                    type = ModuleType.Rom;
                    break;
                case ".bin":
                case ".sav":
                    type = ModuleType.SRAM_Decrypted;
                    break;
                case ".tmd":
                    type = ModuleType.TMD;
                    break;
                case ".cia":
                    type = ModuleType.CIA;
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
                    else if (magic[0] == 'C' && magic[1] == 'B' && magic[2] == 'M' && magic[3] == 'D')
                        type = ModuleType.Banner;
                    else if (magic[0] == 'C' && magic[1] == 'G' && magic[2] == 'F' && magic[3] == 'X')
                        type = ModuleType.CGFX;
                    else if (magic[0] == 'C' && magic[1] == 'W' && magic[2] == 'A' && magic[3] == 'V')
                        type = ModuleType.CWAV;
                    else if (fs.Length >= 0x104) // > 256+4
                    {
                        //CCI CHECK
                        fs.Seek(0x100, SeekOrigin.Current);
                        fs.Read(magic, 0, 4);
                        if (magic[0] == 'N' && magic[1] == 'C' && magic[2] == 'S' && magic[3] == 'D')
                            type = ModuleType.Rom;
                        else if (fs.Length >= 0x10000) // > 64kb
                        {
                            //SAVE Check
                            fs.Seek(0, SeekOrigin.Begin);
                            var crcCheck = new byte[8 + 10 * (fs.Length / 0x1000 - 1)];
                            fs.Read(crcCheck, 0, crcCheck.Length);
                            fs.Read(magic, 0, 2);
                            var calcCheck = CRC16.GetCRC(crcCheck);
                            if (magic[0] == calcCheck[0] && magic[1] == calcCheck[1]) //crc is ok then save
                                type = ModuleType.SRAM_Decrypted; //SAVE
                        }
                    }
                    break;
            }
            if (type == ModuleType.SRAM_Decrypted)
            {
                //check if encrypted
                fs.Seek(0x1000, SeekOrigin.Begin); //Start of information
                while ((fs.Length - fs.Position > 0x200) & !SRAMContext.IsSaveMagic(magic))
                {
                    fs.Read(magic, 0, 4);
                    fs.Seek(0x200 - 4, SeekOrigin.Current);
                }
                if (fs.Length - fs.Position <= 0x200)
                    type = ModuleType.SRAM;

            }
            return type;
        }
    }
}
