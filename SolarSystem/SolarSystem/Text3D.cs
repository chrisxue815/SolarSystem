using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SolarSystem
{
    public class Text3D : GameEntity
    {
        public const float Size = 0.08f;

        public Text3D(Vector3 pos)
        {
            Position = pos;

            DiffuseColor = Color.White.ToVector3();

            Scale = Matrix.CreateScale(new Vector3(Size, Size, Size));
            LocalTransform = Scale;
        }

        public override void Draw(float dt)
        {
            if (Model != null)
            {
                Position = Game.Camera.Up * 20 + Game.Earth.Position;
                var target = Game.Camera.Position - Position;
                //var target = Game.Camera.Target - Game.Camera.Position;
                var axis = Vector3.Cross(Vector3.Backward, target);
                axis.Normalize();
                var angle = (float) Math.Acos(Vector3.Dot(Vector3.Backward, target) / target.Length());
                var matrix = Matrix.CreateFromAxisAngle(axis, angle);
                Up = Vector3.Transform(Vector3.Up, matrix);
                angle = (float)Math.Acos(Vector3.Dot(Game.Camera.Up, Up) / Game.Camera.Up.Length() / Up.Length());
                target.Normalize();
                matrix *= Matrix.CreateFromAxisAngle(target, -angle);

                foreach (var mesh in Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();
                        effect.DiffuseColor = DiffuseColor;
                        effect.World = LocalTransform * matrix * Matrix.CreateTranslation(Position);
                        effect.Projection = Game.Camera.Projection;
                        effect.View = Game.Camera.View;
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
