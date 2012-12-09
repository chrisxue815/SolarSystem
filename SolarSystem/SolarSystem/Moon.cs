using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SolarSystem
{
    public class Moon : GameEntity
    {
        private Earth Earth { get; set; }
        private float Spin { get; set; }
        private VertexPositionColor[] pointList = new VertexPositionColor[2];
        private Vector3 relativePosition;
        private Vector3 RelativePosition
        {
            get { return relativePosition; }
            set { relativePosition = value; Position = Earth.Position + relativePosition; }
        }

        public const float Radius = 3f;
        public const float RevolutionRadius = 20f;
        public const float RevolutionPeriod = 0.75f;
        public const float RevolutionAngularSpeed = MathHelper.TwoPi / RevolutionPeriod;
        public const float RotationPeriod = RevolutionPeriod;
        public const float RotationAngularSpeed = MathHelper.TwoPi / RotationPeriod;

        private static readonly BasicEffect BasicEffect = new BasicEffect(Game1.Instance.GraphicsDevice);

        public Moon(Earth earth)
        {
            Earth = earth;
            Spin = 0;

            RelativePosition = new Vector3(0, 0, -RevolutionRadius);

            Scale = Matrix.CreateScale(new Vector3(Radius, Radius, Radius));

            ModelName = "sphere";

            DiffuseColor = Color.Gray.ToVector3();

            BasicEffect.VertexColorEnabled = true;
            BasicEffect.World = Matrix.Identity;
        }

        public override void Update(GameTime gameTime)
        {
            // delta time
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Axial rotation
            var angle = RevolutionAngularSpeed * dt;
            RelativePosition = Vector3.Transform(RelativePosition, Matrix.CreateRotationY(angle));

            // Revolution
            var rotationAxis = Up;
            rotationAxis.Normalize();
            rotationAxis *= Radius * 2f;
            pointList[0] = new VertexPositionColor(Position + rotationAxis, Color.Red);
            pointList[1] = new VertexPositionColor(Position - rotationAxis, Color.Red);

            Spin += RotationAngularSpeed * dt;
            if (Spin > MathHelper.TwoPi) Spin -= MathHelper.TwoPi;
            rotationAxis.Normalize();
            LocalTransform = Scale*Matrix.CreateFromAxisAngle(rotationAxis, -Spin);
        }

        public override void Draw(GameTime gameTime)
        {
            BasicEffect.View = Game1.Instance.Camera.View;
            BasicEffect.Projection = Game1.Instance.Camera.Projection;

            foreach (var pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game1.Instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                                                 pointList, 0, 1);
            }

            base.Draw(gameTime);
        }
    }
}
