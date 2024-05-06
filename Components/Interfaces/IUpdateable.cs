using Microsoft.Xna.Framework;

namespace SpiritKing.Components.Interfaces;

public interface IUpdateable : INode
{
    bool Enabled { get; }

    int UpdateOrder { get; }

    void Update(GameTime gameTime);
}
