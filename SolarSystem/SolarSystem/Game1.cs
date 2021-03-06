using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SolarSystem
{
    public class Game1 : Game
    {
        public Setting Setting { get; set; }
        public Camera Camera { get; set; }
        public Sun Sun { get; set; }
        public Earth Earth { get; set; }
        public Moon Moon { get; set; }
        public Skybox Skybox { get; set; }
        public Sound Sound { get; set; }
        public List<GameEntity> Children { get; private set; }

        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

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
        }

        protected override void Initialize()
        {
            Setting = new Setting();
            Camera = new Camera();
            Children = new List<GameEntity>();

            Skybox = new Skybox();
            Sound = new Sound();
            Sun = new Sun(0, 0, 0);
            Earth = new Earth();
            Moon = new Moon();

            Children.Add(Sound);
            Children.Add(Sun);
            Children.Add(Earth);
            Children.Add(Moon);
            Children.Add(new SatelliteManager());
            Children.Add(new Monitor());

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Skybox.LoadContent();
            
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
            var dt = (float)gameTime.ElapsedGameTime.TotalDays * Setting.Speed;

            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();

            Skybox.Draw(dt);

            var state = new DepthStencilState { DepthBufferEnable = true };
            GraphicsDevice.DepthStencilState = state;

            foreach (var child in Children)
            {
                child.Draw(dt);
            }

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
