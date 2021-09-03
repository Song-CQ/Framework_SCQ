using System;

namespace ExcelTool.Tool
{
    public static class StringColor
    {
        public static void WriteLine(object val,ConsoleColor color = ConsoleColor.Red)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = color;
            Console.WriteLine(val);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}