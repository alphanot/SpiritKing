using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritKing.Controllers;
using SpiritKing.Scenes;
using System;
using System.Diagnostics;

namespace SpiritKing
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SceneController _sceneController;
        private Texture2D whiteRectangle;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            try
            {
                _sceneController = new SceneController();
                _sceneController.SetScene(new TitleScreenScene(this));
                base.Initialize();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override void LoadContent()
        {
            try
            {
                _spriteBatch = new SpriteBatch(GraphicsDevice);
                whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
                whiteRectangle.SetData(new[] { Color.White });
                Debug.WriteLine(GraphicsDevice.Viewport.Width);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override void UnloadContent()
        {
            try
            {
                _sceneController.CurrentScene.Dispose();
                base.UnloadContent();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override void Update(GameTime gameTime)
        {
            try
            {
                _sceneController.CurrentScene.Update(gameTime);

                base.Update(gameTime);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            try
            {
                _sceneController.CurrentScene.Draw(gameTime, _spriteBatch);
                base.Draw(gameTime);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}