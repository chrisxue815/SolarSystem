using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSystem
{
    public class Earth : GameEntity
    {
        private Sun Sun { get; set; }
        private float Spin { get; set; }
        private VertexPositionColor[] pointList = new VertexPositionColor[2];

        public const float Radius = 10f;
        public const float RevolutionRadius = 100f;
        public const float RevolutionPeriod = 10f;
        public const float RevolutionAngularSpeed = MathHelper.TwoPi / RevolutionPeriod;
        public const float RotationPeriod = RevolutionPeriod / 365;
        public const float RotationAngularSpeed = MathHelper.TwoPi / RotationPeriod;

        private static readonly BasicEffect BasicEffect = new BasicEffect(Game1.Instance.GraphicsDevice);

        public Earth(Sun sun)
        {
            Sun = sun;
            Spin = 0;

            Position = new Vector3(Sun.Position.X, Sun.Position.Y, Sun.Position.Z - RevolutionRadius);

            Scale = Matrix.CreateScale(new Vector3(Radius, Radius, Radius));

            ModelName = "sphere";

            DiffuseColor = Color.Blue.ToVector3();

            BasicEffect.VertexColorEnabled = true;
            BasicEffect.World = Matrix.Identity;
        }

        public override void Update(GameTime gameTime)
        {
            // delta time
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var relativePosition = Position - Sun.Position;
            var angle = RevolutionAngularSpeed * dt;
            var relativePosition2 = Vector3.Transform(relativePosition, Matrix.CreateRotationY(angle));
            Position = relativePosition2 + Sun.Position;

            const float eclipticObliquity = -0.409f;
            var rotationAxis = Vector3.Transform(Up, Matrix.CreateRotationZ(eclipticObliquity));
            rotationAxis.Normalize();
            rotationAxis *= Radius * 2f;
            pointList[0] = new VertexPositionColor(Position + rotationAxis, Color.Red);
            pointList[1] = new VertexPositionColor(Position - rotationAxis, Color.Red);

            Spin += RotationAngularSpeed * dt;
            if (Spin > MathHelper.TwoPi) Spin -= MathHelper.TwoPi;
            rotationAxis.Normalize();
            LocalTransform = Scale*Matrix.CreateRotationZ(eclipticObliquity)*
                             Matrix.CreateFromAxisAngle(rotationAxis, -Spin);
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

            if (Model != null)
            {
                foreach (var mesh in Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.DiffuseColor = DiffuseColor;
                        effect.World = LocalTransform * Matrix.CreateTranslation(Position);
                        effect.Projection = Game1.Instance.Camera.Projection;
                        effect.View = Game1.Instance.Camera.View;
                    }

                    mesh.Draw();
                }
            }
        }
    }
}
