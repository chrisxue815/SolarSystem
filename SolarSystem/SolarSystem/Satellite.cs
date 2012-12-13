using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSystem
{
    public class Satellite : GameEntity
    {
        public Earth Earth { get; set; }

        public const float RevolutionRadius = 20f;
        public const float RevolutionPeriod = Earth.RotationPeriod;
        public const float GroundHeight = Earth.Radius * 4.2f;

        public const float Radius = 0.5f;

        private Vector3 RelativePosition { get; set; }
        private Vector3 Velocity { get; set; }
        private bool AchievedOrbit { get; set; }

        private static readonly Random Random = new Random();

        public Satellite()
        {
            Earth = Game.Earth;

            DiffuseColor = Color.Gray.ToVector3();

            // generate a random initial position
            RelativePosition = new Vector3(GroundHeight, 0, 0);
            var randomAngle = (float)Random.NextDouble() * MathHelper.TwoPi;
            RelativePosition = Vector3.Transform(RelativePosition, Matrix.CreateRotationY(randomAngle));

            Velocity = Vector3.Cross(RelativePosition, Vector3.Up);
            Velocity = Velocity / Velocity.Length() * 5;
            Velocity += RelativePosition / RelativePosition.Length();

            Scale = Matrix.CreateScale(Radius);
            LocalTransform = Scale;

            ModelName = @"Models\moon";
        }

        public override void Update(float dt)
        {
            if (!AchievedOrbit)
            {
                if ((RelativePosition.Length()) >= RevolutionRadius)
                {
                    RelativePosition = RelativePosition/RelativePosition.Length()*RevolutionRadius;
                    AchievedOrbit = true;
                }
                else
                {
                    var accelerate = Velocity;
                    accelerate = accelerate / accelerate.Length() * 5;
                    accelerate -= RelativePosition / RelativePosition.Length() * 5.45f;

                    Velocity += accelerate*dt;
                    RelativePosition += Velocity*dt;
                }
            }
        }

        public override void Draw(float dt)
        {
            if (Model != null)
            {
                Position = Vector3.Transform(RelativePosition, Earth.RotationTransform * Matrix.CreateTranslation(Earth.Position));

                foreach (var mesh in Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.DiffuseColor = DiffuseColor;
                        effect.World = LocalTransform * Matrix.CreateTranslation(Position);
                        effect.Projection = Game.Camera.Projection;
                        effect.View = Game.Camera.View;
                    }

                    mesh.Draw();
                }
            }
        }
    }
}
