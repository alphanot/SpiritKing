using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpiritKing.Components.Interfaces
{
    public interface INode
    {
        public int DrawOrder { get; }

        public void Update(GameTime gameTime);

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public void Dispose();
    }
}