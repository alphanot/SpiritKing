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
using System.Linq;
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

        // Always re-derive the three fixed save slots from what's on disk, so
        // the menu reflects reality (files added/removed outside the game,
        // saves freshly created, etc).
        _settingsData.SaveSlots = SaveDataController.GetSaveSlots();

        // If the previously-tracked CurrentSave no longer exists on disk, drop it.
        if (_settingsData.CurrentSave != null)
        {
            _settingsData.CurrentSave = _settingsData.SaveSlots
                .FirstOrDefault(s => s.FileName == _settingsData.CurrentSave.FileName && s.IsOccupied);
        }

        var anySaveExists = _settingsData.SaveSlots.Any(s => s.IsOccupied);

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

        if (anySaveExists)
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

            MainMenuBtns.Add(new MenuButton(game, Point.Zero, new Point(400, 0), "New Game")
            {
                Action = NewGameClicked
            });
        }
        else
        {
            MainMenuBtns.Add(new MenuButton(game, Point.Zero, new Point(400, 0), "New Game")
            {
                Action = NewGameClicked,
                Highlighted = true
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
            if (saveSlot.IsOccupied)
            {
                var gameData = SaveDataController.LoadGameSaveAsync(saveSlot.FileName).Result;
                MarkCurrentSave(saveSlot.FileName);
                base.OnSceneSwitched(new GameScene(Game, gameData, saveSlot.FileName));
            }
            else
            {
                // Empty slot — treat the click as "start a new game in this slot".
                StartNewGameInSlot(saveSlot.FileName);
            }
        }
    }

    private void StartNewGameInSlot(string fileName)
    {
        var newSave = new SaveData();
        SaveDataController.SaveGameAsync(newSave, fileName).Wait();
        MarkCurrentSave(fileName);
        base.OnSceneSwitched(new GameScene(Game, newSave, fileName));
    }

    private void MarkCurrentSave(string fileName)
    {
        var slot = _settingsData.SaveSlots.FirstOrDefault(s => s.FileName == fileName);
        if (slot != null)
        {
            // Refresh IsOccupied so the persisted slot reflects that the file now exists.
            _settingsData.CurrentSave = slot with { IsOccupied = true };
            SaveDataController.SaveGameSettingsAsync(_settingsData).Wait();
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
        // Pick the first empty fixed slot and start a fresh game there.
        var emptySlot = _settingsData.SaveSlots.FirstOrDefault(s => !s.IsOccupied);
        if (emptySlot == null)
        {
            // All three slots are full. The user has to delete a save to make room.
            Debug.WriteLine("All save slots are full. Delete a save file to make room for a new game.");
            return;
        }

        StartNewGameInSlot(emptySlot.FileName);
    }

    private void ContinueBtnClicked(MenuButton caller)
    {
        // Prefer the explicitly-tracked CurrentSave; otherwise fall back to the
        // first occupied slot (covers the case where settings have no CurrentSave
        // yet but a save file does exist on disk).
        var slot = _settingsData.CurrentSave
            ?? _settingsData.SaveSlots.FirstOrDefault(s => s.IsOccupied);
        if (slot == null)
        {
            Debug.WriteLine("Continue clicked but no save exists.");
            return;
        }

        var gameData = SaveDataController.LoadGameSaveAsync(slot.FileName).Result;
        MarkCurrentSave(slot.FileName);
        base.OnSceneSwitched(new GameScene(Game, gameData, slot.FileName));
    }

    private void LoadGameClicked(MenuButton caller)
    {
        _menuController.SetMenuButtons(caller.ChildButtons);
        _menuController.UpdateMenuButtonsAlignement();
    }

    private void SettingsClicked(MenuButton caller)
    {
        Debug.WriteLine("Settings clicked.");
        // TODO: real settings menu (volume sliders, etc).
    }

    private void ExitClicked(MenuButton caller)
    {
        Dispose();
        Game.Exit();
    }
}