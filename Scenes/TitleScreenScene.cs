using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using SpiritKing.Components.Nodes;
using SpiritKing.Controllers;
using SpiritKing.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static SpiritKing.Data.SettingsData;

namespace SpiritKing.Scenes;

public class TitleScreenScene : Scene
{
    private readonly Label TitleText;
    private List<MenuButton> SaveSlotBtns = [];
    private List<MenuButton> MainMenuBtns = [];
    private readonly MenuController _menuController;
    readonly ParticleController _bottomOfScreenParticles;
    private SettingsData _settingsData;
    public TitleScreenScene(Game game) : base(game)
    {
        _settingsData = SaveDataController.LoadGameSettingsAsync().Result;
        var screen = game.GraphicsDevice.Viewport;

        if (_settingsData == null)
        {
            SaveDataController.SaveGameSettingsAsync(new()).Wait();
            _settingsData = new();
        }
        TitleText = new Label(game, "Posessed Will", Vector2.Zero, Color.DarkRed);
        TitleText.Position = new Vector2((screen.Width - TitleText.STRING_SIZE.X) / 2, (screen.Height - TitleText.STRING_SIZE.Y) / 12);
        var bottomOfTitleText = (int)(TitleText.Position.Y + TitleText.STRING_SIZE.Y);

        foreach (var save in _settingsData.SaveSlots)
        {
            SaveSlotBtns.Add(new SaveSlotMenuButton(Game, Point.Zero, new(400, 0), save)
            {
                Action = ExistingSaveClicked
            });

        }
        SaveSlotBtns.Add(new(Game, Point.Zero, new(400, 0), "Back")
        {
            Action = DisplayMainMenu
        });

        if (_settingsData.CurrentSave == null)
        {
            MainMenuBtns.Add(new MenuButton(game, Point.Zero, new Point(400, 0), "New Game")
            {
                Action = NewGameClicked,
                Highlighted = true
            });
        }
        else
        {
            MainMenuBtns.Add(new MenuButton(game, Point.Zero, new Point(400, 0), "Continue")
            {
                Action = ContinueBtnClicked,
                Highlighted = true
            });

            MainMenuBtns.Add(new MenuButton(game, Point.Zero, new Point(400, 0), "Load Game")
            {
                Action = LoadGameClicked,
                ChildButtons = SaveSlotBtns
            });
        }

        MainMenuBtns.Add(new MenuButton(game, Point.Zero, new Point(400, 0), "Settings")
        {
            Action = SettingsClicked
        });

        MainMenuBtns.Add(new MenuButton(game, Point.Zero, new Point(400, 0), "Exit")
        {
            Action = ExitClicked
        });

        BackgroundColor = Color.Black;
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

        _menuController = new MenuController(new Point(0, bottomOfTitleText), new Vector2(screen.Width, screen.Height - bottomOfTitleText));

        AddNode(_bottomOfScreenParticles);
        AddNode(_menuController);
        AddNode(TitleText);

        _menuController.SetMenuButtons(MainMenuBtns);

        _menuController.UpdateMenuButtonsAlignement();
    }

    private void ExistingSaveClicked(MenuButton button)
    {
        if (button is SaveSlotMenuButton saveSlot)
        {
            var gameData = SaveDataController.LoadGameSaveAsync(saveSlot.FileName).Result;
            base.OnSceneSwitched(new GameScene(Game, gameData));
        }
    }

    private void DisplayMainMenu(MenuButton button)
    {
        _menuController.SetMenuButtons(MainMenuBtns);
        _menuController.UpdateMenuButtonsAlignement();
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

    private void NewGameClicked(MenuButton caller)
    {
        //TODO
    }

    private void ContinueBtnClicked(MenuButton caller)
    {
        var gameData = SaveDataController.LoadGameSaveAsync(_settingsData.CurrentSave.FileName).Result;
        base.OnSceneSwitched(new GameScene(Game, gameData));
    }

    private void LoadGameClicked(MenuButton caller)
    {
        _menuController.SetMenuButtons(caller.ChildButtons);
        _menuController.UpdateMenuButtonsAlignement();
    }

    private void SettingsClicked(MenuButton caller)
    {
        Debug.WriteLine("Settings clicked.");
        var saves = SaveDataController.GetSaveFileNames();

        if (saves.Count <= 0)
        {
            if (_settingsData.SaveSlots.Count == saves.Count)
            {
                _settingsData.SaveSlots.Add(new SaveSlot(true, "Occupied Save", 0, "Save1.dat"));
                _settingsData.CurrentSave = _settingsData.SaveSlots[0];
            }
        }
        SaveDataController.SaveGameSettingsAsync(_settingsData).Wait();
    }

    private void ExitClicked(MenuButton caller)
    {
        Dispose();
        Game.Exit();
    }
}