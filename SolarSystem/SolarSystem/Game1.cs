using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace SolarSystem
{
    public class Game1 : Game
    {
        public Setting Setting { get; set; }
        public Camera Camera { get; set; }
        public Sun Sun { get; set; }
        public Earth Earth { get; set; }
        public Moon Moon { get; set; }
        public List<GameEntity> Children { get; private set; }

        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        public Model model;
        public Texture2D texture;

        private SoundEffect soundEffect;
        private SoundEffectInstance soundEffectIns;

        public static Game1 Instance { get; private set; }

        public Game1()
        {
            Content.RootDirectory = "Content";
            Instance = this;

            Graphics = new GraphicsDeviceManager(this)
                           {
                               PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                               PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
                               PreferMultiSampling = true,
                               SynchronizeWithVerticalRetrace = true
                           };
            Graphics.ApplyChanges();

            Setting = new Setting();
            Camera = new Camera();
            Children = new List<GameEntity>();
        }
        protected override void Initialize()
        {
            Sun = new Sun(0, 0, 0);
            Earth = new Earth();
            Moon = new Moon();

            Children.Add(Sun);
            Children.Add(Earth);
            Children.Add(Moon);
            Children.Add(new Monitor());

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>(@"Models\Skybox");
            texture = Content.Load<Texture2D>(@"Textures\space");

            soundEffect = Content.Load<SoundEffect>(@"Sound\sound");
            soundEffectIns = soundEffect.CreateInstance();
            soundEffectIns.Volume = 1.0f;
            soundEffectIns.IsLooped = true;
            soundEffectIns.Play();

            foreach (var child in Children)
            {
                child.LoadContent();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var dt = (float) gameTime.ElapsedGameTime.TotalDays*Setting.Speed;

            Setting.Update(dt);

            foreach (var child in Children)
            {
                child.Update(dt);
            }

            ToggleFullScreen();

            Camera.Update(dt);

            base.Update(gameTime);
        }

        private void ToggleFullScreen()
        {
            if (Graphics.IsFullScreen != Setting.FullScreen)
            {
                Graphics.IsFullScreen = Setting.FullScreen;
                Graphics.ApplyChanges();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();

            //================ Using Skybox to add background ==============//
            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            var skytransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(skytransforms);

            foreach (var mesh in model.Meshes)
            {
                // Set mesh orientation, and camera projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.TextureEnabled = true;
                    effect.Texture = texture;
                    effect.AmbientLightColor = new Vector3(1, 1, 1);
                    effect.World = skytransforms[mesh.ParentBone.Index]*Matrix.CreateScale(2000.0f)*
                                   Matrix.CreateTranslation(Camera.Position);
                    //effect.World = skytransforms[mesh.ParentBone.Index] * Matrix.CreateScale(2000.0f) * RotationMatrix * Matrix.CreateTranslation(Camera.Position);
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;
                }

                mesh.Draw();
            }
            //===================== End of Using Skybox ==============//

            var dt = (float) gameTime.ElapsedGameTime.TotalDays*Setting.Speed;

            foreach (var child in Children)
            {
                child.Draw(dt);
            }

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
