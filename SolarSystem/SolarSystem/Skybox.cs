using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSystem
{
    public class Skybox : GameEntity
    {
        public const float Radius = 0.2f;

        public Texture2D Texture;
        public string TextureName;

        public Skybox()
        {
            ModelName = @"Models\skybox";
            TextureName = @"Textures\space";
        }

        public override void LoadContent()
        {
            if (TextureName != null)
                Texture = Game.Content.Load<Texture2D>(TextureName);
            base.LoadContent();
        }

        public override void Draw(float dt)
        {
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            // Copy the transform of each bone of the skybox model into a matrix array
            var skytransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(skytransforms);

            foreach (var mesh in Model.Meshes)
            {
                // Set mesh orientation, and camera projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.TextureEnabled = true;
                    effect.Texture = Texture;
                    effect.AmbientLightColor = new Vector3(1, 1, 1);
                    effect.World = skytransforms[mesh.ParentBone.Index] *
                                    Matrix.CreateScale(2000.0f) *
                                    Matrix.CreateTranslation(Game.Camera.Position);
                    effect.View = Game.Camera.View;
                    effect.Projection = Game.Camera.Projection;
                }

                mesh.Draw();
            }
        }
    }
}
