using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSystem
{
    public class GameEntity
    {
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

        public GameEntity()
        {
            Up = new Vector3(0, 1, 0);
            Basis = new Vector3(0, 0, -1);
            var random = new Random();
            DiffuseColor = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
        }

        public virtual void LoadContent()
        {
            Model = Game1.Instance.Content.Load<Model>(ModelName);
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
