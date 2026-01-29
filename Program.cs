using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Linq;

namespace GradiusElite_Project
{
    public enum GameState { MainMenu, Playing, GameOver, HighScores }

    // --- BASE ENTITY ---
    public abstract class Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Sprite { get; protected set; }
        public ConsoleColor Color { get; protected set; }
        public abstract void Move();
    }

    // --- NEW: BONUS CLASS (Inheritance) ---
    public class Bonus : Entity
    {
        public int PointValue { get; private set; }
        public Bonus(int x, int y)
        {
            X = x; Y = y;
            Sprite = "($)"; // Represents a coin or power-up
            Color = ConsoleColor.Magenta;
            PointValue = 150; // High value reward
        }
        public override void Move() => X--; // Moves across screen like enemies
    }

    public class Player : Entity
    {
        public int Lives { get; set; } = 3;
        public Player(int x, int y) { X = x; Y = y; Sprite = "<#=O>"; Color = ConsoleColor.Cyan; }
        public override void Move() { }
    }

    public class Enemy : Entity
    {
        public Enemy(int x, int y) { X = x; Y = y; Sprite = "[X]"; Color = ConsoleColor.Red; }
        public override void Move() => X--;
    }

    public class Bullet : Entity
    {
        public Bullet(int x, int y) { X = x; Y = y; Sprite = "»"; Color = ConsoleColor.Yellow; }
        public override void Move() => X += 2;
    }

    class Program
    {
        const int WIDTH = 90;
        const int HEIGHT = 25;
        static GameState currentState = GameState.MainMenu;
        static Player player;
        static List<Enemy> enemies = new();
        static List<Bullet> bullets = new();
        static List<Bonus> bonuses = new(); // NEW: Bonus Collection
        static int score = 0;
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            Console.Title = "Gradius Elite: FYP Edition";
            Console.CursorVisible = false;
            while (true)
            {
                switch (currentState)
                {
                    case GameState.MainMenu: DrawMenu(); break;
                    case GameState.Playing: RunGameLoop(); break;
                    case GameState.GameOver: DrawGameOver(); break;
                }
            }
        }

        static void RunGameLoop()
        {
            HandleInput();
            UpdateLogic();
            Render();
            Thread.Sleep(50);
        }

        static void HandleInput()
        {
            if (!Console.KeyAvailable) return;
            var key = Console.ReadKey(true).Key;
            if ((key == ConsoleKey.UpArrow || key == ConsoleKey.W) && player.Y > 1) player.Y--;
            if ((key == ConsoleKey.DownArrow || key == ConsoleKey.S) && player.Y < HEIGHT - 2) player.Y++;
            if (key == ConsoleKey.Spacebar) bullets.Add(new Bullet(player.X + 4, player.Y));
        }

        static void UpdateLogic()
        {
            // Update Bullets
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Move();
                if (bullets[i].X > WIDTH - 2) bullets.RemoveAt(i);
            }

            // Spawn Logic: 10% chance for Enemy, 2% chance for Bonus
            if (rnd.Next(0, 100) < 10) enemies.Add(new Enemy(WIDTH - 5, rnd.Next(1, HEIGHT - 1)));
            if (rnd.Next(0, 100) < 2) bonuses.Add(new Bonus(WIDTH - 5, rnd.Next(1, HEIGHT - 1)));

            // Update Bonuses & Collection
            for (int i = bonuses.Count - 1; i >= 0; i--)
            {
                bonuses[i].Move();
                if (Math.Abs(bonuses[i].X - player.X) < 3 && bonuses[i].Y == player.Y)
                {
                    score += bonuses[i].PointValue; // COLLECTED!
                    bonuses.RemoveAt(i);
                }
                else if (bonuses[i].X < 1) bonuses.RemoveAt(i);
            }

            // Update Enemies & Bullet Collisions
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Move();
                if (enemies[i].Y == player.Y && Math.Abs(enemies[i].X - player.X) < 4)
                {
                    player.Lives--;
                    enemies.RemoveAt(i);
                    if (player.Lives <= 0) currentState = GameState.GameOver;
                    continue;
                }

                for (int j = bullets.Count - 1; j >= 0; j--)
                {
                    if (bullets[j].Y == enemies[i].Y && Math.Abs(bullets[j].X - enemies[i].X) < 2)
                    {
                        score += 50;
                        enemies.RemoveAt(i);
                        bullets.RemoveAt(j);
                        break;
                    }
                }
                if (i < enemies.Count && enemies[i].X < 1) enemies.RemoveAt(i);
            }
        }

        static void Render()
        {
            char[,] screen = new char[HEIGHT, WIDTH];
            for (int y = 0; y < HEIGHT; y++)
                for (int x = 0; x < WIDTH; x++) screen[y, x] = ' ';

            // Place Entities
            foreach (var b in bullets) if (b.X < WIDTH) screen[b.Y, b.X] = '>';
            foreach (var e in enemies) if (e.X < WIDTH && e.X > 0) screen[e.Y, e.X] = 'X';
            foreach (var bn in bonuses) if (bn.X < WIDTH && bn.X > 0) screen[bn.Y, bn.X] = '$';

            for (int i = 0; i < player.Sprite.Length; i++)
                if (player.X + i < WIDTH) screen[player.Y, player.X + i] = player.Sprite[i];

            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++) sb.Append(screen[y, x]);
                sb.Append('\n');
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(sb.ToString());
            Console.WriteLine($" SCORE: {score.ToString("D5")} | LIVES: {player.Lives} | [ BONUS COINS ACTIVE ]");
        }

        static void DrawMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\n\t--- GRADIUS ELITE PROJECT ---");
            Console.WriteLine("\t1. START MISSION\n\t2. EXIT");
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.D1) { player = new Player(5, HEIGHT / 2); enemies.Clear(); bullets.Clear(); bonuses.Clear(); score = 0; currentState = GameState.Playing; }
            if (key == ConsoleKey.D2) Environment.Exit(0);
        }

        static void DrawGameOver()
        {
            Console.Clear();
            Console.WriteLine("\n\t--- GAME OVER ---");
            Console.WriteLine($"\tFINAL SCORE: {score}");
            Console.ReadKey();
            currentState = GameState.MainMenu;
        }
    }
}