using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Lab17
{
    public delegate void KeyHandler(object sender, KeyMaster e);
    internal class Program
    {
        public static System.Windows.Interop.RenderMode ProcessRenderMode { get; set; }
        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.Unicode;
                //ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
                Game g = new Game(40,100);
                g.Settings(5);
                g.Run();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadKey();
        }
    }
    class Game
    {
        private Random r = new Random();
        public event KeyHandler PressedButton = (sender, e) =>
        {
            Game game = sender as Game;
            if (game.CheckCollision(e.player, e.direction))
            {
                game.ClearPlayerPosition(e.player);
                e.player.Move(e.direction);
            }
        };
        public static int ordinRow;
        public static int ordinCol;
        public Wolf wolf;
        public Hunter hunter;
        public Coin coin;
        public Coin coin2;
        private int Score;
        private int Health;
        private bool GameOver;
        private Field field;
        public Game(int width, int height)
        {
            field = new Field(width, height);
            wolf = new Wolf();
            GeneratePlayerPosition(wolf);
            hunter = new Hunter();
            GeneratePlayerPosition(hunter);
            GameOver = false;
            ordinCol = Console.CursorLeft + 1;
            ordinRow = Console.CursorTop + 1;
            Score = 100;
            Health = 1;
        }
        private void ScoreBar()
        {
            WriteAt("————Wolf Score Bar————", field.Height + 4, 0);
            WriteAt($"| Score: {wolf.Score:D5}/{Score:D5} |", field.Height + 4, 1);
            WriteAt($"| HP:        {wolf.HP:D3}/{Health:D3} |", field.Height + 4, 2);
            WriteAt("——————————————————————", field.Height + 4, 3);
            Console.SetCursorPosition(0, field.Width + 2);
        }
        /*async Task WaitClick()
        {
            await Task.Run(() =>
            {
                ConsoleKey key = Console.ReadKey().Key;
                switch (key)
                {
                    case ConsoleKey.W:
                        if (PressedButton != null)
                            PressedButton(this, new KeyMaster(hunter, field, Direction.Up));
                        break;
                    case ConsoleKey.S:
                        if (PressedButton != null)
                            PressedButton(this, new KeyMaster(hunter, field, Direction.Down));
                        break;
                    case ConsoleKey.D:
                        if (PressedButton != null)
                            PressedButton(this, new KeyMaster(hunter, field, Direction.Right));
                        break;
                    case ConsoleKey.A:
                        if (PressedButton != null)
                            PressedButton(this, new KeyMaster(hunter, field, Direction.Left));
                        break;
                    case ConsoleKey.UpArrow:
                        if (PressedButton != null)
                            PressedButton(this, new KeyMaster(wolf, field, Direction.Up));
                        break;
                    case ConsoleKey.DownArrow:
                        if (PressedButton != null)
                            PressedButton(this, new KeyMaster(wolf, field, Direction.Down));
                        break;
                    case ConsoleKey.RightArrow:
                        if (PressedButton != null)
                            PressedButton(this, new KeyMaster(wolf, field, Direction.Right));
                        break;
                    case ConsoleKey.LeftArrow:
                        if (PressedButton != null)
                            PressedButton(this, new KeyMaster(wolf, field, Direction.Left));
                        break;
                    case ConsoleKey.Escape:
                        GameOver = true;
                        break;
                    case ConsoleKey.Tab:
                        Console.Clear();
                        field.Display();
                        break;
                    default:
                        break;
                }
                UpdatePlayersPosition();
                if (coin.Position == wolf.Position)
                {
                    wolf.Score += 10;
                    Console.WriteLine(wolf.Score);
                    GenereateCoin();
                }
            });
        }*/
        public void Settings(int WolfHealth = 1, int FinishScore = 100)
        {
            wolf.HP = WolfHealth;
            Health = WolfHealth;
            if (FinishScore % 10 == 0)
            {
                Score = FinishScore;
            }
        }
        public void WaitClick()
        {
            ConsoleKey key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.W:
                    if (PressedButton != null)
                        PressedButton(this, new KeyMaster(hunter, field, Direction.Up));
                    break;
                case ConsoleKey.S:
                    if (PressedButton != null)
                        PressedButton(this, new KeyMaster(hunter, field, Direction.Down));
                    break;
                case ConsoleKey.D:
                    if (PressedButton != null)
                        PressedButton(this, new KeyMaster(hunter, field, Direction.Right));
                    break;
                case ConsoleKey.A:
                    if (PressedButton != null)
                        PressedButton(this, new KeyMaster(hunter, field, Direction.Left));
                    break;
                case ConsoleKey.UpArrow:
                    if (PressedButton != null)
                        PressedButton(this, new KeyMaster(wolf, field, Direction.Up));
                    break;
                case ConsoleKey.DownArrow:
                    if (PressedButton != null)
                        PressedButton(this, new KeyMaster(wolf, field, Direction.Down));
                    break;
                case ConsoleKey.RightArrow:
                    if (PressedButton != null)
                        PressedButton(this, new KeyMaster(wolf, field, Direction.Right));
                    break;
                case ConsoleKey.LeftArrow:
                    if (PressedButton != null)
                        PressedButton(this, new KeyMaster(wolf, field, Direction.Left));
                    break;
                case ConsoleKey.Escape:
                    GameOver = true;
                    break;
                case ConsoleKey.Tab:
                    Console.Clear();
                    field.Display();
                    ScoreBar();
                    break;
                default:
                    break;
            }
            UpdatePlayersPosition();
            if (coin.Position == wolf.Position)
            {
                wolf.Score += 10;
                ScoreBar();
                GenereateCoin();
            }
            if (coin2.Position == wolf.Position)
            {
                wolf.Score += 10;
                ScoreBar();
                GenereateCoin2();
            }
        }
        public void Run()
        {
            Console.WindowHeight = Console.LargestWindowHeight;
            Console.WindowWidth = Console.LargestWindowWidth;
            field.Display();
            ScoreBar();
            GenereateCoin();
            GenereateCoin2();
            while (!GameOver)
            {
                if (wolf.Position == hunter.Position)
                {
                    wolf.HP--;
                    ScoreBar();
                }
                if (wolf.Score == 100)
                    EOG.END(1);
                if(wolf.HP == 0)
                    EOG.END(0);
                WaitClick();
            }
        }
        public bool CheckCollision(Player p, Direction d)
        {
            Player pl = new Player(p);
            switch (d)
            {
                case Direction.Left:
                    pl.Move(Direction.Left);
                    break;
                case Direction.Right:
                    pl.Move(Direction.Right);
                    break;
                case Direction.Up:
                    pl.Move(Direction.Up);
                    break;
                case Direction.Down:
                    pl.Move(Direction.Down);
                    break;
            }
            if (pl.Position.Item1 >= field.Width | pl.Position.Item2 >= field.Height | pl.Position.Item1 < 0 | pl.Position.Item2 < 0)
                return false;
            if (field[pl.Position.Item1, pl.Position.Item2] != "■")
                return true;
            else
                return false;
        }
        private void UpdatePlayersPosition()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteAt(coin.Symbol, coin.Position.Item2, coin.Position.Item1);
            Console.SetCursorPosition(0, field.Width + 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteAt(coin2.Symbol, coin2.Position.Item2, coin2.Position.Item1);
            Console.SetCursorPosition(0, field.Width + 2);
            field[wolf.Position.Item1, wolf.Position.Item2] = wolf.Symbol;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(0, field.Width + 2);
            WriteAt(wolf.Symbol, wolf.Position.Item2, wolf.Position.Item1);
            Console.SetCursorPosition(0, field.Width + 2);
            field[hunter.Position.Item1, hunter.Position.Item2] = hunter.Symbol;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(0, field.Width + 2);
            WriteAt(hunter.Symbol, hunter.Position.Item2, hunter.Position.Item1);
            Console.SetCursorPosition(0, field.Width + 2);
            Console.ForegroundColor= ConsoleColor.White;
        }
        private void GeneratePlayerPosition(Player player)
        {
            int i = r.Next(0, field.Width), j = r.Next(0, field.Height);
            while (field[i, j] == "■")
            {
                i = r.Next(0, field.Width);
                j = r.Next(0, field.Height);
            }
            player.Position = (i, j);
            field[i, j] = player.Symbol;
        }
        public static void WriteAt(String s, int x, int y)
        {
            Console.SetCursorPosition(ordinCol + x, ordinRow + y);
            Console.Write(s);
        }
        public void ClearPlayerPosition(Player player)
        {
            field[player.Position.Item1, player.Position.Item2] = " ";
            WriteAt(" ", player.Position.Item2, player.Position.Item1);
        }
        public void GenereateCoin()
        {
            coin = new Coin();
            int i = r.Next(0, field.Width), j = r.Next(0, field.Height);
            while (field[i, j] == "■")
            {
                i = r.Next(0, field.Width);
                j = r.Next(0, field.Height);
            }
            coin.Position = (i, j);
            field[i, j] = coin.Symbol;
        }
        public void GenereateCoin2()
        {
            coin2 = new Coin();
            int i = r.Next(0, field.Width), j = r.Next(0, field.Height);
            while (field[i, j] == "■")
            {
                i = r.Next(0, field.Width);
                j = r.Next(0, field.Height);
            }
            coin2.Position = (i, j);
            field[i, j] = coin2.Symbol;
        }
    }
    public class KeyMaster
    {
        public Player player { get; }
        public Field field { get; }
        public Direction direction { get; }
        public KeyMaster(Player player, Field field, Direction direction)
        {
            this.player = player;
            this.field = field;
            this.direction = direction;
        }
    }
    public class Field
    {
        private int width;
        public int Width { get => width; }
        private int height;
        public int Height { get => height; }
        private string[,] margin;
        public string this[int i, int j]
        {
            get => margin[i, j];
            set => margin[i, j] = value;
        }
        public Field(int width, int height)
        {
            if (width < 4)
                throw new ArgumentException("The Width of field must be bigger than 4");
            if (height < 4)
                throw new ArgumentException("The Width of field must be bigger than 4");
            this.width = width;
            this.height = height;
            margin = new string[width, height];
            generateMargin();
        }
        public Field()
        {
            this.width = 5;
            this.height = 5;
            margin = new string[width, height];
            generateMargin();
        }
        public void generateMargin()
        {
            Random r = new Random();
            string[] strings = { "■", " " };
            int[] weights = { 1, 6 };
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    margin[i, j] = RandNums.getNextVal(strings, weights);
        }
        public void Display()
        {
            for (int j = 0; j < height + 2; j++)
                Console.Write("—");
            Console.Write('\n');
            for (int i = 0; i < width; i++)
            {
                Console.Write("|");
                for (int j = 0; j < height; j++)
                {
                    if (margin[i, j] == "W")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(margin[i, j]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if(margin[i, j] == "H")
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(margin[i, j]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                        Console.Write(margin[i, j]);
                }
                Console.Write("|");
                Console.Write('\n');
            }
            for (int j = 0; j < height + 2; j++)
                Console.Write("—");
            for(int i=0;i<Console.WindowHeight-height-1;i++)
            {
                Console.WriteLine();
            }
            Console.Write('\n');
        }
    }
    public class Player
    {
        private (int, int) position;
        public string Symbol;
        public (int, int) Position { get { return position; } set { position = value; } }
        public Player(Player p)
        {
            Symbol = p.Symbol;
            position = p.Position;
        }
        public Player()
        {

        }
        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    position.Item2--;
                    break;
                case Direction.Right:
                    position.Item2++;
                    break;
                case Direction.Up:
                    position.Item1--;
                    break;
                case Direction.Down:
                    position.Item1++;
                    break;
            }
        }
    }
    public class Wolf : Player
    {
        public int Score { get; set; }
        public int HP { get; set; }
        public Wolf() : base()
        {
            Symbol = "W";
            Score = 0;
            HP = 1;
        }
        public bool Exist()
        {
            if (this == null)
                return false;
            else
                return true;
        }
    }
    public class Coin
    {
        public String Symbol;
        public (int,int) Position { get; set; }
        public Coin()
        {
            Symbol = "⑩";
        }
    }
    public class Hunter : Player
    {
        public Hunter() : base()
        {
            Symbol = "H";
        }
    }
    public static class RandNums
    {
        private static Random random = new Random();
        public static String getNextVal(String[] vals, int[] weights)
        {
            int rndNu = Math.Abs(random.Next());
            int rangUnit = int.MaxValue / weights.Max(x => x);
            for (int i = 0; i < weights.Length; i++)
                if (rndNu <= rangUnit * weights[i])
                    return vals[i];
            return "\0";
        }
    }
    public static class EOG
    {
        public static void END(int scenario)
        {
            switch(scenario)
            {
                case 0:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("The END!\nHunter win!");
                    Console.ReadLine();
                    Environment.Exit(0);
                    break;
                case 1:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The END!\nWolf win!");
                    Console.ReadLine();
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Undetected scenarion!");
                    Console.ReadLine();
                    Environment.Exit(0);
                    break;
            }
        }
    }
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }
}
