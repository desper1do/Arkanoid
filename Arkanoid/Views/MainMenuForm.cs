using System;
using System.Drawing;
using System.Windows.Forms;

namespace Arkanoid.Views
{
    public class MainMenuView : Form
    {
        public event Action PlayClicked;
        public event Action LevelsClicked;
        public event Action ExitClicked;
        public event Action EndlessClicked;

        public MainMenuView()
        {
            Width = 500;
            Height = 500;
            Text = "ARKANOID - MAIN MENU";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(100, 100);
            MaximizeBox = false;
            this.ControlBox = false;
            DoubleBuffered = true;
            BackColor = Color.Black;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Label logo = new Label
            {
                Text = "ARKANOID",
                Font = new Font(FontManager.PressStartFont.FontFamily, 25f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Top = 75
            };
            logo.Left = (ClientSize.Width - logo.Width) / 4;

            Button playButton = CreateButton("PLAY", 150, Color.Lime, () => PlayClicked?.Invoke());
            Button levelsButton = CreateButton("LEVELS", 200, Color.Cyan, () => LevelsClicked?.Invoke());
            Button endlessButton = CreateButton("ENDLESS", 250, Color.Magenta, () => EndlessClicked?.Invoke());
            Button exitButton = CreateButton("EXIT", 300, Color.Red, () => ExitClicked?.Invoke());

            Controls.Add(logo);
            Controls.Add(playButton);
            Controls.Add(levelsButton);
            Controls.Add(endlessButton);
            Controls.Add(exitButton);
        }

        private Button CreateButton(string text, int top, Color backColor, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font(FontManager.PressStartFont.FontFamily, 12f),
                Size = new Size(200, 40),
                Top = top + 10,
                BackColor = backColor,
                ForeColor = Color.Black,
                Padding = new Padding(0, 10, 0, 0)
            };
            btn.Left = (ClientSize.Width - btn.Width) / 2;
            btn.Click += (s, e) => onClick();
            return btn;
        }
    }
}
