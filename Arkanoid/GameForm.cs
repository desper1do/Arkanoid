using System;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using System.IO;

public class GameForm : Form
{
    private GameModel game;
    private Timer timer;
    private SoundPlayer bounceSound;
    private SoundPlayer breakSound;
    private SoundPlayer gameOverSound;
    private const string CHEAT_CODE = "end";
    private string cheatBuffer = "";
    private DateTime lastKeyTime = DateTime.MinValue;
    private bool cheatActivated = false;
    public bool IsLevelCompleted { get; private set; }
    private readonly int _currentLevel;

    public GameForm(int level = 1)
    {
        this.Width = 500;
        this.Height = 500;
        this.Text = $"Арканоид - Уровень {level}";
        game = new GameModel(this.ClientSize.Width, this.ClientSize.Height, level);
        _currentLevel = level;

        timer = new Timer { Interval = 20 };
        timer.Tick += UpdateGame;
        timer.Start();

        this.Paint += DrawGame;
        this.KeyDown += OnKeyDown;
        this.DoubleBuffered = true;

        this.ClientSize = new Size(500, 500);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;

        game = new GameModel(this.ClientSize.Width, this.ClientSize.Height);

        // Загрузка звуков
        bounceSound = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "bounce.wav");
        breakSound = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "break.wav");
        gameOverSound = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "game_over.wav");
    }

    private void UpdateGame(object sender, EventArgs e)
    {
        game.CheckCollisions();

        if (game.IsGameOver || game.Lives == 0)
        {
            timer.Stop();
            gameOverSound.Play();
            MessageBox.Show("Игра окончена! Счет: " + game.Score);
            this.Close();
            return;
        }

        if (game.AreAllBlocksDestroyed())
        {
            IsLevelCompleted = true;
            timer.Stop();

            if (_currentLevel == 5)
            {
                MessageBox.Show($"Поздравляем! Вы полностью прошли игру!",
                              "Игра пройдена",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Уровень {_currentLevel} пройден!",
                              "Успех",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }

            this.Close();
        }

        game.Ball.Move();

        if (game.Ball.Bounds.IntersectsWith(game.Paddle.Bounds))
        {
            bounceSound.Play();
            game.Ball.Bounce();
        }

        Invalidate();
    }


    private void DrawGame(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.FillRectangle(Brushes.Blue, game.Paddle.Bounds);
        g.FillEllipse(Brushes.Red, game.Ball.Bounds);

        foreach (var block in game.Blocks)
        {
            if (!block.IsDestroyed)
            {
                g.FillRectangle(new SolidBrush(block.Color), block.Bounds);
            }
        }

        g.DrawString($"Счет: {game.Score}", new Font("Arial", 12), Brushes.Black, 10, 10);
        g.DrawString($"Жизни: {game.Lives}", new Font("Arial", 12), Brushes.Black, 10, 30);

        if (cheatActivated)
        {
            e.Graphics.DrawString("CHEAT ACTIVATED!",
                                new Font("Arial", 16),
                                Brushes.Gold,
                                this.Width / 2 - 100,
                                this.Height / 2 - 50);
        }
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left)
        {
            game.Paddle.Move(-1);
        }
        else if (e.KeyCode == Keys.Right)
        {
            game.Paddle.Move(1);
        }
        else if (e.KeyCode == Keys.R && game.IsGameOver)
        {
            game = new GameModel(this.Width, this.Height);
            timer.Start();
            cheatActivated = false;
        }

        var now = DateTime.Now;
        if ((now - lastKeyTime).TotalSeconds > 1)
        {
            cheatBuffer = "";
        }
        lastKeyTime = now;

        if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
        {
            cheatBuffer += e.KeyCode.ToString().ToLower();
        }

        if (cheatBuffer.Length > CHEAT_CODE.Length)
        {
            cheatBuffer = cheatBuffer.Substring(cheatBuffer.Length - CHEAT_CODE.Length);
        }

        if (cheatBuffer == CHEAT_CODE && !cheatActivated)
        {
            cheatActivated = true;
            game.ActivateCheat();
            Invalidate();
        }
    }
}
