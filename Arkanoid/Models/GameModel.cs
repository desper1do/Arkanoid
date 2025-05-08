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
        public bool WasBlockDestroyed { get; set; }
        public int formWidth;
        public int formHeight;
        public event Action LifeLost;
        private bool _isEndlessMode = false;
        private int maxPossibleScore;

        public GameModel(int formWidth, int formHeight, int level = 1, bool isEndless = false)
        {
            this.formWidth = formWidth;
            this.formHeight = formHeight;
            _isEndlessMode = isEndless;
            Level = isEndless ? 1 : Math.Max(1, Math.Min(level, 5));

            Paddle = new Paddle(formWidth);
            Ball = new Ball(formWidth, formHeight);
            Blocks = new List<Block>();
            Score = 0;
            Lives = 3;
            IsGameOver = false;

            if (_isEndlessMode)
                GenerateRandomBlocks();
            else
                InitializeBlocks(Level);

            maxPossibleScore = Blocks.Count * 10;
        }

        public void Reset(int formWidth, int formHeight, int level = 1, bool isEndless = false)
        {
            this.formWidth = formWidth;
            this.formHeight = formHeight;
            _isEndlessMode = isEndless;

            if (!_isEndlessMode)
                Level = Math.Max(1, Math.Min(level, 5));

            Paddle = new Paddle(formWidth);
            Ball = new Ball(formWidth, formHeight);
            Blocks = new List<Block>();
            Score = 0;
            Lives = 3;
            IsGameOver = false;

            if (_isEndlessMode)
                GenerateRandomBlocks();
            else
                InitializeBlocks(Level);
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
            int startX = (formWidth - totalWidth) / 2;
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
                            case 1: color = Color.Cyan; break;
                            case 2: color = Color.Magenta; break;
                            case 3: color = Color.Yellow; break;
                            case 4: color = Color.Red; break;
                            case 5: color = Color.Blue; break;
                            default: color = Color.Green; break;
                        }
                        Blocks.Add(new Block(x, y, blockWidth, blockHeight, color));
                    }
                }
            }
        }

        public void GenerateRandomBlocks()
        {
            Blocks.Clear();
            Random rand = new Random();
            int blockWidth = 50;
            int blockHeight = 20;
            int maxRows = 8;
            int maxColumns = 8;
            int totalBlocks = 24;
            int margin = 10;

            var blockColors = new List<Color>
            {
                Color.Cyan,       // Циан
                Color.Magenta,    // Пурпурный
                Color.Yellow,     // Желтый
                Color.Red,        // Красный
                Color.Blue,       // Синий
                Color.Green       // Зеленый
            };

            var allPositions = new List<Tuple<int, int>>();

            for (int row = 0; row < maxRows; row++)
            {
                for (int col = 0; col < maxColumns; col++)
                {
                    allPositions.Add(new Tuple<int, int>(row, col));
                }
            }

            allPositions = allPositions.OrderBy(x => rand.Next()).ToList();

            int maxCols = maxColumns;
            int totalWidth = maxCols * (blockWidth + margin) - margin;
            int startX = (formWidth - totalWidth) / 2;
            int startY = 50;

            for (int i = 0; i < totalBlocks; i++)
            {
                var position = allPositions[i];
                int row = position.Item1;
                int col = position.Item2;

                int x = startX + col * (blockWidth + margin);
                int y = startY + row * (blockHeight + margin);

                if (Blocks.Any(b => b.Bounds.IntersectsWith(new Rectangle(x, y, blockWidth, blockHeight))))
                {
                    continue;
                }

                Color randomColor = blockColors[rand.Next(blockColors.Count)];

                var block = new Block(x, y, blockWidth, blockHeight, randomColor);

                Blocks.Add(block);
            }

            Ball.SetVelocity(0, 0);
            Ball.SetPosition(
                Paddle.Bounds.X + Paddle.Bounds.Width / 2 - Ball.Bounds.Width / 2,
                Paddle.Bounds.Y - Ball.Bounds.Height
            );

            WasBlockDestroyed = false;
            maxPossibleScore = Blocks.Count * 10;
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

            if (Ball.Bounds.Bottom >= formHeight)
            {
                Lives--;
                WasBlockDestroyed = false;

                if (Lives <= 0)
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
        public bool AreAllBlocksDestroyed() => Blocks.All(b => b.IsDestroyed);

        public void ActivateCheat()
        {
            foreach (var block in Blocks)
            {
                if (!block.IsDestroyed)
                {
                    block.IsDestroyed = true;
                }
            }

            if (!_isEndlessMode)
            {
                Score = maxPossibleScore;
            }
        }


        public void DestroyAllBlocks()
        {
            foreach (var block in Blocks)
            {
                block.IsDestroyed = true;
            }
        }
    }
}
