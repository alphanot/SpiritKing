using Apos.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using SpiritKing.Components.Nodes;
using SpiritKing.Components.Posessables;
using SpiritKing.Controllers;
using SpiritKing.Data;
using SpiritKing.Utils;

namespace SpiritKing.Scenes;

public class GameScene : Scene
{
    private readonly HUD _hud;
    private readonly Tweener _tweener = new();
    private PosessablesController PosessableController { get; set; }
    private GameWorldController GameWorldController { get; set; }

    public readonly ICondition ExitKey = new AnyCondition(
      new KeyboardCondition(Keys.Escape)
  );

    private SaveData _data;

    public GameScene(Game game, SaveData? data) : base(game)
    {
        _data = data ?? new();
        Name = "Game Scene";

        PosessableController = new PosessablesController();
        GameWorldController = new GameWorldController(game);

        PosessableController.InitializePosessables(game);
        PosessableController.InitializePlayer();

        Camera.LookAt(PosessableController.Player.Position);
        _hud = new HUD(game, PosessableController.Player);
        _hud.SetPosition(Camera.Position);
        AddNode(_hud);
        AddNode(GameWorldController);
        SortNodes();
        ControllerUtils.BumpController(0);
        MusicController.PlaySong(MusicController.Ambience, true);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        PosessableController.Update(gameTime);

        var x = PosessableController.Player.Position - new Vector2(Game.GraphicsDevice.Viewport.Width / 2f, Game.GraphicsDevice.Viewport.Height / 2f);

        _tweener.TweenTo(target: Camera, expression: camera => camera.Position, toValue: x, duration: 0.06f);
        _tweener.Update(gameTime.GetElapsedSeconds());

        _hud.SetPosition(Camera.Position);

        if (ExitKey.Pressed())
        {
            // query game and update `_data`
            _data.PlayerPosition = PosessableController.Player.Position;
            SaveDataController.SaveGameAsync(_data).Wait();
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);

        // not sure if this is correct. To reopen the spritebatch, but it should work for now until I have more knowledge.
        var transformMatrix = Camera.GetViewMatrix();
        spriteBatch.Begin(transformMatrix: transformMatrix);
        PosessableController.Draw(gameTime, spriteBatch);
        spriteBatch.End();
    }
}