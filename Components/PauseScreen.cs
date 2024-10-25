using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritKing.Components.Nodes;
using SpiritKing.Controllers;
using System;

namespace SpiritKing.Components;
public class PauseScreen : Interfaces.IUpdateable, Interfaces.IDrawable
{
    public int DrawOrder => 1;

    public bool Visible => true;

    public bool Enabled => true;

    public int UpdateOrder => 1;

    public bool Paused { get; set; } = false;
    
    public static event Action<MenuButton>? ExitBtnClicked;

    private MenuController MenuController;

    private MenuButton ResumeBtn;
    private MenuButton SaveBtn;
    private MenuButton ExitBtn;

    private Color BackgroundColor;

    private Texture2D BackgroundTexture;

    private Vector2 ScreenOffset = Vector2.Zero;

    private Point ScreenSize = Point.Zero;
    public PauseScreen(Game game)
    {
        ScreenSize = new(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
        BackgroundColor = Color.Black * 0.65f;
        MenuController = new(new(0, 0), new(ScreenSize.X / 2, ScreenSize.Y / 2));
        ResumeBtn = new(game, new(0, 0), new(400, 0), "Resume")
        {
            Action = _ => InvertPaused()
        };

        SaveBtn = new(game, new(0, 0), new(400, 0), "Save");

        ExitBtn = new(game, new(0, 0), new(400, 0), "Exit")
        {
            Action = btn => ExitBtnClicked?.Invoke(btn)
        };

        MenuController.SetMenuButtons([ResumeBtn, ExitBtn]);
        MenuController.UpdateMenuButtonsAlignement();
        BackgroundTexture = new Texture2D(game.GraphicsDevice, 1, 1);
        BackgroundTexture.SetData(new[] { Color.White });
    }

    public void Dispose()
    {
        BackgroundTexture.Dispose();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (Paused)
        {
            spriteBatch.Draw(BackgroundTexture, new Rectangle(MenuController.Position.X, MenuController.Position.Y, (int)MenuController.Size.X, (int)MenuController.Size.Y), BackgroundColor);
            MenuController.Draw(gameTime, spriteBatch);
        }
    }

    public void Update(GameTime gameTime)
    {
        if (Paused)
        {
            MenuController.Update(gameTime);
        }
    }

    public void SetScreenPosition(Vector2 offset)
    {
        ScreenOffset = offset;
        // center menu
        var menuNewPosX = ScreenOffset.X + ((ScreenSize.X / 2) - (MenuController.Size.X / 2));
        var menuNewPosY = ScreenOffset.Y + ((ScreenSize.Y / 2) - (MenuController.Size.Y / 2));
        MenuController.Position = new((int)menuNewPosX, (int)menuNewPosY);
        // update menu buttons
        MenuController.UpdateMenuButtonsAlignement();
    }

    public bool InvertPaused()
    {
        Paused = !Paused;
        return Paused;
    }
}
