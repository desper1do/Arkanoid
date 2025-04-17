using System;
using System.Drawing;
using System.Windows.Forms;
using Arkanoid.Models;

namespace Arkanoid.Views
{
    public class GameView : Form
    {
        private static readonly Color BACKGROUND_COLOR = Color.White;
        private static readonly Color PADDLE_COLOR = Color.Blue;
        private static readonly Color BALL_COLOR = Color.Red;
        private static readonly Color TEXT_COLOR = Color.Black;

        public event Action<int> PaddleMoveRequested;
        public event Action<string> CheatCodeEntered;
        public event Action RestartRequested;

        private bool _cheatActivated;
        private GameModel _model;
        private bool _isPaused;

        public GameView(int level)
        {
            ClientSize = new Size(500, 500);
            Text = $"Арканоид - Уровень {level}";
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            BackColor = BACKGROUND_COLOR;
            KeyPreview = true;
            KeyDown += OnKeyDown;
        }

        public void SetModel(GameModel model) => _model = model;

        public void SetPaused(bool paused)
        {
            _isPaused = paused;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_model == null) return;

            e.Graphics.FillRectangle(new SolidBrush(PADDLE_COLOR), _model.Paddle.Bounds);

            e.Graphics.FillEllipse(new SolidBrush(BALL_COLOR), _model.Ball.Bounds);

            foreach (var block in _model.Blocks)
            {
                if (!block.IsDestroyed)
                {
                    e.Graphics.FillRectangle(new SolidBrush(block.Color), block.Bounds);
                }
            }

            var font = new Font("Arial", 12);
            e.Graphics.DrawString($"Счет: {_model.Score}", font, new SolidBrush(TEXT_COLOR), 10, 10);
            e.Graphics.DrawString($"Жизни: {_model.Lives}", font, new SolidBrush(TEXT_COLOR), 10, 30);

            if (_cheatActivated)
            {
                e.Graphics.DrawString("CHEAT ACTIVATED!",
                    new Font("Arial", 16),
                    Brushes.Gold,
                    Width / 2 - 100,
                    Height / 2 - 50);
            }

            if (_isPaused)
            {
                var pauseFont = new Font("Arial", 24, FontStyle.Bold);
                var pauseText = "ПАУЗА";
                var textSize = e.Graphics.MeasureString(pauseText, pauseFont);
                e.Graphics.DrawString(pauseText,
                    pauseFont,
                    Brushes.Red,
                    Width / 2 - textSize.Width / 2,
                    Height / 2 - textSize.Height / 2);
            }
        }

        public void SetCheatActivated(bool activated)
        {
            _cheatActivated = activated;
            Invalidate();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left: PaddleMoveRequested?.Invoke(-1); break;
                case Keys.Right: PaddleMoveRequested?.Invoke(1); break;
                case Keys.R: RestartRequested?.Invoke(); break;
                default:
                    if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
                        CheatCodeEntered?.Invoke(e.KeyCode.ToString().ToLower());
                    break;
            }
        }
    }
}
