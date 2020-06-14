using System;
namespace ConsoleGameEngine
{
    public class Snake : Symbol
    {

        public string Direction;
        public char Texture;

        public Snake(char Value, byte Color, string Direction) : base (Value, Color)
        {
            this.Direction = Direction;
        }

        public void Control()
        {
            while(true)
            {
                switch(Console.ReadKey().Key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        if(Direction != "down")
                            Direction = "up";
                        break;
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        if (Direction != "right")
                            Direction = "left";
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        if (Direction != "left")
                            Direction = "right";
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        if (Direction != "up")
                            Direction = "down";
                        break;
                }
            }
        }

    }
}
