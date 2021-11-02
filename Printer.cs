using System;

namespace Tweet_DL
{
    public class Printer
    {
        public static void Print(ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
        public static void PrintLine(ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
