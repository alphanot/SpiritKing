using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using SpiritKing.Components;
using SpiritKing.Components.Interfaces;
using SpiritKing.Components.Posessables;
using SpiritKing.Controllers;

namespace SpiritKing.Scenes
{
    public class GameScene : Scene
    {
        private HUD _hud;
        private Tweener _tweener = new Tweener();
        private PosessablesHandler PosessableHandler { get; set; }

        public GameScene(Game game) : base(game)
        {
            InputController.SetGameState(InputController.GameState.Game);
            Name = "Game Scene";
            PosessableHandler = new PosessablesHandler();
            PosessableHandler.InitializePosessables(game);
            PosessableHandler.InitializePlayer();
            Camera.LookAt(PosessableHandler.Player.Position);
            _hud = new HUD(game, PosessableHandler.Player.Stats.MaxHealth, PosessableHandler.Player.Stats.MaxStamina, PosessableHandler.Player.Stats.Health, PosessableHandler.Player.Stats.Stamina, PosessableHandler.Player.PlayerState.IsExhausted);

            Nodes.Add(new Platform(game, 1, new Vector2(350, 500), new Vector2(600, 50)));
            Nodes.Add(new Platform(game, 1, new Vector2(250, 300), new Vector2(600, 50)));
            Nodes.Add(new Platform(game, 1, new Vector2(50, 600), new Vector2(600, 50)));
            Nodes.Add(new Platform(game, 1, new Vector2(0, 500), new Vector2(50, 50)));
            Nodes.Add(new Platform(game, 1, new Vector2(1000, 500), new Vector2(1000, 100)));
            Nodes.Add(new Platform(game, 1, new Vector2(1000, 800), new Vector2(1000, 50)));
            Nodes.Add(new Platform(game, 1, new Vector2(3050, 305), new Vector2(500, 50)));
            Nodes.Add(_hud);
            SortNodes();
            PosessablesHandler.PosessableSwitched += SwitchPosessable;
            InputController.BumpController(InputController.GameState.Game);
            MusicController.PlaySong(MusicController.Ambience, true);
            InputController.IsReady = true;
        }

        private void SwitchPosessable(IPosessable posessable)
        {
            _hud.SetHUD(PosessableHandler.Player.Stats.MaxHealth, PosessableHandler.Player.Stats.MaxStamina, PosessableHandler.Player.Stats.Health, PosessableHandler.Player.Stats.Stamina, PosessableHandler.Player.PlayerState.IsExhausted);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            PosessableHandler.Update(gameTime);
            var x = PosessableHandler.Player.Position - new Vector2(Game.GraphicsDevice.Viewport.Width / 2f, Game.GraphicsDevice.Viewport.Height / 2f);

            _tweener.TweenTo(target: Camera, expression: camera => camera.Position, toValue: x, duration: 0.06f);
            _tweener.Update(gameTime.GetElapsedSeconds());

            _hud.SetPosition(Camera.Position);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            // not sure if this is correct. To reopen the spritebatch, but it should work for now until I have more knowledge.
            var transformMatrix = Camera.GetViewMatrix();
            spriteBatch.Begin(transformMatrix: transformMatrix);
            PosessableHandler.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}