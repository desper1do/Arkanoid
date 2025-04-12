using System;
using System.Drawing;
using System.Windows.Forms;

public partial class LevelSelectionForm : Form
{
    public int SelectedLevel { get; private set; } = 1;
    private readonly int unlockedLevels;

    public LevelSelectionForm(int unlockedLevels)
    {
        this.unlockedLevels = unlockedLevels;
        CreateLevelButtons();
    }

    private void CreateLevelButtons()
    {
        const int btnWidth = 120;
        const int btnHeight = 50;
        const int margin = 20;

        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.Text = "Выбор уровня";
        this.StartPosition = FormStartPosition.CenterScreen;

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

            btn.Click += (s, e) =>
            {
                SelectedLevel = (int)btn.Tag;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(btn);
        }

        var backBtn = new Button
        {
            Text = "Назад",
            Size = new Size(btnWidth, btnHeight),
            Location = new Point(margin, margin + 5 * (btnHeight + 10)),
            Font = new Font("Arial", 12)
        };
        backBtn.Click += (s, e) => this.Close();

        this.Controls.Add(backBtn);
        this.ClientSize = new Size(btnWidth + 2 * margin, margin + 6 * (btnHeight + 10));
    }
}