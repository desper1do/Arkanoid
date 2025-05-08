using System.Drawing;

namespace Arkanoid.Models
{
    public class Paddle
    {
        public Rectangle Bounds { get; private set; }
        private int speed = 5;
        private int formWidth;

        public Paddle(int formWidth)
        {
            this.formWidth = formWidth;
            Bounds = new Rectangle(formWidth / 2 - 50, 400, 100, 10);
        }

        public void Move(int direction)
        {
            int newX = Bounds.X + direction * speed;
            if (newX >= 0 && newX + Bounds.Width <= formWidth)
            {
                Bounds = new Rectangle(newX, Bounds.Y, Bounds.Width, Bounds.Height);
            }
        }
    }
}