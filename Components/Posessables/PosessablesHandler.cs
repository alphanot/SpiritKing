using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpiritKing.Components.Posessables;

public class PosessablesHandler : Interfaces.IDrawable, Interfaces.IUpdateable
{
    public int DrawOrder => 1;

    public List<Posessable> Posessables { get; set; }

    public Posessable Player { get; set; }

    public Posessable HighlightedPosessable { get; set; }

    public bool Enabled => true;

    public int UpdateOrder => 1;

    public bool Visible => true;

    public static event Action<Posessable> PosessableSwitched;

    public PosessablesHandler()
    {
        Posessable.PosessableDied += RemovePosessable;
        Posessables = new List<Posessable>();
    }

    public void InitializePosessables(Game game, GameWorldHandler gameWorld)
    {
        Posessables.Add(new Goblin(game, new Vector2(0, 0), gameWorld));
        //Posessables.Add(new Gargoyle(game, new Vector2(1000, 0), gameWorld));
        Posessables.Add(new Hound(game, new Vector2(600, 0), gameWorld));
    }

    public Posessable InitializePlayer()
    {
        Player = Posessables[0];
        Player.Posess();
        return Player;
    }

    public void Dispose()
    {
        foreach (var p in Posessables)
        {
            p.Dispose();
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        foreach (var p in Posessables)
        {
            p.Draw(gameTime, spriteBatch);
        }
    }

    public void Update(GameTime gameTime)
    {
        var seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (Player.InputController.IsRightStickMoving())
        {
            HighlightPosessable();
        }
        else if (HighlightedPosessable != null)
        {
            HighlightedPosessable.IsHighlighted = false;
            HighlightedPosessable = null;
        }

        if (Player.InputController.Posess.Pressed() && Player.PosessIsReady() && HighlightedPosessable != null && HighlightedPosessable.CanBePosessed())
        {
            Player.Unposess();
            Player = HighlightedPosessable;
            Player.Posess();
            HighlightedPosessable = null;
            PosessableSwitched.Invoke(Player);
        }

        foreach (var p in Posessables)
        {
            if (!p.IsPosessed)
            {
                HandleEnemyAI(p, gameTime);
            }

            p.Update(gameTime);
        }

    }

    private void HandleEnemyAI(Posessable enemy, GameTime gameTime)
    {
        enemy.EnemyAI.DelayNormalAttack.Update(gameTime);
        enemy.EnemyAI.StopNormalAttack.Update(gameTime);
        enemy.EnemyAI.WalkAroundAndSwitchDirections.Update(gameTime);

        // Check for aggro
        if (enemy.EnemyAI.EnemyAIFieldOfView.Shape.Intersects(Player.CollisionShape.Shape))
        {
            if (enemy.OtherPosessableIsWithinAttackingRange(Player))
            {
                enemy.EnemyAI.DelayNormalAttack.Start();
            }
            enemy.EnemyAI.MovementX = !enemy.EnemyAI.NormalAttackActivated ? Player.Position.X < enemy.Position.X ? -1 : 1 : 0;
        }
        else
        {
            enemy.EnemyAI.WalkAroundAndSwitchDirections.Start();
        }
    }

    private void HighlightPosessable()
    {
        Posessable closestPosessable = null;
        foreach (var p in Posessables)
        {
            p.IsHighlighted = false;
            if (Player.PosessRay.Intersects(p.PosessableCollider.Shape) && !p.CollisionShape.Shape.Contains(Player.PosessRay.Position))
            {
                if (closestPosessable == null)
                {
                    closestPosessable = p;
                }
                else
                {
                    if (Microsoft.Xna.Framework.Vector2.Distance(closestPosessable.Position, Player.Position) >
                        Microsoft.Xna.Framework.Vector2.Distance(p.Position, Player.Position))
                    {
                        closestPosessable = p;
                    }
                }
            }
        }
        if (closestPosessable != null)
        {
            HighlightedPosessable = closestPosessable;
            HighlightedPosessable.IsHighlighted = true;
        }
    }

    private void RemovePosessable(Posessable posessable)
    {
        if (Player == posessable)
        {
            Debug.Print("implement death ");
        }
        else
        {
            Posessables.Remove(posessable);
            posessable.Dispose();
        }
    }
}

public static class PosessableExtensions
{
    public static bool OtherPosessableIsWithinAttackingRange(this Posessable current, Posessable other)
    {
        var halfCurrentWidth = current.Stats.Width / 2;
        var currentCenterPoint = current.Position.X + halfCurrentWidth;

        var halfOtherWidth = other.Stats.Width / 2;
        var otherCenterPoint = other.Position.X + halfOtherWidth;

        return current.Position.X < other.Position.X
            ? currentCenterPoint + halfCurrentWidth + current.NormalAttack.CollisionShape.Shape.Width > otherCenterPoint - halfOtherWidth
            : currentCenterPoint - halfCurrentWidth - current.NormalAttack.CollisionShape.Shape.Width < otherCenterPoint - halfOtherWidth;
    }
}