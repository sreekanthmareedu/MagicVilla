using System;

namespace MagicVilla_VillaAPI.logging
{
    public class LoggingV2 : Ilogging
    {
        public void Log(string message,string type)
        {
         
            if (type == "error")
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Error -" + message);
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                if (type == "warning")
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Error -" + message);
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }

    }
}
