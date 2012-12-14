using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SolarSystem
{
    public class Text3D : GameEntity
    {
        public const float Size = 0.1f;

        private Sun Sun { get; set; }
        private Earth Earth { get; set; }

        public Text3D(Vector3 pos)
        {
            Sun = Game.Sun;
            Earth = Game.Earth;

            Position = pos;

            DiffuseColor = Color.White.ToVector3();

            Scale = Matrix.CreateScale(new Vector3(Size, Size, Size));
            LocalTransform = Scale;
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
