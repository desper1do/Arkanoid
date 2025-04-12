using System;
using System.Drawing;
using System.Windows.Forms;

public class MainMenuForm : Form
{
    public MainMenuForm()
    {
        this.Width = 500;
        this.Height = 500;
        this.Text = "Арканоид - Главное меню";
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.DoubleBuffered = true;
        this.BackColor = Color.DarkBlue;

        GameProgress.LoadProgress();
        InitializeComponents();
        this.FormClosing += (s, e) => GameProgress.ResetProgress();
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
        logo.Left = (this.ClientSize.Width - logo.Width) / 3;

        Button playButton = new Button
        {
            Text = "Играть",
            Font = new Font("Arial", 14),
            Size = new Size(200, 40),
            Top = 150,
            BackColor = Color.White
        };
        playButton.Left = (this.ClientSize.Width - playButton.Width) / 2;
        playButton.Click += (s, e) => StartGame(GameProgress.UnlockedLevels);

        Button levelsButton = new Button
        {
            Text = "Выбор уровня",
            Font = new Font("Arial", 14),
            Size = new Size(200, 40),
            Top = 200,
            BackColor = Color.White
        };
        levelsButton.Left = (this.ClientSize.Width - levelsButton.Width) / 2;
        levelsButton.Click += (s, e) => ShowLevelSelection();

        Button exitButton = new Button
        {
            Text = "Выход",
            Font = new Font("Arial", 14),
            Size = new Size(200, 40),
            Top = 250,
            BackColor = Color.White
        };
        exitButton.Left = (this.ClientSize.Width - exitButton.Width) / 2;
        exitButton.Click += (s, e) => Application.Exit();

        this.Controls.Add(logo);
        this.Controls.Add(playButton);
        this.Controls.Add(levelsButton);
        this.Controls.Add(exitButton);
    }

    private void ShowLevelSelection()
    {
        var levelForm = new LevelSelectionForm(GameProgress.UnlockedLevels);
        if (levelForm.ShowDialog() == DialogResult.OK)
        {
            StartGame(levelForm.SelectedLevel);
        }
    }

    private void StartGame(int level)
    {
        var gameForm = new GameForm(level);
        gameForm.Show();
        this.Hide();

        gameForm.FormClosed += (s, args) =>
        {
            if (gameForm.IsLevelCompleted && level >= GameProgress.UnlockedLevels && level < 5)
            {
                GameProgress.SaveProgress(level + 1);
            }
            this.Show();
        };
    }
}