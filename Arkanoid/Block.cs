using System.Drawing;

public class Block
{
    public Rectangle Bounds { get; private set; }
    public bool IsDestroyed { get; set; }
    public Color Color { get; private set; }

    public Block(int x, int y, int width, int height, Color color)
    {
        Bounds = new Rectangle(x, y, width, height);
        IsDestroyed = false;
        Color = color;
    }
}