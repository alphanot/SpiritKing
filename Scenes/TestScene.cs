using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Apos.Input;
using Track = Apos.Input.Track;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using SpiritKing.Controllers;
using SpiritKing.Components.Nodes;

namespace SpiritKing.Scenes;
public class TestScene : Scene
{
    readonly ICondition left = new AnyCondition(
        new KeyboardCondition(Keys.Left),
        new GamePadStickCondition(GamePadStickCondition.GamePadStick.Left, GamePadStickCondition.StickAxis.X, GamePadStickCondition.AxisDirection.Negative, 0)
    );

    readonly ICondition right = new AnyCondition(
        new KeyboardCondition(Keys.Right),
        new GamePadStickCondition(GamePadStickCondition.GamePadStick.Left, GamePadStickCondition.StickAxis.X, GamePadStickCondition.AxisDirection.Positive, 0)
    );

    readonly ICondition up = new AnyCondition(
        new KeyboardCondition(Keys.Up),
        new GamePadStickCondition(GamePadStickCondition.GamePadStick.Left, GamePadStickCondition.StickAxis.Y, GamePadStickCondition.AxisDirection.Positive, 0)
    );

    readonly ICondition down = new AnyCondition(
        new KeyboardCondition(Keys.Down),
        new GamePadStickCondition(GamePadStickCondition.GamePadStick.Left, GamePadStickCondition.StickAxis.Y, GamePadStickCondition.AxisDirection.Negative, 0)
    );

    public TestScene(Game game) : base(game)
    {

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (left.Pressed())
        {
            Debug.WriteLine("pressed");
        }
        if (left.Released())
        {
            Debug.WriteLine("released");
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) => base.Draw(gameTime, spriteBatch);
}
