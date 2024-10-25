using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using SpiritKing.Components.Nodes;
using SpiritKing.Controllers.InputControllers;
using System.Collections.Generic;

namespace SpiritKing.Controllers;

public class MenuController : Components.Interfaces.IUpdateable, Components.Interfaces.IDrawable
{
    private List<MenuButton> Buttons { get; set; } = [];
    private int currentBtnIndex;

    public int DrawOrder => 1;

    public bool Enabled => true;

    public int UpdateOrder => 1;

    public Point Position { get; set; }

    public Vector2 Size { get; set; }

    public bool Visible => true;

    private readonly MenuInputController _menuInputController = new();
    public MenuController(Point position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public void Update(GameTime gameTime)
    {
        if (_menuInputController.Down.Pressed())
        {
            HighlightChanged(false);
            currentBtnIndex++;
            if (currentBtnIndex >= Buttons.Count)
            {
                currentBtnIndex = 0;
            }
            HighlightChanged(true);
        }
        else if (_menuInputController.Up.Pressed())
        {
            HighlightChanged(false);
            currentBtnIndex--;
            if (currentBtnIndex < 0)
            {
                currentBtnIndex = Buttons.Count - 1;
            }
            HighlightChanged(true);
        }

        if (_menuInputController.Select.Pressed())
        {
            Buttons[currentBtnIndex].Action?.Invoke(Buttons[currentBtnIndex]);
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        foreach (var button in Buttons)
        {
            button.Draw(gameTime, spriteBatch);
        }
    }

    public void Dispose()
    {
        foreach (var button in Buttons)
        {
            button.Dispose();
        }
    }

    public void SetMenuButtons(List<MenuButton> buttons)
    {
        currentBtnIndex = 0;
        Buttons = buttons;
        currentBtnIndex = 0;
        foreach (var button in Buttons)
        {
            button.Highlighted = false;
        }
        HighlightChanged(true);
        UpdateMenuButtonsAlignement();
    }

    public void UpdateMenuButtonsAlignement()
    {
        var lastBtnBottomBorder = Position.Y + 25;

        foreach (var button in Buttons)
        {
            button.Position = new Point(Position.X + (((int)Size.X - button.Size.X) / 2), lastBtnBottomBorder);
            lastBtnBottomBorder += button.Size.Y;
        }
    }
    private void HighlightChanged(bool hightlight)
    {
        Buttons[currentBtnIndex].Highlighted = hightlight;
    }
}