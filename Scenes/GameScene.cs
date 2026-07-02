using Apos.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using SpiritKing.Components;
using SpiritKing.Components.Nodes;
using SpiritKing.Components.Posessables;
using SpiritKing.Controllers;
using SpiritKing.Data;
using SpiritKing.Utils;
using System.Diagnostics;

namespace SpiritKing.Scenes;

public class GameScene : Scene
{
    private readonly HUD _hud;
    private readonly Tweener _tweener = new();
    private readonly PosessablesController PosessableController;
    private readonly GameWorldController GameWorldController;

    public readonly ICondition PauseButton = new AnyCondition(
      new KeyboardCondition(Keys.Escape),
      new GamePadCondition(GamePadButton.Start, 0)
    );

    private SaveData _data;

    private string? _saveFileName;

    private PauseScreen _pauseScreen;

#if DEBUG
    private bool _debugDrawQuadTree = false;
    private readonly ICondition _debugQuadTreeToggle = new KeyboardCondition(Keys.F1);
    private Texture2D _debugPixel;
#endif

    private Vector2 centerOfScreen;
    public GameScene(Game game, SaveData? data, string? saveFileName = null) : base(game)
    {
        _pauseScreen = new(game);
        _data = data ?? new();
        _saveFileName = saveFileName;
        Name = "Game Scene";

        var levelData = LevelController.LoadLevelAsync(_data.LevelFileName).GetAwaiter().GetResult();

        PosessableController = new PosessablesController();
        GameWorldController = new GameWorldController(game, levelData);

        PosessableController.InitializePosessables(game, levelData);
        PosessableController.InitializePlayer();

        // Use save position if continuing, otherwise use level's spawn point.
        PosessableController.Player.Position = data != null
            ? _data.PlayerPosition
            : levelData.PlayerSpawnPosition;

        Camera.LookAt(PosessableController.Player.Position);
        _hud = new HUD(game, PosessableController.Player);
        _hud.SetPosition(Camera.Position);
        AddNode(_hud);
        AddNode(GameWorldController);
        AddNode(PosessableController);
        AddNode(_pauseScreen);
        SortNodes();
        ControllerUtils.BumpController(0);
        MusicController.PlaySong(MusicController.Ambience, true);
        centerOfScreen = new Vector2(Game.GraphicsDevice.Viewport.Width / 2f, Game.GraphicsDevice.Viewport.Height / 2f);
        PauseScreen.SaveAndExitBtnClicked += PauseMenu_SaveAndExitBtnClicked;
        PauseScreen.SaveBtnClicked += PauseMenu_SaveBtnClicked;
#if DEBUG
        _debugPixel = new Texture2D(game.GraphicsDevice, 1, 1);
        _debugPixel.SetData(new[] { Color.White });
#endif
    }

    public override void Update(GameTime gameTime)
    {
        if (!_pauseScreen.Paused)
        {
            base.Update(gameTime);

            var nextCameraPos = PosessableController.Player.Position - centerOfScreen;

            _tweener.TweenTo(target: Camera, expression: camera => camera.Position, toValue: nextCameraPos, duration: 0.06f);
            _tweener.Update(gameTime.GetElapsedSeconds());

            _hud.SetPosition(Camera.Position);
        }
        else
        {
            _pauseScreen.Update(gameTime);
        }

        if (PauseButton.Pressed())
        {
            _pauseScreen.InvertPaused();
            _pauseScreen.SetScreenPosition(Camera.Position);
        }

#if DEBUG
        if (_debugQuadTreeToggle.Pressed())
        {
            _debugDrawQuadTree = !_debugDrawQuadTree;
        }
#endif
    }

#if DEBUG
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);
        if (_debugDrawQuadTree)
        {
            spriteBatch.Begin(transformMatrix: Camera.GetViewMatrix());
            GameWorldController.QuadTree.DebugDraw(spriteBatch, _debugPixel);
            spriteBatch.End();
        }
    }
#endif

    public void PauseMenu_SaveBtnClicked(MenuButton menuButton) => SaveCurrentGame();

    public void PauseMenu_SaveAndExitBtnClicked(MenuButton menuButton)
    {
        SaveCurrentGame();
        base.OnSceneSwitched(new TitleScreenScene(Game));
    }

    private void SaveCurrentGame()
    {
        if (string.IsNullOrEmpty(_saveFileName))
        {
            Debug.WriteLine("Save attempted but no save file name was provided for this game.");
            return;
        }

        _data.PlayerPosition = PosessableController.Player.Position;
        SaveDataController.SaveGameAsync(_data, _saveFileName).Wait();
    }
}