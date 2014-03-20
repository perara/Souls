using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsServer.Tools
{
    public class Logging
    {
        public enum Type
        {
            GENERAL = 0,
            GAME = 1,
            CHAT = 2,
            GAMEQUEUE = 3,
            ERROR = 4
        }

        public Logging()
        {

        }

        static public void Write(Type type, string message)
        {
            string write;

            switch (type)
            {
                case Type.GENERAL:
                    Console.ForegroundColor = ConsoleColor.White;
                    write = "[GENERAL]\t";
                    break;
                case Type.GAME:
                    Console.ForegroundColor = ConsoleColor.Green;
                    write = "[GAME]\t\t";
                    break;
                case Type.CHAT:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    write = "[CHAT]\t\t";
                    break;
                case Type.GAMEQUEUE:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    write = "[GAMEQUEUE]\t";
                    break;
                case Type.ERROR:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    write = "[ERROR]\t\t";
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    write = "[UNDEFINED]\t\t";
                    break;
            }

            write += message;
            Console.WriteLine(write);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;

        }
    }
}
