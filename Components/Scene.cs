using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Timers;
using MonoGame.Extended.ViewportAdapters;
using SpiritKing.Components.Interfaces;
using SpiritKing.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpiritKing.Components
{
    public class Scene : INode
    {

        public List<INode> Nodes { get; private set; } = new List<INode>();
        public string Name { get; internal set; }

        public MusicController MusicController { get; private set; }

        public OrthographicCamera Camera { get; set; }

        public virtual event Action<Scene> SceneSwitched;

        public int DrawOrder { get; } = 0;

        public virtual Game Game { get; set; }

        public Color BackgroundColor { get; set; }
        public Texture2D _logo;
        public Scene(Game game)
        {
            Game = game;
            var viewportAdapter = new BoxingViewportAdapter(game.Window, game.GraphicsDevice, 1280, 720);
            Camera = new OrthographicCamera(viewportAdapter);
            MusicController = new(game.Content);
            BackgroundColor = Color.DarkGray;

        }

        public virtual void Update(GameTime gameTime)
        { 
            for (int i = Nodes.Count - 1; i >= 0; i--)
            {
                Nodes[i].Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Game.GraphicsDevice.Clear(BackgroundColor);
            var transformMatrix = Camera.GetViewMatrix();
            spriteBatch.Begin(transformMatrix: transformMatrix);

            foreach (var node in Nodes)
            {
                node.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();
            
        }

        public virtual void SortNodes()
        {
            Nodes.Sort((a, b) => a.DrawOrder.CompareTo(b.DrawOrder));
        }

        public virtual void Dispose()
        {
            foreach (var node in Nodes)
            {
                node.Dispose();
                Debug.Print("Scene.Dispose() Foreach(" + node.ToString() +")");
            }
            MusicController.Unload();
        }

    }
}
