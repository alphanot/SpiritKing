using Microsoft.Xna.Framework;

namespace SpiritKing.Components
{
    public class Camera
    {
        public Game Game { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Size { get; set; }

        private GraphicsDeviceManager graphicsDeviceManager;

        public Camera(Game game)
        {
            Game = game;
            Position = Vector2.Zero;
            Size = Vector2.Zero;

            SetScreen();
        }

        public Camera(Game game, int width, int height, int x = 0, int y = 0)
        {
            Game = game;
            Size = new Vector2(width, height);
            SetScreen();
        }

        public Camera(Game game, Vector2 position, int width, int height)
        {
            Game = game;
            Size = new Vector2(width, height);
            Position = position;
            SetScreen();
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void SetPosition(float x, float y)
        {
            Position = new Vector2(x, y);
        }

        private void SetScreen()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(Game);
            graphicsDeviceManager.PreferredBackBufferWidth = (int)Size.X;
            graphicsDeviceManager.PreferredBackBufferHeight = (int)Size.Y;
            graphicsDeviceManager.ApplyChanges();
        }
    }
}