namespace _3DSExplorer
{
    public static class GameTitleResolver
    {
        public static string Resolve(char[] chars)
        {
            var productCode = chars[7].ToString() + chars[8];
            switch (productCode)
            {
                case "66": return "Samurai Warriors: Chronicles";
                case "AB": return "Tales of the Abyss";
                case "AZ": return "Cars 2";
                case "BB": return "Puzzle Bobble Universe";
                case "BL": return "Blazblue: Continuum Shift II";
                case "BM": return "Resident Evil The Meercenaries 3D";
                case "C3": return "Ace Combat: Assault Horizon Legacy";
                case "CC": return "Super Pokémon Rumble";
                case "CN": return "Cartoon Network: Punch Time Explosion";
                case "CQ": return "Cooking Mama 4: Kitchen Magic";
                case "CT": return "Puppies World 3D";
                case "CV": return "Cave Story 3D";
                case "DA": return "Nintendogs + Cats: Golden Retriever & New Friends";
                case "DB": return "Nintendogs + Cats: French Bulldog & New Friends";
                case "DC": return "Nintendogs + Cats: Toy Poodle & New Friends";
                case "DD": return "Dead or Alive: Dimensions";
                case "DE": return "Sports Island 3D";
                case "DR": return "Driver Renegate 3D";
                case "DT": return "Dream Trigger 3D";
                case "EE": return "Pro Evolution Soccer 2011 3D";
                case "F2": return "FIFA 12";
                case "F4": return "F1 2011";
                case "FD": return "Angler's Club - Ultimate Bass Fishing 3D";
                case "FR": return "Frogger 3D";
                case "HC": return "James Noir's Hollywood Crimes";
                case "HF": return "Happy Feet 2";
                case "HP": return "LEGO Harry Potter: Years 5-7";
                case "GL": return "Green Lantern: Rise of the Manhunters";
                case "GT": return "Thor - God of Thunder";
                case "GR": return "Tom Clancy's Ghost Recon: Shadow Wars";
                case "GU": return "Imagine: Fashion Designer 3D";
                case "GX": return "Generator Rex: Agent of Providence";
                case "KK": return "Professor Layton & The Mask of Miracle";
                case "KZ": return "Dreamworks Superstar Kartz";
                case "LD": return "Doctor Lautrec and The Forgotten Knights";
                case "LF": return "One Piece: Unlimited Cruise SP";
                case "LG": return "LEGO Star Wars III: The Clone Wars";
                case "MD": return "Madden NFL Football";
                case "MJ": return "Michael Jackson : The Experience";
                case "MK": return "Mario Kart 7";
                case "MS": return "Marvel Super Hero Squad: The Infinity Gauntlet";
                case "NC": return "NCIS";
                case "NR": return "Star Fox 64 3D";
                case "NS": return "Need for Speed: The Run";
                case "NT": return "Naruto Shippuden 3D: The New Era";
                case "PC": return "LEGO Pirates of The Caribbean - The Video Game";
                case "PG": return "Pac-Man & Galaga Dimensions";
                case "PP": return "DualPenSports";
                case "PU": return "Mind Quiz";
                case "QE": return "The Legend of Zelda: Ocarina of Time 3D";
                case "QN": return "Cubic Ninja";               
                case "RB": return "Raving Rabbids: Travel in Time 3D";
                case "RE": return "Super Mario 3D Land";
                case "RR": return "Ridge Racer 3D";
                case "RY": return "Rayman 3D";
                case "S3": return "The Sims 3";
                case "S4": return "The Sims 3: Pets";
                case "S7": return "Spider-Man: Egde of Time";
                case "S9": return "Sudoku: The Puzzle Game Collection";
                case "SC": return "Tom Clancy's Splinter Cell 3D";
                case "SD": return "Steel Diver";
                case "SF": return "Asphalt 3D";
                case "SG": return "SpongeBob SquigglePants";
                case "SM": return "Super Monkey Ball 3D";
                case "SN": return "Sonic Generations";
                case "SP": return "Skylanders: Spyro's Adventure";
                case "SS": return "Super Street Fighter IV - 3D Edition";
                case "SV": return "Shinobi";
                case "TF": return "Transformers 3: Dark Of The Moon - Stealth Force Edition";
                case "TL": return "Tetris";
                case "TN": return "The Adventures of Tintin - The Game";
                case "TT": return "Combat of Giants: Dinosaurs 3D";
                case "WA": return "Pilotwings Resort";
                case "WE": return "WWE All Stars";
                case "ZO": return "Zoo Resort 3D";
                default: return "<Unknown>";
            }
        }
    }
}
