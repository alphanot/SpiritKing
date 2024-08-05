using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using SpiritKing.Components.Nodes;
using SpiritKing.Controllers;
using System;
using System.Diagnostics;

namespace SpiritKing.Scenes;

public class TitleScreenScene : Scene
{
    private readonly Label TitleText;
    private readonly MenuButton NewGameBtn;
    private readonly MenuButton LoadGameBtn;
    private readonly MenuButton SettingsBtn;
    private readonly MenuButton ExitBtn;

    private readonly MenuController _menuController;

    readonly ParticleController _bottomOfScreenParticles;

    public TitleScreenScene(Game game) : base(game)
    {
        TitleText = new Label(game, "Posessed Will", Vector2.Zero, Color.DarkRed);
        NewGameBtn = new MenuButton(game, new Point(0, 0), new Point(350, 0), "New Game", 0);
        LoadGameBtn = new MenuButton(game, new Point(0, 0), new Point(350, 0), "Load Game", 0);
        SettingsBtn = new MenuButton(game, new Point(0, 0), new Point(350, 0), "Settings", 0);
        ExitBtn = new MenuButton(game, new Point(0, 0), new Point(350, 0), "Exit", 0);
        _menuController = new MenuController([NewGameBtn, LoadGameBtn, SettingsBtn, ExitBtn]);

        NewGameBtn.Action = NewGameClicked;
        LoadGameBtn.Action = LoadGameClicked;
        SettingsBtn.Action = SettingsClicked;
        ExitBtn.Action = ExitClicked;
        NewGameBtn.Highlighted = true;
        BackgroundColor = Color.Black;
        var screen = game.GraphicsDevice.Viewport;
        _bottomOfScreenParticles = new ParticleController(game.GraphicsDevice, new Vector2(screen.Width / 2, screen.Height));

        _bottomOfScreenParticles.AddEmitter(
            new ParticleEmitter(_bottomOfScreenParticles.TextureRegion, 250, TimeSpan.FromSeconds(4),
                Profile.Line(new Vector2(1, 0), screen.Width))
            {
                Parameters = new ParticleReleaseParameters
                {
                    Speed = new Range<float>(0.1f, 50f),
                    Quantity = 1,
                    Rotation = new Range<float>(-1f, 1f),
                    Scale = new Range<float>(1.0f, 6f),
                    Color = new Range<HslColor>(HslColor.FromRgb(Color.DarkRed), HslColor.FromRgb(Color.Yellow)),
                },
                Modifiers =
                    {
                        new AgeModifier
                        {
                            Interpolators =
                            {
                                new OpacityInterpolator
                                {
                                    StartValue = 0.6f, EndValue = 0f
                                },
                                new HueInterpolator
                                {
                                    StartValue = 30f,
                                    EndValue = -25f
                                }
                            }
                        },
                        new RotationModifier {RotationRate = -2.1f},
                        new LinearGravityModifier {Direction = new Vector2(- 0.2f, -1f), Strength = 9.3f},

                    }
            });

        AddNode(_bottomOfScreenParticles);
        AddNode(_menuController);
        AddNode(TitleText);
        MusicController.PlaySong(MusicController.OrganTheme, true);

        TitleText.Position = new Vector2((screen.Width - TitleText.STRING_SIZE.X) / 2, (screen.Height - TitleText.STRING_SIZE.Y) / 8);
        NewGameBtn.Position = new Point((screen.Width - NewGameBtn.Size.X) / 2, 2 * (screen.Height - NewGameBtn.Size.Y) / 5);
        LoadGameBtn.Position = new Point((screen.Width - LoadGameBtn.Size.X) / 2, 3 * (screen.Height - LoadGameBtn.Size.Y) / 5);
        SettingsBtn.Position = new Point((screen.Width - SettingsBtn.Size.X) / 2, 4 * (screen.Height - SettingsBtn.Size.Y) / 5);
        ExitBtn.Position = new Point((screen.Width - ExitBtn.Size.X) / 2, 5 * (screen.Height - ExitBtn.Size.Y) / 5);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Dispose()
    {
        base.Dispose();
        _bottomOfScreenParticles.Dispose();
        _menuController.Dispose();
        TitleText.Dispose();
    }

    private void NewGameClicked()
    {
        base.OnSceneSwitched(new GameScene(Game, null));
    }

    private void LoadGameClicked()
    {
        var data = SaveDataController.LoadGameSaveAsync().Result;
        base.OnSceneSwitched(new GameScene(Game, data));
    }

    private void SettingsClicked()
    {
        Debug.WriteLine("Settings clicked.");
    }

    private void ExitClicked()
    {
        Dispose();
        Game.Exit();
    }
}