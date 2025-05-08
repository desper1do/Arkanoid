using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using Arkanoid.Models;

namespace Arkanoid.Views
{
    public class GameView : Form
    {
        private static readonly Color BACKGROUND_COLOR = Color.Black;
        private static readonly Color PADDLE_COLOR = Color.Lime;
        private static readonly Color BALL_COLOR = Color.White;
        private static readonly Color TEXT_COLOR = Color.White;

        public event Action<int> PaddleMoveRequested;
        public event Action<string> CheatCodeEntered;
        public event Action RestartRequested;
        public event Action ViewClosedExternally;

        private bool _cheatActivated;
        private GameModel _model;
        private bool _isPaused;

        private PrivateFontCollection _fonts;
        private Font _mainFont;
        private Font _largeFont;

        public GameView(int level)
        {
            ClientSize = new Size(500, 500);
            if (level > 0)
                Text = $"ARKANOID - LEVEL {level}";
            else
                Text = $"ARKANOID - ENDLESS";

            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(100, 100);
            MaximizeBox = false;
            this.ControlBox = false;
            BackColor = BACKGROUND_COLOR;
            KeyPreview = true;
            KeyDown += OnKeyDown;

            LoadFonts();
            AddCloseButton();
        }

        private void LoadFonts()
        {
            _fonts = new PrivateFontCollection();
            _fonts.AddFontFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Fonts", "PressStart2P-Regular.ttf"));
            _mainFont = new Font(_fonts.Families[0], 8);
            _largeFont = new Font(_fonts.Families[0], 16, FontStyle.Bold);
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

            e.Graphics.Clear(BACKGROUND_COLOR);

            e.Graphics.FillRectangle(new SolidBrush(PADDLE_COLOR), _model.Paddle.Bounds);
            e.Graphics.FillRectangle(new SolidBrush(BALL_COLOR), _model.Ball.Bounds);

            foreach (var block in _model.Blocks)
            {
                if (!block.IsDestroyed)
                {
                    e.Graphics.FillRectangle(new SolidBrush(block.Color), block.Bounds);
                }
            }

            e.Graphics.DrawString($"SCORE: {_model.Score}", _mainFont, new SolidBrush(TEXT_COLOR), 10, 10);
            e.Graphics.DrawString($"LIVES: {_model.Lives}", _mainFont, new SolidBrush(TEXT_COLOR), 10, 30);

            if (_cheatActivated)
            {
                var cheatText = "CHEAT ACTIVATED!";
                var size = e.Graphics.MeasureString(cheatText, _mainFont);
                e.Graphics.DrawString(cheatText,
                    _mainFont,
                    Brushes.Gold,
                    Width / 2 - 80,
                    Height / 2 - 50);
            }

            if (_isPaused)
            {
                var pauseText = "PAUSE";
                var size = e.Graphics.MeasureString(pauseText, _largeFont);
                e.Graphics.DrawString(pauseText,
                    _largeFont,
                    Brushes.Red,
                    Width / 2 - size.Width / 2,
                    Height / 2 - size.Height / 2);
            }

            string[] instructions =
            {
                "",
                "",
                "← → - MOVE PADDLE",
                "↑ - LAUNCH BALL",
                "P - PAUSE",
                "CHEAT CODE: 'end'"
            };

            for (int i = 0; i < instructions.Length; i++)
            {
                e.Graphics.DrawString(
                    instructions[i],
                    _mainFont,
                    Brushes.Gray,
                    10,
                    ClientSize.Height - 100 + i * 15
                );
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

        private void AddCloseButton()
        {
            var closeLabel = new Label
            {
                Text = "╳",
                Size = new Size(30, 30),
                Location = new Point(ClientSize.Width - 35, 5),
                Font = new Font(FontManager.PressStartFont.FontFamily, 12f, FontStyle.Regular),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };

            closeLabel.MouseEnter += (s, e) => closeLabel.ForeColor = Color.Yellow;
            closeLabel.MouseLeave += (s, e) => closeLabel.ForeColor = Color.Red;

            closeLabel.Click += (s, e) =>
            {
                ViewClosedExternally?.Invoke();
                Close();
            };

            closeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Controls.Add(closeLabel);
        }
    }
}
