using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using SpiritKing.Components;
using SpiritKing.Components.Interfaces;
using SpiritKing.Components.Nodes;
using SpiritKing.Components.Posessables;
using SpiritKing.Controllers;
using SpiritKing.Utils;

namespace SpiritKing.Scenes;

public class GameScene : Scene
{
    private readonly HUD _hud;
    private readonly Tweener _tweener = new();
    private PosessablesHandler PosessableHandler { get; set; }

    private GameWorldHandler gameWorld { get; set; }

    public GameScene(Game game) : base(game)
    {
        Name = "Game Scene";
        PosessableHandler = new PosessablesHandler();
        gameWorld = new GameWorldHandler(game);
        
        PosessableHandler.InitializePosessables(game, gameWorld);
        PosessableHandler.InitializePlayer();
        Camera.LookAt(PosessableHandler.Player.Position);
        _hud = new HUD(game, PosessableHandler.Player);
        _hud.SetPosition(Camera.Position);

        AddNode(_hud);
        AddNode(gameWorld);
        SortNodes();
        ControllerUtils.BumpController(0);
        MusicController.PlaySong(MusicController.Ambience, true);
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