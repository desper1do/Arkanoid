using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public class GameModel
{
    public Paddle Paddle { get; private set; }
    public Ball Ball { get; private set; }
    public List<Block> Blocks { get; private set; }
    public int Score { get; private set; }
    public int Lives { get; private set; }
    public bool IsGameOver { get; private set; }
    private readonly int maxPossibleScore;
    private bool cheatUsed = false;

    public GameModel(int formWidth, int formHeight, int level = 1)
    {
        level = Math.Max(1, Math.Min(level, 5));

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
        int rows = 3 + (level - 1);
        int cols = 8;
        int margin = 10;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int x = col * (blockWidth + margin) + margin;
                int y = row * (blockHeight + margin) + margin;
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

    public void CheckCollisions()
    {
        foreach (var block in Blocks.ToList())
        {
            if (!block.IsDestroyed && Ball.Bounds.IntersectsWith(block.Bounds))
            {
                block.IsDestroyed = true;
                Ball.Bounce();
                Score += 10;
                System.Media.SoundPlayer breakSound = new System.Media.SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "break.wav");
                breakSound.Play();
            }
        }

        if (Ball.Bounds.Bottom >= Paddle.Bounds.Top + Paddle.Bounds.Height)
        {
            Lives--;
            if (Lives < 0) Lives = 0;

            if (Lives == 0)
            {
                IsGameOver = true;
                return;
            }

            Ball = new Ball(Ball.formWidth, Ball.formHeight);
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