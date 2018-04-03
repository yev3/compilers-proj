using System;

namespace CompilerILGen
{
    using static ConsoleColor;


    public class OutColor : IDisposable
    {
        public static OutColor Black => new OutColor(ConsoleColor.Black);
        public static OutColor DarkBlue => new OutColor(ConsoleColor.DarkBlue);
        public static OutColor DarkGreen => new OutColor(ConsoleColor.DarkGreen);
        public static OutColor DarkCyan => new OutColor(ConsoleColor.DarkCyan);
        public static OutColor DarkRed => new OutColor(ConsoleColor.DarkRed);
        public static OutColor DarkMagenta => new OutColor(ConsoleColor.DarkMagenta);
        public static OutColor DarkYellow => new OutColor(ConsoleColor.DarkYellow);
        public static OutColor Gray => new OutColor(ConsoleColor.Gray);
        public static OutColor DarkGray => new OutColor(ConsoleColor.DarkGray);
        public static OutColor Blue => new OutColor(ConsoleColor.Blue);
        public static OutColor Green => new OutColor(ConsoleColor.Green);
        public static OutColor Cyan => new OutColor(ConsoleColor.Cyan);
        public static OutColor Red => new OutColor(ConsoleColor.Red);
        public static OutColor Magenta => new OutColor(ConsoleColor.Magenta);
        public static OutColor Yellow => new OutColor(ConsoleColor.Yellow);
        public static OutColor White => new OutColor(ConsoleColor.White);

        public static OutColor WithColor(ConsoleColor newFore,
            ConsoleColor? newBack = null)
        {
            return new OutColor(newFore, newBack);
        }

        private ConsoleColor _originalForeColor;
        private ConsoleColor _originalBackColor;

        private OutColor(ConsoleColor newFore, ConsoleColor? newBack = null)
        {
            _originalForeColor = Console.ForegroundColor;
            _originalBackColor = Console.BackgroundColor;
            Console.ForegroundColor = newFore;
            if (newBack.HasValue)
            {
                Console.BackgroundColor = newBack.Value;
            }
        }

        public void Dispose()
        {
            Console.ForegroundColor = _originalForeColor;
            Console.BackgroundColor = _originalBackColor;
        }
    }
}