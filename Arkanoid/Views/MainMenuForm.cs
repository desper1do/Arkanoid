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

        public MainMenuView()
        {
            Width = 500;
            Height = 500;
            Text = "Арканоид - Главное меню";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            BackColor = Color.DarkBlue;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Label logo = new Label
            {
                Text = "ARKANOID",
                Font = new Font("Arial", 32, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Top = 50
            };
            logo.Left = (ClientSize.Width - logo.Width) / 3;

            Button playButton = new Button
            {
                Text = "Играть",
                Font = new Font("Arial", 14),
                Size = new Size(200, 40),
                Top = 150,
                BackColor = Color.White
            };
            playButton.Left = (ClientSize.Width - playButton.Width) / 2;
            playButton.Click += (s, e) => PlayClicked?.Invoke();

            Button levelsButton = new Button
            {
                Text = "Выбор уровня",
                Font = new Font("Arial", 14),
                Size = new Size(200, 40),
                Top = 200,
                BackColor = Color.White
            };
            levelsButton.Left = (ClientSize.Width - levelsButton.Width) / 2;
            levelsButton.Click += (s, e) => LevelsClicked?.Invoke();

            Button exitButton = new Button
            {
                Text = "Выход",
                Font = new Font("Arial", 14),
                Size = new Size(200, 40),
                Top = 250,
                BackColor = Color.White
            };
            exitButton.Left = (ClientSize.Width - exitButton.Width) / 2;
            exitButton.Click += (s, e) => ExitClicked?.Invoke();

            Controls.Add(logo);
            Controls.Add(playButton);
            Controls.Add(levelsButton);
            Controls.Add(exitButton);
        }
    }
}
