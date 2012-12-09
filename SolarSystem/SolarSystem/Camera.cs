using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolarSystem
{
    public class Camera : GameEntity
    {
        public Matrix Projection { get; private set; }
        public Matrix View { get; private set; }

        private Vector3 Target { get; set; }

        private int perspective = 1;
        private bool perspectiveChanged = false;

        public Camera()
        {
            Position = new Vector3(0, 100, 200);
            Target = new Vector3(0, 0, 20);
            Up = new Vector3(0, 1, 0);

            Calculate();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var key in Keyboard.GetState().GetPressedKeys())
            {
                var code = key.GetHashCode() - 48;

                if (code == 1)
                {
                    perspective = 1;
                    perspectiveChanged = true;
                    Position = new Vector3(0, 100, 200);
                    Target = new Vector3(0, 0, 20);
                    Up = new Vector3(0, 1, 0);
                }
                else if (code == 2)
                {
                    perspective = 2;
                }
            }

            if (perspective == 2)
            {
                Target = Game.Earth.Position;
                Up = Game.Earth.Up;

                var toSun = Game.Sun.Position - Game.Earth.Position;
                var earthRight = Vector3.Cross(Game.Earth.Up, toSun);
                earthRight.Normalize();

                Position = Game.Earth.Position + earthRight * 50;

                perspectiveChanged = true;
            }

            if (perspectiveChanged)
            {
                perspectiveChanged = false;
                Calculate();
            }
        }

        public override void Draw(GameTime gameTime)
        {
        }

        private void Calculate()
        {
            View = Matrix.CreateLookAt(Position, Target, Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, Game.Graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000.0f);
        }
    }
}
