using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Arkanoid.Models
{
    public class GameModel
    {
        public Paddle Paddle { get; private set; }
        public Ball Ball { get; private set; }
        public List<Block> Blocks { get; private set; }
        public int Level { get; private set; }
        public int Score { get; private set; }
        public int Lives { get; private set; }
        public bool IsGameOver { get; private set; }
        private readonly int maxPossibleScore;
        private bool cheatUsed = false;
        public bool WasBlockDestroyed { get; set; }
        public int formWidth;
        public int formHeight;
        public int FormWidth;
        public int FormHeight;
        public event Action LifeLost;

        public GameModel(int formWidth, int formHeight, int level = 1)
        {
            Level = Math.Max(1, Math.Min(level, 5));
            Reset(formWidth, formHeight, Level);


            Paddle = new Paddle(formWidth);
            Ball = new Ball(formWidth, formHeight);
            Blocks = new List<Block>();
            Score = 0;
            Lives = 3;
            IsGameOver = false;

            InitializeBlocks(level);
            maxPossibleScore = Blocks.Count * 10;
        }

        private void InitializeBlocks(int level)
        {
            int blockWidth = 50;
            int blockHeight = 20;
            int margin = 10;

            string[] levelPatterns = new string[]
            {
        // Уровень 1
        "········\n" +
        "··■■■■··\n" +
        "·■■■■■■·\n" +
        "■■■■■■■■\n" +
        "········\n" +
        "········",
        
        // Уровень 2
        "··■■■■··\n" +
        "··■■■■··\n" +
        "■■····■■\n" +
        "■■····■■\n" +
        "··■■■■··\n" +
        "··■■■■··",
        
        // Уровень 3
        "■·■·■·■·\n" +
        "·■·■·■·■\n" +
        "■·■·■·■·\n" +
        "·■·■·■·■\n" +
        "■·■·■·■·\n" +
        "·■·■·■·■",
        
        // Уровень 4
        "·■■··■■·\n" +
        "■■■■■■■■\n" +
        "■■····■■\n" +
        "·■■··■■·\n" +
        "··■■■■··\n" +
        "···■■···",
        
        // Уровень 5
        "■·■··■·■\n" +
        "■·■··■·■\n" +
        "■■■■■■■■\n" +
        "■■■■■■■■\n" +
        "■·■··■·■\n" +
        "■·■··■·■"
            };

            string pattern = levelPatterns[level - 1];
            string[] rows = pattern.Split('\n');

            int maxCols = rows.Max(r => r.Length);
            int totalWidth = maxCols * (blockWidth + margin) - margin;
            int startX = formWidth + 15;
            int startY = 50;

            for (int row = 0; row < rows.Length; row++)
            {
                for (int col = 0; col < rows[row].Length; col++)
                {
                    if (rows[row][col] == '■')
                    {
                        int x = startX + col * (blockWidth + margin);
                        int y = startY + row * (blockHeight + margin);
                        Color color;
                        switch (level)
                        {
                            case 1: color = Color.Green; break;
                            case 2: color = Color.Blue; break;
                            case 3: color = Color.Yellow; break;
                            case 4: color = Color.Orange; break;
                            case 5: color = Color.Red; break;
                            default: color = Color.Green; break;
                        }
                        Blocks.Add(new Block(x, y, blockWidth, blockHeight, color));
                    }
                }
            }
        }

        public void Reset(int formWidth, int formHeight, int level = 1)
        {
            Level = Math.Max(1, Math.Min(level, 5));

            Paddle = new Paddle(formWidth);
            Ball = new Ball(formWidth, formHeight);
            Blocks = new List<Block>();
            Score = 0;
            Lives = 3;
            IsGameOver = false;
            cheatUsed = false;

            InitializeBlocks(level);
        }

        public void CheckCollisions()
        {
            foreach (var block in Blocks.ToList())
            {
                if (!block.IsDestroyed && Ball.Bounds.IntersectsWith(block.Bounds))
                {
                    Ball.BounceFromBlock(block.Bounds);
                    block.IsDestroyed = true;
                    Score += 10;
                    WasBlockDestroyed = true;
                }
            }

            if (Ball.Bounds.IntersectsWith(Paddle.Bounds))
            {
                Ball.BounceFromPaddle(Paddle.Bounds);
                WasBlockDestroyed = false;
            }

            if (Ball.Bounds.Bottom >= Paddle.Bounds.Top + Paddle.Bounds.Height)
            {
                Lives--;
                WasBlockDestroyed = false;

                if (Lives == 0)
                {
                    IsGameOver = true;
                    return;
                }

                Ball.SetVelocity(0, 0);
                Ball.SetPosition(
                    Paddle.Bounds.X + Paddle.Bounds.Width / 2 - Ball.Bounds.Width / 2,
                    Paddle.Bounds.Y - Ball.Bounds.Height
                );
                LifeLost?.Invoke();
            }
        }


        public bool AreAllBlocksDestroyed()
        {
            return Blocks.All(block => block.IsDestroyed);
        }

        public void DestroyAllBlocks()
        {
            foreach (var block in Blocks)
            {
                block.IsDestroyed = true;
            }
            Score += Blocks.Count * 10;
        }

        public void Update()
        {
            Ball.Move();
            CheckCollisions();
        }
        public void ActivateCheat()
        {
            if (!cheatUsed)
            {
                cheatUsed = true;
                Score = maxPossibleScore;
                foreach (var block in Blocks)
                {
                    block.IsDestroyed = true;
                }
            }
        }
    }
}
