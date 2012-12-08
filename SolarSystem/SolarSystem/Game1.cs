using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SolarSystem
{
    public class Game1 : Game
    {
        public List<GameEntity> Children { get; private set; }
        public Camera Camera { get; set; }

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

            Children = new List<GameEntity>();
            Camera = new Camera();
        }

        protected override void Initialize()
        {
            var sun = new Sun(0, 0, 0);
            var earth = new Earth(sun);

            Children.Add(sun);
            Children.Add(earth);

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

            Camera.Update(gameTime);

            foreach (var child in Children)
            {
                child.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();

            var state = new DepthStencilState {DepthBufferEnable = true};
            GraphicsDevice.DepthStencilState = state;

            foreach (var child in Children)
            {
                child.Draw(gameTime);
            }

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
