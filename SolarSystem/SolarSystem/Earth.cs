using System;
using Microsoft.Xna.Framework;

namespace SolarSystem
{
    public class Earth : GameEntity
    {
        private Sun Sun { get; set; }

        public const float Mass = 5.9742e24f;
        //public const float Radius = 6.372797e6f;
        public const float Radius = 10f;
        public const float RevolutionRadius = 1.4958e11f;

        public const float GravitationalConstant = 6.67e-11f;

        public const float RevolutionPeriod = 240f;

        public Earth(Sun sun)
        {
            Sun = sun;

            var speed = (float)Math.Sqrt(GravitationalConstant * Sun.Mass / RevolutionRadius);

            Position = new Vector3(Sun.Position.X + RevolutionRadius, Sun.Position.Y, Sun.Position.Z);

            LocalTransform = Matrix.CreateScale(new Vector3(Radius, Radius, Radius));

            ModelName = "sphere";
        }

        public override void Update(GameTime gameTime)
        {
            // delta time
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var relativePosition = Position - Sun.Position;
            var revolution = Quaternion.CreateFromAxisAngle(Up, MathHelper.Pi/RevolutionPeriod*dt);
            var relativePosition2 = Vector3.Transform(relativePosition, revolution);
            Position = relativePosition2 + Sun.Position;
        }
    }
}
