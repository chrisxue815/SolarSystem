using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSystem
{
    public class GameEntity
    {
        public Game1 Game { get; set; }

        // logical
        public Vector3 Position { get; set; }

        //public Vector3 Look { get; set; }
        //public Vector3 Right { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Basis { get; set; }

        // draw
        public string ModelName { get; set; }
        public Model Model { get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Matrix LocalTransform { get; set; }
        public Matrix Scale { get; set; }

        public GameEntity()
        {
            Game = Game1.Instance;
            Up = new Vector3(0, 1, 0);
            Basis = new Vector3(0, 0, -1);
        }

        public virtual void LoadContent()
        {
            Model = Game.Content.Load<Model>(ModelName);
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (Model != null)
            {
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
