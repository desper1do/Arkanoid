using System.Drawing;

public class Ball
{
    public Rectangle Bounds { get; private set; }
    private Point velocity;
    public int formWidth, formHeight;

    public Ball(int formWidth, int formHeight)
    {
        this.formWidth = formWidth;
        this.formHeight = formHeight;
        Bounds = new Rectangle(formWidth / 2, formHeight / 2, 10, 10);
        velocity = new Point(5, -5);
    }

    public void Move()
    {
        if (Bounds.Right >= formWidth)
        {
            Bounds = new Rectangle(formWidth - Bounds.Width, Bounds.Y, Bounds.Width, Bounds.Height);
            velocity.X = -velocity.X;
        }
        else if (Bounds.Left <= 0)
        {
            Bounds = new Rectangle(0, Bounds.Y, Bounds.Width, Bounds.Height);
            velocity.X = -velocity.X;
        }
        else if (Bounds.Top <= 0)
        {
            Bounds = new Rectangle(Bounds.X, 0, Bounds.Width, Bounds.Height);
            velocity.Y = -velocity.Y;
        }

        Bounds = new Rectangle(
            Bounds.X + velocity.X,
            Bounds.Y + velocity.Y,
            Bounds.Width,
            Bounds.Height);
    }

    public void Bounce()
    {
        velocity.Y = -velocity.Y;
    }
}