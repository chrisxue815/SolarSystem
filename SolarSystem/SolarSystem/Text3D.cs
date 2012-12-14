﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SolarSystem
{
    public class Text3D : GameEntity
    {
        public const float Size = 0.08f;
        public Vector3 OriginalPosition { get; set; }

        public Text3D(Vector3 pos)
        {
            OriginalPosition = pos;

            DiffuseColor = Color.White.ToVector3();

            Scale = Matrix.CreateScale(new Vector3(Size, Size, Size));
            LocalTransform = Scale;
        }

        public override void Draw(float dt)
        {
            if (Model != null)
            {
                Position = Game.Camera.Up / Game.Camera.Up.Length() * 20 + OriginalPosition;
                var target = Game.Camera.Position - Position;
                target = Game.Camera.Position - Game.Camera.Target;
                var axis = Vector3.Cross(Vector3.Backward, target);
                axis.Normalize();
                var angle = (float) Math.Acos(Vector3.Dot(Vector3.Backward, target) / target.Length());
                LocalTransform = Matrix.CreateFromAxisAngle(axis, angle);
                Up = Vector3.Transform(Vector3.Up, LocalTransform);
                angle = (float)Math.Acos(Vector3.Dot(Game.Camera.Up, Up) / Game.Camera.Up.Length() / Up.Length());
                target.Normalize();
                LocalTransform *= Matrix.CreateFromAxisAngle(target, -angle);
                var distance = (Position - Game.Camera.Position).Length();
                LocalTransform *= Matrix.CreateScale(distance / 4000f);

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
