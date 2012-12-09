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

        // revolution
        public const float RevolutionRadius = 20f;
        public const float RevolutionPeriod = 29.53f;
        public const float RevolutionAngularSpeed = MathHelper.TwoPi / RevolutionPeriod;

        // rotation
        public const float RotationPeriod = RevolutionPeriod;
        public const float RotationAngularSpeed = MathHelper.TwoPi / RotationPeriod;

        private BasicEffect BasicEffect { get; set; }

        public Moon()
        {
            Earth = Game.Earth;
            Spin = 0;

            RelativePosition = new Vector3(0, 0, -RevolutionRadius);

            Scale = Matrix.CreateScale(new Vector3(Radius, Radius, Radius));

            ModelName = "sphere";

            DiffuseColor = Color.Gray.ToVector3();

            BasicEffect = new BasicEffect(Game1.Instance.GraphicsDevice);
            BasicEffect.VertexColorEnabled = true;
            BasicEffect.World = Matrix.Identity;
        }

        public override void Update(float dt)
        {
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
            if (Spin > MathHelper.TwoPi) Spin %= MathHelper.TwoPi;
            rotationAxis.Normalize();
            LocalTransform = Scale*Matrix.CreateFromAxisAngle(rotationAxis, Spin);
        }

        public override void Draw(float dt)
        {
            if (Game.Setting.ShowAxis)
            {
                BasicEffect.View = Game.Camera.View;
                BasicEffect.Projection = Game.Camera.Projection;

                foreach (var pass in BasicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                                           pointList, 0, 1);
                }
            }

            base.Draw(dt);
        }
    }
}
