using System;
using System.Drawing;
using System.Windows.Forms;

namespace Arkanoid.Views
{
    public class MessageView : Form
    {
        public MessageView(string title, string message, Color bgColor)
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = bgColor;
            ForeColor = Color.White;
            Size = new Size(400, 200);
            DoubleBuffered = true;

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font(FontManager.PressStartFont.FontFamily, 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20),
                Padding = new Padding(0, 10, 0, 0)
            };

            var messageLabel = new Label
            {
                Text = message,
                Font = new Font(FontManager.PressStartFont.FontFamily, 10),
                AutoSize = true,
                Location = new Point(20, 60),
                Padding = new Padding(0, 7, 0, 0)
            };

            var okButton = new Button
            {
                Text = "OK",
                Font = new Font(FontManager.PressStartFont.FontFamily, 10),
                Size = new Size(100, 40),
                Location = new Point(150, 120),
                BackColor = Color.Lime,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Padding = new Padding(0, 7, 0, 0)
            };
            okButton.FlatAppearance.BorderSize = 0;
            okButton.Click += (s, e) => Close();

            Controls.Add(titleLabel);
            Controls.Add(messageLabel);
            Controls.Add(okButton);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawRectangle(new Pen(Color.White, 3),
                new Rectangle(0, 0, Width - 1, Height - 1));
        }
    }
}