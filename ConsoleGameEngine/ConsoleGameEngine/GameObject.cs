using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleGameEngine
{

    public class GameObject
    {

        public int PosX { get; private set; }
        public int PosY { get; private set; }

        public bool Active = true;

        public Symbol[,] Content;

        public static Core core;

        public List<GameObjectDelegate> OnClick = new List<GameObjectDelegate>();
        public List<GameObjectDelegate> Update = new List<GameObjectDelegate>();

        public void Move(string direction)
        {
            switch (direction)
            {

                case "up":
                    PosY--;
                    break;
                case "down":
                    PosY++;
                    break;
                case "left":
                    PosX--;
                    break;
                case "right":
                    PosX++;
                    break;
            }
        }

        private void Flip(string direction)
        {
            direction = direction.ToLower();
            byte[,] Result;
            switch (direction)
            {
                case "clockwise":
                    Result = new byte[Content.GetLength(1), Content.GetLength(0)];

                    break;
                case "counterclockwise":
                    Result = new byte[Content.GetLength(1), Content.GetLength(0)];
                    break;
            }
        }

        public void Move(int PosY, int PosX)
        {
            this.PosY = PosY;
            this.PosX = PosX;
        }

        public GameObject(int PosY, int PosX, string path)
        {
            try
            {
                this.PosX = PosX;
                this.PosY = PosY;

                string[] line = File.ReadAllLines(path);

                // If file empty create invisible pixel (more safe than empty array)
                if(line.Length < 1)
                {
                    Content = new Symbol[,] { { new Symbol(' ', 0x52) } };

                    return;
                }

                Content = new Symbol[line.Length, line[0].Replace(" ", "").Length / 2];

                int i = 0;
                foreach (string str in line)
                {
                    byte[] Result = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(i => byte.Parse(i, System.Globalization.NumberStyles.HexNumber)).ToArray();

                    for (int j = 0; j < line[0].Replace(" ", "").Length / 2; j++)
                    {
                        try
                        {
                            Content[i, j] = new Symbol(' ', Result[j]);
                        }
                        catch(Exception e)
                        {
                            Content[i, j] = new Symbol(' ', 0x52);
                            continue;
                        }
                    }

                    i++;

                }

                if(core != null)
                {
                    core.Objects.Add(this);
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public GameObject(int PosY, int PosX, Symbol[,] Content)
        {
            this.PosY = PosY;
            this.PosX = PosX;

            this.Content = Content;

            if (core != null)
            {
                core.Objects.Add(this);
            }
        }

        public void Move(ConsoleKey direction)
        {
            switch (direction)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    PosY--;
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    PosY++;
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    PosX--;
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    PosX++;
                    break;
            }
        }

    }

}
