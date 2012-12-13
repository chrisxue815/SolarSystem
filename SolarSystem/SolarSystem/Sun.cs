using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSystem
{
    public class Sun : GameEntity
    {
        public const float Radius = 0.2f;

        public Sun(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);

            Scale = Matrix.CreateScale(Radius);
            LocalTransform = Scale * Matrix.CreateRotationZ(MathHelper.PiOver4) * Matrix.CreateRotationX(MathHelper.PiOver4);

            ModelName = @"Models\sun";

            DiffuseColor = Color.White.ToVector3();
        }

        public override void Draw(float dt)
        {
            if (Model != null)
            {
                foreach (var mesh in Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();
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
