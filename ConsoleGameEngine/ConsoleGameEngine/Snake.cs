using System;
using System.Text;

namespace ConsoleGameEngine
{
    public class Snake : Symbol
    {

        public string Direction;
        public string setDir;
        public char Texture;

        public Snake(char Value, byte Color, string Direction) : base (Value, Color)
        {
            this.Direction = Direction;
            setDir = Direction;
        }

        public void Control()
        {
            System.Text.EncodingProvider provider = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);
            while (true)
            {
                switch(Console.ReadKey().Key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        if(Direction != "down")
                            setDir = "up";
                        break;
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        if (Direction != "right")
                            setDir = "left";
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        if (Direction != "left")
                            setDir = "right";
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        if (Direction != "up")
                            setDir = "down";
                        break;
                }
            }
        }

    }
}
