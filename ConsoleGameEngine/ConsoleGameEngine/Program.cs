using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace ConsoleGameEngine
{

    public delegate void InputDelegate(ConsoleKey input);
    public delegate void GameObjectDelegate(GameObject gameObject);


    class Program
    {

        private static int SizeY = 20; // Console height
        private static int SizeX = 20; // Console width

        private static bool FoodSpawn = false; // Food exist
        public static GameObject Food;
        public static List<GameObject> SnakeBody = new List<GameObject>();

        public static Random random = new Random();


        private static string rootPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())))) + "/assets/";

        static void Main(string[] args)
        {

            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            // Создаем движок
            Core core = new Core();
            // Создаем окно отрисовки
            core.CreateWindow(SizeY, SizeX);


            GameObject.core = core;


            Food = new GameObject(-10, -10, new Symbol[,] { { new Symbol('*', 0xC0) } });

            SnakeBody.Add(new GameObject(5, 5, new Snake[,]
            {
                { new Snake('-', 0xDE, "right") }
            }));
            SnakeBody.Add(new GameObject(5, 4, new Snake[,]
           {
                { new Snake('#', 0xCC, "right") }
           }));
            SnakeBody.Add(new GameObject(5, 3, new Snake[,]
           {
                { new Snake('#', 0xCC, "right") }
           }));
            SnakeBody.Add(new GameObject(5, 2, new Snake[,]
           {
                { new Snake('#', 0xCC, "right") }
           }));


            // При нажатии на пробел будет
            // Просчитываться клик по объекту
            core.clickButtons.Add(ConsoleKey.Spacebar);




            


            // Создаем новый поток для чтения ввода пользователя
            Thread inputReader = new Thread((SnakeBody[0].Content[0, 0] as Snake).Control);
            inputReader.Start();

            Thread gameDrawer = new Thread(core.DrawContent);
            gameDrawer.Start();

            //squre.OnClick.Add((gameObject) => { gameObject.Active = false; });



            /*Cursor.Update.Add((gameObject) =>
            {

                for (int i = 0; i < gameObject.Content.GetLength(0); i++)
                {
                    for (int j = 0; j < gameObject.Content.GetLength(1); j++)
                    {
                        gameObject.Content[i, j] = (byte)((gameObject.Content[i, j] + 1) % 255);
                    }
                }

            });*/;

        }

        public static void SpawnFood(GameObject Food)
        {
            if (!FoodSpawn & random.Next(0, 100) <= 7)
            {
                Food.Move(random.Next(2, SizeY - 3), random.Next(2, SizeX - 3));
                FoodSpawn = true;
            }
        }

        public static void MoveSnake()
        {
            (SnakeBody[0].Content[0, 0] as Snake).Direction = (SnakeBody[0].Content[0, 0] as Snake).setDir;
            switch ((SnakeBody[0].Content[0, 0] as Snake).Direction)
            {
                case "up":
                    SnakeBody[0].Content[0, 0].Value = '^';
                    break;
                case "left":
                    SnakeBody[0].Content[0, 0].Value = '<';
                    break;
                case "right":
                    SnakeBody[0].Content[0, 0].Value = '>';
                    break;
                case "down":
                    SnakeBody[0].Content[0, 0].Value = 'V';
                    break;
            }

            bool spawn = false;
            int posX = 0;
            int posY = 0;
            string Dir = "up";

            for (int i = 0; i < Program.SnakeBody.Count; i++)
            {
                (SnakeBody[i].Content[0, 0] as Snake).Direction = (SnakeBody[i].Content[0, 0] as Snake).setDir;
                if (i != 0 && SnakeBody[i].PosX == SnakeBody[0].PosX && SnakeBody[i].PosY == SnakeBody[0].PosY)
                {
                    Console.WriteLine("You snake Ded(");
                    System.Environment.Exit(0);
                }
                

                GameObject gameObject = SnakeBody[i];
                

                gameObject.Move((gameObject.Content[0, 0] as Snake).Direction);

                if (Food.PosY == gameObject.PosY && Food.PosX == gameObject.PosX)
                {
                    Food.Move(-10, -10);
                    FoodSpawn = false;

                    GameObject lastSnake = SnakeBody[SnakeBody.Count - 1];

                    spawn = true;
                    posY = lastSnake.PosY;
                    posX = lastSnake.PosX;
                    Dir = (lastSnake.Content[0, 0] as Snake).Direction;

                    SnakeBody.Add(new GameObject(posY, posX, new Snake[,] { { new Snake('#', 0xCC, Dir) } }));
   
                }


                // If out of map (Y)
                if (gameObject.PosY >= SizeY)
                    // Move on 0 coordinate
                    gameObject.Move(0, gameObject.PosX);

                // If out of map (Y)
                if (gameObject.PosY < 0)
                    // Move on window sizee coordinate
                    gameObject.Move(SizeY, gameObject.PosX);



                // If out of map (X)
                if (gameObject.PosX >= SizeX)
                    // Move on 0 coordinate
                    gameObject.Move(gameObject.PosY, 0);

                // If out of map (Y)
                if (gameObject.PosX < 0)
                    // Move on window sizee coordinate
                    gameObject.Move(gameObject.PosY, SizeX);
            }

            for (int i = SnakeBody.Count - 1; i > 0; i--)
            {

                (SnakeBody[i].Content[0, 0] as Snake).Direction = (SnakeBody[i - 1].Content[0, 0] as Snake).Direction;
                (SnakeBody[i].Content[0, 0] as Snake).setDir = (SnakeBody[i - 1].Content[0, 0] as Snake).Direction;

            }

            if(spawn)
            {
                spawn = false;
                (SnakeBody[SnakeBody.Count - 1].Content[0, 0] as Snake).Direction = Dir;
                SnakeBody[SnakeBody.Count - 1].Move(posY, posX);
            }

        }

    }

    public class Core
    {

        public Window window { get; private set; }

        public List<GameObject> Objects { get; private set; } = new List<GameObject>();

        public List<InputDelegate> InputSubscribers { get; private set; } = new List<InputDelegate>();

        public List<ConsoleKey> clickButtons = new List<ConsoleKey>();

        public GameObject Cursor = null;
        public GameObject BackGround;


        

        // Is objects crossing
        public bool IsCrossing(GameObject gameObject1, GameObject gameObject2)
        {
            if ((gameObject1.PosX + gameObject1.Content.GetLength(1) - 1) >= gameObject2.PosX && gameObject1.PosX <= (gameObject2.PosX + gameObject2.Content.GetLength(1) - 1))
            {
                if ((gameObject1.PosY + gameObject1.Content.GetLength(0) - 1) >= gameObject2.PosY && gameObject1.PosY <= (gameObject2.PosY + gameObject2.Content.GetLength(0) - 1))
                {
                    return true;
                }
            }
            return false;
        }

        public void ReadInput()
        {
            while (true)
            {

                ConsoleKey input = Console.ReadKey().Key;

                // If user press special button, which simulate click
                if (clickButtons.Contains(input) && Cursor != null)
                {
                    // Check crossing of objects
                    foreach (GameObject obj in Objects)
                    {
                        // If cursor cross object
                        if (IsCrossing(Cursor, obj))
                        {
                            // Call onclick object functions
                            foreach (GameObjectDelegate gameObjectDelegate in obj.OnClick)
                            {
                                if (gameObjectDelegate != null)
                                {
                                    gameObjectDelegate(obj);
                                }
                            }

                            // Call onclick cursor functions
                            foreach (GameObjectDelegate gameObjectDelegate in Cursor.OnClick)
                            {
                                if (gameObjectDelegate != null)
                                {
                                    gameObjectDelegate(obj);
                                }
                            }
                        }
                    }

                }

                foreach (InputDelegate subscriber in InputSubscribers)
                {
                    subscriber(input);
                }

            }
        }

        public void CreateWindow(int height, int width)
        {
            window = new Window(height, width);
        }

        public void CreateObject(GameObject gameObject)
        {
            Objects.Add(gameObject);
        }


        public void DrawContent()
        {
            while (true)
            {

                Program.SpawnFood(Program.Food);

                Program.MoveSnake();


                UpdateData();

                window.Draw();

                Thread.Sleep(66);

            }
        }

        /// <summary>
        /// Отрисовываем каждый объект, что есть в массиве с объектами.
        /// </summary>
        private void UpdateData()
        {
            for (int i = 0; i < window.SizeY; i++)
            {
                for (int j = 0; j < window.SizeX; j++)
                {
                    window.Content[i, j] = new Symbol(' ', 0xFF);
                }
            }

            foreach (GameObject obj in Objects)
            {


                if (obj.Active == false)
                {
                    continue;
                }

                // Если объект уходит в минус по координатам
                // То его мы его обрезаем (отрисовываем с нужного места)
                int drawFromY = 0;

                if (obj.PosY < 0)
                {
                    drawFromY = -obj.PosY;
                }

                int drawFromX = 0;

                if (obj.PosX < 0)
                {
                    drawFromX = -obj.PosX;
                }

                int posY = obj.PosY;
                int posX = obj.PosX;

                // Если объект ушел в минус по координатам
                // То мы обрезаем его часть, которая ушла в минус
                if (posY < 0)
                    posY = 0;
                if (posX < 0)
                    posX = 0;


                for (int i = 0; (drawFromY + i) < obj.Content.GetLength(0) && (posY + i) < window.SizeY; i++)
                {
                    for (int j = 0; (drawFromX + j) < obj.Content.GetLength(1) && (posX + j) < window.SizeX; j++)
                    {

                        if (obj.Content[drawFromY + i, drawFromX + j].Color == 0x52)
                        {
                            continue;
                        }

                        window.Content[i + posY, j + posX] = obj.Content[drawFromY + i, drawFromX + j];

                    }
                }

            }


        }

    }

}
