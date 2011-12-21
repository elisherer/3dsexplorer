namespace _3DSExplorer
{
    public class TitleInfo
    {
        public string Type;
        public string Title;
        public string Region;
        public string Developer;
        public string ProductCode;

        public TitleInfo()
        {
            const string unknown = "<Unknown>";
            Type = unknown;
            Title = unknown;
            Region = unknown;
            Developer = unknown;
            ProductCode = unknown;
        }

        public override string ToString()
        {
            return Title;
        }

        public static TitleInfo Resolve(char[] productChars, char[] developerChars)
        {
            var info = new TitleInfo();
            //CTR-P-#$$%
            //0123456789
            info.ProductCode = new string(productChars);

            if (productChars != null)
            {
                var type = productChars[6];
                switch (type)
                {
                    case 'A':
                        info.Type = "Application";
                        break;
                    case 'C':
                        info.Type = "CTR";
                        break;
                    case 'H':
                        info.Type = "Dev";
                        break;
                }
                var regionCode = productChars[9];
                switch (regionCode)
                {
                    case 'E':
                        info.Region = "USA";
                        break;
                    case 'P':
                        info.Region = "EUR";
                        break;
                    case 'J':
                        info.Region = "JPN";
                        break;
                }
                var productCode = productChars[7].ToString() + productChars[8];
                if (type == 'A')
                {
                    switch (productCode)
                    {
                        case "66":
                            info.Title = "Samurai Warriors: Chronicles";
                            break;
                        case "AB":
                            info.Title = "Tales of the Abyss";
                            break;
                        case "AZ":
                            info.Title = "Cars 2";
                            break;
                        case "BB":
                            info.Title = "Puzzle Bobble Universe";
                            break;
                        case "BL":
                            info.Title = "Blazblue: Continuum Shift II";
                            break;
                        case "BM":
                            info.Title = "Resident Evil: The Mercenaries 3D";
                            break;
                        case "C3":
                            info.Title = "Ace Combat: Assault Horizon Legacy";
                            break;
                        case "CC":
                            info.Title = "Super Pokémon Rumble";
                            break;
                        case "CN":
                            info.Title = "Cartoon Network: Punch Time Explosion";
                            break;
                        case "CQ":
                            info.Title = "Cooking Mama 4: Kitchen Magic";
                            break;
                        case "CT":
                            info.Title = "Puppies World 3D";
                            break;
                        case "CV":
                            info.Title = "Cave Story 3D";
                            break;
                        case "DA":
                            info.Title = "Nintendogs + Cats: Golden Retriever & New Friends";
                            break;
                        case "DB":
                            info.Title = "Nintendogs + Cats: French Bulldog & New Friends";
                            break;
                        case "DC":
                            info.Title = "Nintendogs + Cats: Toy Poodle & New Friends";
                            break;
                        case "DD":
                            info.Title = "Dead or Alive: Dimensions";
                            break;
                        case "DE":
                            info.Title = "Sports Island 3D";
                            break;
                        case "DR":
                            info.Title = "Driver Renegate 3D";
                            break;
                        case "DT":
                            info.Title = "Dream Trigger 3D";
                            break;
                        case "EE":
                            info.Title = "Pro Evolution Soccer 2011 3D";
                            break;
                        case "F2":
                            info.Title = "FIFA 12";
                            break;
                        case "F4":
                            info.Title = "F1 2011";
                            break;
                        case "FD":
                            info.Title = "Angler's Club: Ultimate Bass Fishing 3D";
                            break;
                        case "FR":
                            info.Title = "Frogger 3D";
                            break;
                        case "HC":
                            info.Title = "James Noir's Hollywood Crimes";
                            break;
                        case "HF":
                            info.Title = "Happy Feet 2";
                            break;
                        case "HP":
                            info.Title = "LEGO Harry Potter: Years 5-7";
                            break;
                        case "GL":
                            info.Title = "Green Lantern: Rise of the Manhunters";
                            break;
                        case "GT":
                            info.Title = "Thor: God of Thunder";
                            break;
                        case "GR":
                            info.Title = "Tom Clancy's Ghost Recon: Shadow Wars";
                            break;
                        case "GU":
                            info.Title = "Imagine: Fashion Designer 3D";
                            break;
                        case "GX":
                            info.Title = "Generator Rex: Agent of Providence";
                            break;
                        case "KK":
                            info.Title = "Professor Layton & The Mask of Miracle";
                            break;
                        case "KZ":
                            info.Title = "Dreamworks Superstar Kartz";
                            break;
                        case "LD":
                            info.Title = "Doctor Lautrec and The Forgotten Knights";
                            break;
                        case "LF":
                            info.Title = "One Piece: Unlimited Cruise SP";
                            break;
                        case "LG":
                            info.Title = "LEGO Star Wars III: The Clone Wars";
                            break;
                        case "MD":
                            info.Title = "Madden NFL Football";
                            break;
                        case "MJ":
                            info.Title = "Michael Jackson: The Experience";
                            break;
                        case "MK":
                            info.Title = "Mario Kart 7";
                            break;
                        case "MS":
                            info.Title = "Marvel Super Hero Squad: The Infinity Gauntlet";
                            break;
                        case "NC":
                            info.Title = "NCIS";
                            break;
                        case "NR":
                            info.Title = "Star Fox 64 3D";
                            break;
                        case "NS":
                            info.Title = "Need for Speed: The Run";
                            break;
                        case "NT":
                            info.Title = "Naruto Shippuden 3D: The New Era";
                            break;
                        case "PC":
                            info.Title = "LEGO Pirates of The Caribbean: The Video Game";
                            break;
                        case "PG":
                            info.Title = "Pac-Man & Galaga Dimensions";
                            break;
                        case "PP":
                            info.Title = "DualPenSports";
                            break;
                        case "PU":
                            info.Title = "Mind Quiz";
                            break;
                        case "QE":
                            info.Title = "The Legend of Zelda: Ocarina of Time 3D";
                            break;
                        case "QN":
                            info.Title = "Cubic Ninja";
                            break;
                        case "RB":
                            info.Title = "Raving Rabbids: Travel in Time 3D";
                            break;
                        case "RE":
                            info.Title = "Super Mario 3D Land";
                            break;
                        case "RR":
                            info.Title = "Ridge Racer 3D";
                            break;
                        case "RY":
                            info.Title = "Rayman 3D";
                            break;
                        case "S3":
                            info.Title = "The Sims 3";
                            break;
                        case "S4":
                            info.Title = "The Sims 3: Pets";
                            break;
                        case "S7":
                            info.Title = "Spider-Man: Egde of Time";
                            break;
                        case "S9":
                            info.Title = "Sudoku: The Puzzle Game Collection";
                            break;
                        case "SC":
                            info.Title = "Tom Clancy's Splinter Cell 3D";
                            break;
                        case "SD":
                            info.Title = "Steel Diver";
                            break;
                        case "SF":
                            info.Title = "Asphalt 3D";
                            break;
                        case "SG":
                            info.Title = "SpongeBob SquigglePants";
                            break;
                        case "SM":
                            info.Title = "Super Monkey Ball 3D";
                            break;
                        case "SN":
                            info.Title = "Sonic Generations";
                            break;
                        case "SP":
                            info.Title = "Skylanders: Spyro's Adventure";
                            break;
                        case "SS":
                            info.Title = "Super Street Fighter IV: 3D Edition";
                            break;
                        case "SV":
                            info.Title = "Shinobi";
                            break;
                        case "TF":
                            info.Title = "Transformers 3: Dark Of The Moon - Stealth Force Edition";
                            break;
                        case "TL":
                            info.Title = "Tetris";
                            break;
                        case "TN":
                            info.Title = "The Adventures of Tintin: The Game";
                            break;
                        case "TT":
                            info.Title = "Combat of Giants: Dinosaurs 3D";
                            break;
                        case "WA":
                            info.Title = "Pilotwings Resort";
                            break;
                        case "WE":
                            info.Title = "WWE All Stars";
                            break;
                        case "ZO":
                            info.Title = "Zoo Resort 3D";
                            break;
                    }
                }
                else
                {
                    info.Title = productCode;
                }
            }
            if (developerChars != null)
            {
                var developerCode = developerChars[0].ToString() + developerChars[1];
                switch (developerCode)
                {
                    case "00":
                    case "01":
                        info.Developer = "Nintendo"; break;
                    case "02":
                        info.Developer = "Ajinomoto"; break;
                    case "03": 
                        info.Developer = "Imagineer-Zoom"; break;
                    case "04": 
                        info.Developer = "Gray Matter?"; break;
                    case "05": 
                        info.Developer = "Zamuse"; break;
                    case "06": 
                        info.Developer = "Falcom"; break;
                    case "07": 
                        info.Developer = "Enix?"; break;
                    case "08": 
                        info.Developer = "Capcom"; break;
                    case "09": 
                        info.Developer = "Hot B Co."; break;
                    case "0A": 
                        info.Developer = "Jaleco"; break;
                    case "0B":
                        info.Developer = "Coconuts Japan"; break;
                    case "0C":
                        info.Developer = "Coconuts Japan/G.X.Media"; break;
                    case "0D":
                        info.Developer = "Micronet?"; break;
                    case "0E":
                        info.Developer = "Technos"; break;
                    case "0F":
                        info.Developer = "Mebio Software"; break;
                    case "0G":
                        info.Developer = "Shouei System"; break;
                    case "0H":
                        info.Developer = "Starfish"; break;
                    case "0J":
                        info.Developer = "Mitsui Fudosan/Dentsu"; break;
                    case "0L":
                        info.Developer = "Warashi Inc."; break;
                    case "0N":
                        info.Developer = "Nowpro"; break;
                    case "0P":
                        info.Developer = "Game Village"; break;
                    case "0Q":
                        info.Developer = "IE Institute"; break;
                    case "10":
                        info.Developer = "Shogakukan"; break;
                    case "12":
                        info.Developer = "Infocom"; break;
                    case "13":
                        info.Developer = "Electronic Arts Japan"; break;
                    case "15":
                        info.Developer = "Cobra Team"; break;
                    case "16":
                        info.Developer = "Human/Field"; break;
                    case "17":
                        info.Developer = "KOEI"; break;
                    case "18":
                        info.Developer = "Hudson Soft"; break;
                    case "19":
                        info.Developer = "S.C.P."; break;
                    case "1A":
                        info.Developer = "Yanoman"; break;
                    case "1C":
                        info.Developer = "Tecmo Products"; break;
                    case "1D":
                        info.Developer = "Japan Glary Business"; break;
                    case "1E":
                        info.Developer = "Forum/OpenSystem"; break;
                    case "1F":
                        info.Developer = "Virgin Games"; break;
                    case "1G":
                        info.Developer = "SMDE"; break;
                    case "1J":
                        info.Developer = "Daikokudenki"; break;
                    case "1P":
                        info.Developer = "Creatures Inc."; break;
                    case "1Q":
                        info.Developer = "TDK Deep Impresion"; break;
                    case "20":
                        info.Developer = "Destination Software, KSS"; break;
                    case "21":
                        info.Developer = "Sunsoft/Tokai Engineering??"; break;
                    case "22":
                        info.Developer = "POW, VR 1 Japan??"; break;
                    case "23":
                        info.Developer = "Micro World"; break;
                    case "25":
                        info.Developer = "San-X"; break;
                    case "26":
                        info.Developer = "Enix"; break;
                    case "27":
                        info.Developer = "Loriciel/Electro Brain"; break;
                    case "28":
                        info.Developer = "Kemco Japan"; break;
                    case "29":
                        info.Developer = "Seta"; break;
                    case "2A":
                        info.Developer = "Culture Brain"; break;
                    case "2C":
                        info.Developer = "Palsoft"; break;
                    case "2D":
                        info.Developer = "Visit Co.,Ltd."; break;
                    case "2E":
                        info.Developer = "Intec"; break;
                    case "2F":
                        info.Developer = "System Sacom"; break;
                    case "2G":
                        info.Developer = "Poppo"; break;
                    case "2H":
                        info.Developer = "Ubisoft Japan"; break;
                    case "2J":
                        info.Developer = "Media Works"; break;
                    case "2K":
                        info.Developer = "NEC InterChannel"; break;
                    case "2L":
                        info.Developer = "Tam"; break;
                    case "2M":
                        info.Developer = "Jordan"; break;
                    case "2N":
                        info.Developer = "Rocket Science Games"; break;
                    case "2Q":
                        info.Developer = "Mediakite"; break;
                    case "30":
                        info.Developer = "Viacom"; break;
                    case "31":
                        info.Developer = "Carrozzeria"; break;
                    case "32":
                        info.Developer = "Dynamic"; break;
                        // case "33": info.Developer = "NOT A COMPANY!"; break; //in hex it's 3333
                    case "34":
                        info.Developer = "Magifact"; break;
                    case "35":
                        info.Developer = "Hect"; break;
                    case "36":
                        info.Developer = "Codemasters"; break;
                    case "37":
                        info.Developer = "Taito/GAGA Communications"; break;
                    case "38":
                        info.Developer = "Laguna"; break;
                    case "39":
                        info.Developer = "Telstar Fun & Games, Event/Taito"; break;
                    case "3B":
                        info.Developer = "Arcade Zone Ltd"; break;
                    case "3C":
                        info.Developer = "Entertainment International/Empire Software?"; break;
                    case "3D":
                        info.Developer = "Loriciel"; break;
                    case "3E":
                        info.Developer = "Gremlin Graphics"; break;
                    case "3F":
                        info.Developer = "K.Amusement Leasing Co."; break;
                    case "40":
                        info.Developer = "Seika Corp."; break;
                    case "41":
                        info.Developer = "Ubi Soft Entertainment"; break;
                    case "42":
                        info.Developer = "Sunsoft US?"; break;
                    case "44":
                        info.Developer = "Life Fitness"; break;
                    case "46":
                        info.Developer = "System 3"; break;
                    case "47":
                        info.Developer = "Spectrum Holobyte"; break;
                    case "49":
                        info.Developer = "IREM"; break;
                    case "4B":
                        info.Developer = "Raya Systems"; break;
                    case "4C":
                        info.Developer = "Renovation Products"; break;
                    case "4D":
                        info.Developer = "Malibu Games"; break;
                    case "4F":
                        info.Developer = "Eidos"; break;
                    case "4G":
                        info.Developer = "Playmates Interactive?"; break;
                    case "4J":
                        info.Developer = "Fox Interactive"; break;
                    case "4K":
                        info.Developer = "Time Warner Interactive"; break;
                    case "4Q":
                        info.Developer = "Disney Interactive"; break;
                    case "4S":
                        info.Developer = "Black Pearl"; break;
                    case "4U":
                        info.Developer = "Advanced Productions"; break;
                    case "4X":
                        info.Developer = "GT Interactive"; break;
                    case "4Y":
                        info.Developer = "RARE?"; break;
                    case "4Z":
                        info.Developer = "Crave Entertainment"; break;
                    case "50":
                        info.Developer = "Absolute Entertainment"; break;
                    case "51":
                        info.Developer = "Acclaim"; break;
                    case "52":
                        info.Developer = "Activision"; break;
                    case "53":
                        info.Developer = "American Sammy"; break;
                    case "54":
                        info.Developer = "Take 2 Interactive (before it was GameTek)"; break;
                    case "55":
                        info.Developer = "Hi Tech"; break;
                    case "56":
                        info.Developer = "LJN LTD."; break;
                    case "58":
                        info.Developer = "Mattel"; break;
                    case "5A":
                        info.Developer = "Mindscape, Red Orb Entertainment?"; break;
                    case "5B":
                        info.Developer = "Romstar"; break;
                    case "5C":
                        info.Developer = "Taxan"; break;
                    case "5D":
                        info.Developer = "Midway (before it was Tradewest)"; break;
                    case "5F":
                        info.Developer = "American Softworks"; break;
                    case "5G":
                        info.Developer = "Majesco Sales Inc"; break;
                    case "5H":
                        info.Developer = "3DO"; break;
                    case "5K":
                        info.Developer = "Hasbro"; break;
                    case "5L":
                        info.Developer = "NewKidCo"; break;
                    case "5M":
                        info.Developer = "Telegames"; break;
                    case "5N":
                        info.Developer = "Metro3D"; break;
                    case "5P":
                        info.Developer = "Vatical Entertainment"; break;
                    case "5Q":
                        info.Developer = "LEGO Media"; break;
                    case "5S":
                        info.Developer = "Xicat Interactive"; break;
                    case "5T":
                        info.Developer = "Cryo Interactive"; break;
                    case "5W":
                        info.Developer = "Red Storm Entertainment"; break;
                    case "5X":
                        info.Developer = "Microids"; break;
                    case "5Z":
                        info.Developer = "Conspiracy/Swing"; break;
                    case "60":
                        info.Developer = "Titus"; break;
                    case "61":
                        info.Developer = "Virgin Interactive"; break;
                    case "62":
                        info.Developer = "Maxis"; break;
                    case "64":
                        info.Developer = "LucasArts Entertainment"; break;
                    case "67":
                        info.Developer = "Ocean"; break;
                    case "69":
                        info.Developer = "Electronic Arts"; break;
                    case "6B":
                        info.Developer = "Laser Beam"; break;
                    case "6E":
                        info.Developer = "Elite Systems"; break;
                    case "6F":
                        info.Developer = "Electro Brain"; break;
                    case "6G":
                        info.Developer = "The Learning Company"; break;
                    case "6H":
                        info.Developer = "BBC"; break;
                    case "6J":
                        info.Developer = "Software 2000"; break;
                    case "6L":
                        info.Developer = "BAM! Entertainment"; break;
                    case "6M":
                        info.Developer = "Studio 3"; break;
                    case "6Q":
                        info.Developer = "Classified Games"; break;
                    case "6S":
                        info.Developer = "TDK Mediactive"; break;
                    case "6U":
                        info.Developer = "DreamCatcher"; break;
                    case "6V":
                        info.Developer = "JoWood Produtions"; break;
                    case "6W":
                        info.Developer = "SEGA"; break;
                    case "6X":
                        info.Developer = "Wannado Edition"; break;
                    case "6Y":
                        info.Developer = "LSP"; break;
                    case "6Z":
                        info.Developer = "ITE Media"; break;
                    case "70":
                        info.Developer = "Infogrames"; break;
                    case "71":
                        info.Developer = "Interplay"; break;
                    case "72":
                        info.Developer = "JVC"; break;
                    case "73":
                        info.Developer = "Parker Brothers"; break;
                    case "75":
                        info.Developer = "Sales Curve"; break;
                    case "78":
                        info.Developer = "THQ"; break;
                    case "79":
                        info.Developer = "Accolade"; break;
                    case "7A":
                        info.Developer = "Triffix Entertainment"; break;
                    case "7C":
                        info.Developer = "Microprose Software"; break;
                    case "7D":
                        info.Developer = "Universal Interactive, Sierra, Simon & Schuster?"; break;
                    case "7F":
                        info.Developer = "Kemco"; break;
                    case "7G":
                        info.Developer = "Rage Software"; break;
                    case "7H":
                        info.Developer = "Encore"; break;
                    case "7J":
                        info.Developer = "Zoo"; break;
                    case "7K":
                        info.Developer = "BVM"; break;
                    case "7L":
                        info.Developer = "Simon & Schuster Interactive"; break;
                    case "7M":
                        info.Developer = "Asmik Ace Entertainment Inc./AIA"; break;
                    case "7N":
                        info.Developer = "Empire Interactive?"; break;
                    case "7Q":
                        info.Developer = "Jester Interactive"; break;
                    case "7T":
                        info.Developer = "Scholastic"; break;
                    case "7U":
                        info.Developer = "Ignition Entertainment"; break;
                    case "7W":
                        info.Developer = "Stadlbauer"; break;
                    case "80":
                        info.Developer = "Misawa"; break;
                    case "81":
                        info.Developer = "Teichiku"; break;
                    case "82":
                        info.Developer = "Namco Ltd."; break;
                    case "83":
                        info.Developer = "LOZC"; break;
                    case "84":
                        info.Developer = "KOEI"; break;
                    case "86":
                        info.Developer = "Tokuma Shoten Intermedia"; break;
                    case "87":
                        info.Developer = "Tsukuda Original"; break;
                    case "88":
                        info.Developer = "DATAM-Polystar"; break;
                    case "8B":
                        info.Developer = "Bulletproof Software"; break;
                    case "8C":
                        info.Developer = "Vic Tokai Inc."; break;
                    case "8E":
                        info.Developer = "Character Soft"; break;
                    case "8F":
                        info.Developer = "I'Max"; break;
                    case "8G":
                        info.Developer = "Saurus"; break;
                    case "8J":
                        info.Developer = "General Entertainment"; break;
                    case "8N":
                        info.Developer = "Success"; break;
                    case "8P":
                        info.Developer = "SEGA Japan"; break;
                    case "90":
                        info.Developer = "Takara Amusement"; break;
                    case "91":
                        info.Developer = "Chun Soft"; break;
                    case "92":
                        info.Developer = "Video System, McO'River???"; break;
                    case "93":
                        info.Developer = "BEC"; break;
                    case "95":
                        info.Developer = "Varie"; break;
                    case "96":
                        info.Developer = "Yonezawa/S'pal"; break;
                    case "97":
                        info.Developer = "Kaneko"; break;
                    case "99":
                        info.Developer = "Victor Interactive Software, Pack in Video"; break;
                    case "9A":
                        info.Developer = "Nichibutsu/Nihon Bussan"; break;
                    case "9B":
                        info.Developer = "Tecmo"; break;
                    case "9C":
                        info.Developer = "Imagineer"; break;
                    case "9F":
                        info.Developer = "Nova"; break;
                    case "9G":
                        info.Developer = "Den'Z"; break;
                    case "9H":
                        info.Developer = "Bottom Up"; break;
                    case "9J":
                        info.Developer = "TGL"; break;
                    case "9L":
                        info.Developer = "Hasbro Japan?"; break;
                    case "9N":
                        info.Developer = "Marvelous Entertainment"; break;
                    case "9P":
                        info.Developer = "Keynet Inc."; break;
                    case "9Q":
                        info.Developer = "Hands-On Entertainment"; break;
                    case "A0":
                        info.Developer = "Telenet"; break;
                    case "A1":
                        info.Developer = "Hori"; break;
                    case "A4":
                        info.Developer = "Konami"; break;
                    case "A5":
                        info.Developer = "K.Amusement Leasing Co."; break;
                    case "A6":
                        info.Developer = "Kawada"; break;
                    case "A7":
                        info.Developer = "Takara"; break;
                    case "A9":
                        info.Developer = "Technos Japan Corp."; break;
                    case "AA":
                        info.Developer = "JVC, Victor Musical Indutries"; break;
                    case "AC":
                        info.Developer = "Toei Animation"; break;
                    case "AD":
                        info.Developer = "Toho"; break;
                    case "AF":
                        info.Developer = "Namco"; break;
                    case "AG":
                        info.Developer = "Media Rings Corporation"; break;
                    case "AH":
                        info.Developer = "J-Wing"; break;
                    case "AJ":
                        info.Developer = "Pioneer LDC"; break;
                    case "AK":
                        info.Developer = "KID"; break;
                    case "AL":
                        info.Developer = "Mediafactory"; break;
                    case "AP":
                        info.Developer = "Infogrames Hudson"; break;
                    case "AQ":
                        info.Developer = "Kiratto. Ludic Inc"; break;
                    case "B0":
                        info.Developer = "Acclaim Japan"; break;
                    case "B1":
                        info.Developer = "ASCII (was Nexoft?)"; break;
                    case "B2":
                        info.Developer = "Bandai"; break;
                    case "B4":
                        info.Developer = "Enix"; break;
                    case "B6":
                        info.Developer = "HAL Laboratory"; break;
                    case "B7":
                        info.Developer = "SNK"; break;
                    case "B9":
                        info.Developer = "Pony Canyon"; break;
                    case "BA":
                        info.Developer = "Culture Brain"; break;
                    case "BB":
                        info.Developer = "Sunsoft"; break;
                    case "BC":
                        info.Developer = "Toshiba EMI"; break;
                    case "BD":
                        info.Developer = "Sony Imagesoft"; break;
                    case "BF":
                        info.Developer = "Sammy"; break;
                    case "BG":
                        info.Developer = "Magical"; break;
                    case "BH":
                        info.Developer = "Visco"; break;
                    case "BJ":
                        info.Developer = "Compile "; break;
                    case "BL":
                        info.Developer = "MTO Inc."; break;
                    case "BN":
                        info.Developer = "Sunrise Interactive"; break;
                    case "BP":
                        info.Developer = "Global A Entertainment"; break;
                    case "BQ":
                        info.Developer = "Fuuki"; break;
                    case "C0":
                        info.Developer = "Taito"; break;
                    case "C2":
                        info.Developer = "Kemco"; break;
                    case "C3":
                        info.Developer = "Square"; break;
                    case "C4":
                        info.Developer = "Tokuma Shoten"; break;
                    case "C5":
                        info.Developer = "Data East"; break;
                    case "C6":
                        info.Developer = "Tonkin House	(was Tokyo Shoseki)"; break;
                    case "C8":
                        info.Developer = "Koei"; break;
                    case "CA":
                        info.Developer = "Konami/Ultra/Palcom"; break;
                    case "CB":
                        info.Developer = "NTVIC/VAP"; break;
                    case "CC":
                        info.Developer = "Use Co.,Ltd."; break;
                    case "CD":
                        info.Developer = "Meldac"; break;
                    case "CE":
                        info.Developer = "Pony Canyon"; break;
                    case "CF":
                        info.Developer = "Angel, Sotsu Agency/Sunrise"; break;
                    case "CJ":
                        info.Developer = "Boss"; break;
                    case "CG":
                        info.Developer = "Yumedia/Aroma Co., Ltd"; break;
                    case "CK":
                        info.Developer = "Axela/Crea-Tech?"; break;
                    case "CL":
                        info.Developer = "Sekaibunka-Sha, Sumire kobo?, Marigul Management Inc.?"; break;
                    case "CM":
                        info.Developer = "Konami Computer Entertainment Osaka"; break;
                    case "CP":
                        info.Developer = "Enterbrain"; break;
                    case "D0":
                        info.Developer = "Taito/Disco"; break;
                    case "D1":
                        info.Developer = "Sofel"; break;
                    case "D2":
                        info.Developer = "Quest, Bothtec"; break;
                    case "D3":
                        info.Developer = "Sigma, ?????"; break;
                    case "D4":
                        info.Developer = "Ask Kodansha"; break;
                    case "D6":
                        info.Developer = "Naxat"; break;
                    case "D7":
                        info.Developer = "Copya System"; break;
                    case "D8":
                        info.Developer = "Capcom Co., Ltd."; break;
                    case "D9":
                        info.Developer = "Banpresto"; break;
                    case "DA":
                        info.Developer = "TOMY"; break;
                    case "DB":
                        info.Developer = "LJN Japan"; break;
                    case "DD":
                        info.Developer = "NCS"; break;
                    case "DE":
                        info.Developer = "Human Entertainment"; break;
                    case "DF":
                        info.Developer = "Altron"; break;
                    case "DG":
                        info.Developer = "Jaleco???"; break;
                    case "DH":
                        info.Developer = "Gaps Inc."; break;
                    case "DL":
                        info.Developer = "????"; break;
                    case "DN":
                        info.Developer = "Elf"; break;
                    case "E0":
                        info.Developer = "Jaleco"; break;
                    case "E1":
                        info.Developer = "????"; break;
                    case "E2":
                        info.Developer = "Yutaka"; break;
                    case "E3":
                        info.Developer = "Varie"; break;
                    case "E4":
                        info.Developer = "T&ESoft"; break;
                    case "E5":
                        info.Developer = "Epoch"; break;
                    case "E7":
                        info.Developer = "Athena"; break;
                    case "E8":
                        info.Developer = "Asmik"; break;
                    case "E9":
                        info.Developer = "Natsume"; break;
                    case "EA":
                        info.Developer = "King Records"; break;
                    case "EB":
                        info.Developer = "Atlus"; break;
                    case "EC":
                        info.Developer = "Epic/Sony Records"; break;
                    case "EE":
                        info.Developer = "IGS"; break;
                    case "EG":
                        info.Developer = "Chatnoir"; break;
                    case "EH":
                        info.Developer = "Right Stuff"; break;
                    case "EJ":
                        info.Developer = "????"; break;
                    case "EL":
                        info.Developer = "Spike"; break;
                    case "EM":
                        info.Developer = "Konami Computer Entertainment Tokyo"; break;
                    case "EN":
                        info.Developer = "Alphadream Corporation"; break;
                    case "F0":
                        info.Developer = "A Wave"; break;
                    case "F1":
                        info.Developer = "Motown Software"; break;
                    case "F2":
                        info.Developer = "Left Field Entertainment"; break;
                    case "F3":
                        info.Developer = "Extreme Ent. Grp."; break;
                    case "F4":
                        info.Developer = "TecMagik"; break;
                    case "F9":
                        info.Developer = "Cybersoft"; break;
                    case "FB":
                        info.Developer = "Psygnosis"; break;
                    case "FE":
                        info.Developer = "Davidson/Western Tech."; break;
                    case "G1":
                        info.Developer = "PCCW Japan"; break;
                    case "G4":
                        info.Developer = "KiKi Co Ltd"; break;
                    case "G5":
                        info.Developer = "Open Sesame Inc???"; break;
                    case "G6":
                        info.Developer = "Sims"; break;
                    case "G7":
                        info.Developer = "Broccoli"; break;
                    case "G8":
                        info.Developer = "Avex"; break;
                    case "G9":
                        info.Developer = "D3 Publisher"; break;
                    case "GB":
                        info.Developer = "Konami Computer Entertainment Japan"; break;
                    case "GD":
                        info.Developer = "Square-Enix"; break;
                    case "IH":
                        info.Developer = "Yojigen"; break;
                    default:
                        info.Developer = developerCode + "-Unknown";
                        break;
                }
            }
            return info;
        }
    }
}
