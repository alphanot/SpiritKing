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

    private PauseScreen _pauseScreen;

    private Vector2 centerOfScreen;
    public GameScene(Game game, SaveData? data) : base(game)
    {
        _pauseScreen = new(game);
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
        AddNode(PosessableController);
        AddNode(_pauseScreen);
        SortNodes();
        ControllerUtils.BumpController(0);
        MusicController.PlaySong(MusicController.Ambience, true);
        centerOfScreen = new Vector2(Game.GraphicsDevice.Viewport.Width / 2f, Game.GraphicsDevice.Viewport.Height / 2f);
        PauseScreen.ExitBtnClicked += PauseMenu_ExitBtnClicked;
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

            //if (ExitKey.Pressed())
            //{
            //    // query game and update `_data`
            //    _data.PlayerPosition = PosessableController.Player.Position;
            //    SaveDataController.SaveGameAsync(_data).Wait();
            //}
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
    }

    public void PauseMenu_ExitBtnClicked(MenuButton menuButton) => base.OnSceneSwitched(new TitleScreenScene(Game));
}