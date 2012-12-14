using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolarSystem
{
    public class Camera : GameEntity
    {
        public Matrix Projection { get; private set; }
        public Matrix View { get; private set; }

        public Vector3 Target { get; set; }

        private int previousPerspective = 1;
        private bool perspectiveChanged;

        public Camera()
        {
            Position = new Vector3(0, 100, 200);
            Target = new Vector3(0, 0, 20);
            Up = new Vector3(0, 1, 0);

            Calculate();
        }

        public override void Update(float dt)
        {
            var perspective = Game.Setting.Perspective;

            if (perspective == 1 && previousPerspective != 1)
            {
                Position = new Vector3(0, 100, 200);
                Target = new Vector3(0, 0, 20);
                Up = new Vector3(0, 1, 0);

                perspectiveChanged = true;
            }

            if (perspective == 2)
            {
                Target = Game.Earth.Position;
                Up = Game.Earth.Up;

                var toSun = Game.Sun.Position - Game.Earth.Position;
                var earthLeft = Vector3.Cross(toSun, Game.Earth.Up);
                earthLeft.Normalize();

                Position = Game.Earth.Position + earthLeft * 60;

                perspectiveChanged = true;
            }

            if (perspective == 3)
            {
                Target = Game.Earth.Position;
                Up = Game.Earth.Up;

                var toSun = Game.Sun.Position - Game.Earth.Position;
                var earthRight = Vector3.Cross(Game.Earth.Up, toSun);
                earthRight.Normalize();

                Position = Game.Earth.Position + earthRight * 60;

                perspectiveChanged = true;
            }

            if (perspective == 4)
            {
                var earth = Game.Earth;

                Position = earth.Position + earth.Up*100;
                Target = earth.Position;
                Up = Vector3.Forward;

                perspectiveChanged = true;
            }

            if (perspective == 5)
            {
                var earth = Game.Earth;

                var relativeUp = Vector3.Transform(Vector3.Backward,
                                  Matrix.CreateRotationZ(Earth.EclipticObliquity)*
                                  Matrix.CreateFromAxisAngle(earth.Up, earth.Rotation));
                relativeUp.Normalize();

                Position = earth.Position + relativeUp * Earth.Radius;
                Target = Position + relativeUp;

                Up = earth.Up;

                perspectiveChanged = true;
            }

            previousPerspective = perspective;

            if (perspectiveChanged)
            {
                perspectiveChanged = false;
                Calculate();
            }
        }

        private void Calculate()
        {
            View = Matrix.CreateLookAt(Position, Target, Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, Game.Graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000.0f);
        }
    }
}
