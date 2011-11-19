using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DSExplorer
{
    class GameTitleResolver
    {
        public static string Resolve(char[] chars)
        {
            string productCode = chars[7].ToString() + chars[8].ToString();
            switch (productCode)
            {
                case "SF": return "Asphalt 3D";
                case "QN": return "Cubic Ninja";
                case "DD": return "Dead or Alive - Dimensions";
                case "LG": return "Lego Star Wars III: The Clone Wars";
                case "DA": return "Nintendogs+Cats (Kiosk Demo)";
                case "DB": return "Nintendogs+Cats: French Bulldog & New Friends";
                case "DC": return "Nintendogs+Cats: Toy Poodle & New Friends";
                case "LF": return "One Piece: Unlimited Cruise SP";
                case "WA": return "Pilotwings Resort";
                case "BB": return "Puzzle Bobble Universe";
                case "RB": return "Raving Rabbids: Travel in Time 3D";
                case "BM": return "Resident Evil The Meercenaries 3D";
                case "RR": return "Ridge Racer 3D";
                case "66": return "Samurai Warriors - Chronicles";
                case "SC": return "Splinter Cell 3D";
                case "SD": return "Steel Diver";
                case "SS": return "Super Street Fighter IV - 3D Edition";
                case "RE": return "Super Mario 3D Land";
                case "SM": return "Super Monkey Ball 3D";
                case "QE": return "The Legend of Zelda: Ocarina of Time 3D";
                case "S3": return "The Sims 3";
                case "GR": return "Tom Clancy's Ghost Recon: Shadow Wars";
                case "KK": return "Professor Layton & The Mask of Miracle";
                default: return "(Unknown)";
            }
        }
    }
}
