using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSystem
{
    public class Earth : GameEntity
    {
        private Sun Sun { get; set; }

        public const float Radius = 10f;
        public const float RevolutionRadius = 100f;
        public const float RevolutionPeriod = 10f;

        private static readonly BasicEffect BasicEffect = new BasicEffect(Game1.Instance.GraphicsDevice);

        public Earth(Sun sun)
        {
            Sun = sun;

            Position = new Vector3(Sun.Position.X, Sun.Position.Y, Sun.Position.Z - RevolutionRadius);

            LocalTransform = Matrix.CreateScale(new Vector3(Radius, Radius, Radius));

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
            var angle = MathHelper.Pi/RevolutionPeriod*dt;
            var relativePosition2 = Vector3.Transform(relativePosition, Matrix.CreateRotationY(angle));
            Position = relativePosition2 + Sun.Position;
        }

        public override void Draw(GameTime gameTime)
        {
            const float angle = -0.409f;

            var axis = Vector3.Transform(Up, Matrix.CreateRotationZ(angle));
            axis.Normalize();
            axis *= Radius*2f;

            var pointList = new VertexPositionColor[4];
            pointList[0] = new VertexPositionColor(Position + axis, Color.Red);
            pointList[1] = new VertexPositionColor(Position - axis, Color.Red);

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
