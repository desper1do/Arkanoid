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
            Text = "Выбор уровня";
            StartPosition = FormStartPosition.CenterScreen;

            CreateLevelButtons(unlockedLevels);
        }

        private void CreateLevelButtons(int unlockedLevels)
        {
            const int btnWidth = 120;
            const int btnHeight = 50;
            const int margin = 20;

            for (int i = 1; i <= 5; i++)
            {
                var btn = new Button
                {
                    Text = $"Уровень {i}",
                    Tag = i,
                    Size = new Size(btnWidth, btnHeight),
                    Location = new Point(margin, margin + (i - 1) * (btnHeight + 10)),
                    Font = new Font("Arial", 12),
                    BackColor = i <= unlockedLevels ? Color.LightGreen : Color.LightGray,
                    Enabled = i <= unlockedLevels
                };

                if (!btn.Enabled)
                {
                    btn.Text += Environment.NewLine + "(недоступно)";
                }

                btn.Click += (s, e) => LevelSelected?.Invoke((int)btn.Tag);

                Controls.Add(btn);
            }

            var backBtn = new Button
            {
                Text = "Назад",
                Size = new Size(btnWidth, btnHeight),
                Location = new Point(margin, margin + 5 * (btnHeight + 10)),
                Font = new Font("Arial", 12)
            };
            backBtn.Click += (s, e) => BackClicked?.Invoke();

            Controls.Add(backBtn);
            ClientSize = new Size(btnWidth + 2 * margin, margin + 6 * (btnHeight + 10));
        }
    }
}
