﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpiritKing.Components.Interfaces;
using SpiritKing.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpiritKing.Components.Posessables
{
    public class PosessablesHandler : INode
    {
        public int DrawOrder => 1;

        public List<IPosessable> Posessables { get; set; }

        public IPosessable Player { get; set; }

        public IPosessable HighlightedPosessable { get; set; }

        public static event Action<IPosessable> PosessableSwitched;

        public PosessablesHandler()
        {
            Posessable.PosessableDied += RemovePosessable;
            Posessables = new List<IPosessable>();
        }

        public void InitializePosessables(Game game)
        {
            Posessables.Add(new Goblin(game, new Vector2(0, 0)));
            Posessables.Add(new Gargoyle(game, new Vector2(1000, 0)));
            Posessables.Add(new Hound(game, new Vector2(400, 0)));
        }

        public IPosessable InitializePlayer()
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
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (InputController.GetRightStickPastDeadzone(InputController.GameState.Game))
            {
                HighlightPosessable();
            }
            else if (HighlightedPosessable != null)
            {
                HighlightedPosessable.IsHighlighted = false;
                HighlightedPosessable = null;
            }

            if (InputController.IsPressed(Buttons.RightShoulder, InputController.GameState.Game) && Player.PosessIsReady() && HighlightedPosessable.CanBePosessed() && HighlightedPosessable != null)
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
                    HandleEnemyAI(p, seconds);
                p.Update(gameTime);
            }
        }

        private void HandleEnemyAI(IPosessable p, float seconds)
        {
            // Check for aggro
            if (p.EnemyAIFieldOfView.Shape.Intersects(Player.Collider.Shape))
            {
                if (Player.Position.X < p.Position.X)
                {
                    p.PlayerState.MovementX = States.PlayerState.MovementStateX.MoveLeft;
                }
                else
                {
                    p.PlayerState.MovementX = States.PlayerState.MovementStateX.MoveRight;
                }

                if (p.PlayerState.LastDirection == States.PlayerState.LastLookState.Left)
                {
                    // p.Position.X - p.NormalAttack.AttackCollisionShape.Shape.Width
                }
                if (Math.Abs(Player.Position.X) - Math.Abs(p.Position.X - p.NormalAttack.AttackCollisionShape.Shape.Width) < 0)
                {
                    // p.NormalAttack.Update(seconds, p => Debug.Print("hello"), p.PlayerState.IsExhausted);
                    Debug.Print("hello");
                }
            }
            else
            {
                p.PlayerState.MovementX = States.PlayerState.MovementStateX.Idle;
            }
        }

        private void HighlightPosessable()
        {
            IPosessable closestPosessable = null;
            foreach (var p in Posessables)
            {
                p.IsHighlighted = false;
                if (Player.PosessRay.Intersects(p.PosessableCollider.Shape) && !p.Collider.Shape.Contains(Player.PosessRay.Position))
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
}