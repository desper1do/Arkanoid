
using System;
using System.Drawing;

namespace Arkanoid.Models
{
    public class Ball
    {
        public Rectangle Bounds { get; private set; }
        private PointF _velocity;
        public int formWidth;
        public int formHeight;
        private const float Speed = 5f;
        private const float MaxBounceAngle = 75f;

        public PointF Velocity => _velocity;

        public Ball(int formWidth, int formHeight)
        {
            this.formWidth = formWidth;
            this.formHeight = formHeight;
            Bounds = new Rectangle(formWidth / 2, formHeight / 2, 10, 10);
            ResetVelocity();
        }

        public void ResetVelocity()
        {
            Random rand = new Random();

            double angleDeg = rand.Next(30, 150);
            double angleRad = angleDeg * Math.PI / 180;

            float vx = Speed * (float)Math.Cos(angleRad);
            float vy = -Speed * (float)Math.Sin(angleRad);

            _velocity = new PointF(vx, vy);
        }

        private void NormalizeVelocity()
        {
            float length = (float)Math.Sqrt(_velocity.X * _velocity.X + _velocity.Y * _velocity.Y);
            if (length == 0) return;
            _velocity = new PointF(Speed * (_velocity.X / length), Speed * (_velocity.Y / length));
        }

        public void Move()
        {
            float newX = Bounds.X + _velocity.X;
            float newY = Bounds.Y + _velocity.Y;

            if (newX <= 0 || newX + Bounds.Width >= formWidth)
            {
                _velocity.X = -_velocity.X;
                NormalizeVelocity();
                newX = Bounds.X + _velocity.X;
            }

            if (newY <= 0)
            {
                _velocity.Y = -_velocity.Y;
                NormalizeVelocity();
                newY = Bounds.Y + _velocity.Y;
            }

            Bounds = new Rectangle((int)newX, (int)newY, Bounds.Width, Bounds.Height);
        }

        public void BounceFromPaddle(Rectangle paddleBounds)
        {
            float ballCenter = Bounds.X + Bounds.Width / 2f;
            float paddleCenter = paddleBounds.X + paddleBounds.Width / 2f;

            float distanceFromCenter = ballCenter - paddleCenter;
            float normalized = distanceFromCenter / (paddleBounds.Width / 2f);
            normalized = Math.Max(-1f, Math.Min(1f, normalized));

            float angle = normalized * MaxBounceAngle;
            float rad = angle * (float)Math.PI / 180f;

            _velocity.X = Speed * (float)Math.Sin(rad);
            _velocity.Y = -Speed * (float)Math.Cos(rad);

            NormalizeVelocity();
        }

        public void BounceFromBlock(Rectangle blockBounds)
        {
            Rectangle intersect = Rectangle.Intersect(Bounds, blockBounds);

            if (intersect.Width < intersect.Height)
            {
                _velocity.X = -_velocity.X;
            }
            else
            {
                _velocity.Y = -_velocity.Y;
            }

            NormalizeVelocity();
        }

        public void SetPosition(int x, int y)
        {
            Bounds = new Rectangle(x, y, Bounds.Width, Bounds.Height);
        }

        public void SetVelocity(float vx, float vy)
        {
            _velocity = new PointF(vx, vy);
        }
    }
}
