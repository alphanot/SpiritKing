using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using SpiritKing.Components.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritKing.Controllers;
public class DamageTextController : Components.Interfaces.IUpdateable, Components.Interfaces.IDrawable
{
    private readonly Game Game;
    private readonly Tweener _tweener = new();

    public bool Enabled => true;

    public int UpdateOrder => 1;

    public int DrawOrder => 1;

    public bool Visible => true;

    public List<DamageText> damageTexts = new();

    private readonly FastRandom rand = new FastRandom();

    public DamageTextController(Game game)
    {
        Game = game;
    }

    public void Hit(Vector2 position, int damage)
    {
        damageTexts.Add(new DamageText(Game, position, damage.ToString()));
        _tweener.TweenTo(target: damageTexts.Last(), expression: x => x.Position, toValue: GetRandomPosition(position), duration: 0.5f)
            .Easing(EasingFunctions.QuadraticOut).OnEnd(x => damageTexts.Remove(x.Target as DamageText));

    }

    public void Update(GameTime gameTime)
    {
        _tweener.Update(gameTime.GetElapsedSeconds());
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        foreach (DamageText damageText in damageTexts)
        {
            damageText.Draw(gameTime, spriteBatch);
        }
    }

    public void Dispose() => GC.SuppressFinalize(this);

    private Vector2 GetRandomPosition(Vector2 startPos)
    {
        var differenceX = rand.Next(1, 50);
        var differenceY = rand.Next(1, 20);

        return startPos + new Vector2(differenceX + 50, -(differenceY + 50));
    }
}
