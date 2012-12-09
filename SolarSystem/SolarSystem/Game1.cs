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
        public List<GameEntity> Children { get; private set; }

        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        public static Game1 Instance { get; private set; }

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = 1366;
            Graphics.PreferredBackBufferHeight = 768;
            Graphics.PreferMultiSampling = true;
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            Instance = this;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

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

            var dt = (float)gameTime.ElapsedGameTime.TotalDays*Setting.Speed;

            Setting.Update(dt);

            foreach (var child in Children)
            {
                child.Update(dt);
            }

            Camera.Update(dt);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();

            var state = new DepthStencilState {DepthBufferEnable = true};
            GraphicsDevice.DepthStencilState = state;

            var dt = (float)gameTime.ElapsedGameTime.TotalDays * Setting.Speed;

            foreach (var child in Children)
            {
                child.Draw(dt);
            }

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
