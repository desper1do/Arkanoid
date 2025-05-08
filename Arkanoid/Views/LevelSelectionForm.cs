using System;
using System.Drawing;
using System.Windows.Forms;

namespace Arkanoid.Views
{
    public class LevelSelectionView : Form
    {
        public event Action<int> LevelSelected;
        public event Action BackClicked;

        public LevelSelectionView(int unlockedLevels)
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            this.ControlBox = false;
            Text = "SELECT LEVEL";
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            BackColor = Color.Black;
            InitializeComponents(unlockedLevels);
        }

        private void InitializeComponents(int unlockedLevels)
        {
            const int btnWidth = 120;
            const int btnHeight = 40;
            const int margin = 20;

            for (int i = 1; i <= 5; i++)
            {
                var btn = new Button
                {
                    Text = $"LEVEL {i}",
                    Tag = i,
                    Size = new Size(btnWidth, btnHeight),
                    Location = new Point(margin, margin + (i - 1) * (btnHeight + 10)),
                    Font = new Font(FontManager.PressStartFont.FontFamily, 9f),
                    BackColor = i <= unlockedLevels ? Color.Lime : Color.Gray,
                    ForeColor = Color.Black,
                    Enabled = i <= unlockedLevels,
                    Padding = new Padding(0, 7, 0, 0)
                };
                btn.Click += (s, e) => LevelSelected?.Invoke((int)btn.Tag);

                Controls.Add(btn);
            }

            var backBtn = new Button
            {
                Text = "BACK",
                Size = new Size(btnWidth, btnHeight),
                Location = new Point(margin, margin + 5 * (btnHeight + 10)),
                Font = new Font(FontManager.PressStartFont.FontFamily, 9f),
                BackColor = Color.Magenta,
                ForeColor = Color.Black,
                Padding = new Padding(0, 7, 0, 0)
            };
            backBtn.Click += (s, e) => BackClicked?.Invoke();

            Controls.Add(backBtn);
            ClientSize = new Size(btnWidth + 2 * margin, margin + 6 * (btnHeight + 10));
        }
    }
}
