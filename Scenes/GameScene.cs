using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using MonoGame.Extended.ViewportAdapters;
using SpiritKing.Components;
using SpiritKing.Components.Interfaces;
using SpiritKing.Components.Posessables;
using SpiritKing.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SpiritKing.Scenes
{
    public class GameScene : Scene
    {
        Posessable _player;
        HUD _hud;
        Tweener _tweener = new Tweener();
        public GameScene(Game game) : base(game)
        {
            _player = new Goblin(game, true);
            _hud = new HUD(game, _player.Stats.MaxHealth, _player.Stats.MaxStamina, _player.Stats.Health, _player.Stats.Stamina, _player.PlayerState.IsExhausted);
            Name = "Game Scene";
            Camera.LookAt(_player.Position);
            _player.Stats.Name = "player";
            Nodes.Add(_player);
            Nodes.Add(new Gargoyle(game));
            Nodes.Add(new Hound(game));
            Nodes.Add(new Platform(game, 1, new Vector2(350, 500), new Vector2( 600, 50)));
            Nodes.Add(new Platform(game, 1, new Vector2(250, 300), new Vector2( 600, 50)));
            Nodes.Add(new Platform(game, 1, new Vector2(50, 600), new Vector2( 600, 50)));
            Nodes.Add(new Platform(game, 1, new Vector2(0, 500), new Vector2( 50, 50)));
            Nodes.Add(new Platform(game, 1, new Vector2(1000, 500), new Vector2( 1000, 100)));
            Nodes.Add(new Platform(game, 1, new Vector2(1000, 800), new Vector2( 1000, 50)));
            Nodes.Add(new Platform(game, 1, new Vector2(3050, 305), new Vector2( 500, 50)));
            Nodes.Add(_hud);
            SortNodes();
            Posessable.PosessableDied += RemovePosessable;
            Posessable.PosessableSwitched += SwitchPosessable;
            InputController.BumpController();
            MusicController.PlaySong(MusicController.Ambience, true);
            InputController.IsReady = true;
        }

        private void SwitchPosessable(IPosessable posessable)
        {
            _player = (Posessable)posessable;
            _hud.SetHUD(_player.Stats.MaxHealth, _player.Stats.MaxStamina, _player.Stats.Health, _player.Stats.Stamina, _player.PlayerState.IsExhausted);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var x = _player.Position - new Vector2(Game.GraphicsDevice.Viewport.Width / 2f, Game.GraphicsDevice.Viewport.Height / 2f);
            
            _tweener.TweenTo(target: Camera, expression: camera => camera.Position, toValue: x, duration: 0.06f);
            _tweener.Update(gameTime.GetElapsedSeconds());
            
            _hud.SetPosition(Camera.Position);
        }

        private void RemovePosessable(Posessable posessable)
        {
            if (_player == posessable)
            {
                Game.Exit();
            }
            else
            {
                Nodes.Remove(posessable);
                posessable.Dispose();
                posessable = null;
                Debug.Print("GameScene.RemovePosessable()");
            }
            
        }
    }
}
