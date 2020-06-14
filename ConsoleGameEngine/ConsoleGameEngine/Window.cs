using System;
using System.Collections.Generic;

namespace ConsoleGameEngine
{
    public class Window
    {
        public int SizeY { get; private set; } // Height
        public int SizeX { get; private set; } // Width

        public Symbol[,] Content { get; private set; }

        public Window(int sizeY, int sizeX)
        {
            SizeY = sizeY;
            SizeX = sizeX;

            Content = new Symbol[sizeY, sizeX];

            for (int i = 0; i < SizeY; i++)
            {
                for (int j = 0; j < SizeX; j++)
                {
                    Content[i, j] = new Symbol(' ', 0x00);
                }
            }
        }

        public void Draw()
        {
            Console.Clear();
            for (int i = 0; i < SizeY; i++)
            {
                for (int j = 0; j < SizeX; j++)
                {
                    Console.Write("\x1b[48;5;" + Content[i, j].Color + "m" + Content[i, j].Value);
                }
                Console.WriteLine();
            }
            Console.Write("\x1b[48;5;255m");
        }

    }
}
