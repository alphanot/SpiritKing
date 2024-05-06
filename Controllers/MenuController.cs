using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritKing.Components.Interfaces;
using SpiritKing.Components.Nodes;
using SpiritKing.Controllers.InputControllers;
using System.Collections.Generic;

namespace SpiritKing.Controllers;

public class MenuController : Components.Interfaces.IUpdateable, Components.Interfaces.IDrawable
{
    public MenuButton[] Buttons;
    private int currentBtnIndex;

    public int DrawOrder => 1;

    public bool Enabled => true;

    public int UpdateOrder => 1;

    public bool Visible => true;

    private readonly MenuInputController _menuInputController = new();
    public MenuController(MenuButton[] buttons)
    {
        Buttons = buttons;
        currentBtnIndex = 0;
    }

    public void Update(GameTime gameTime)
    {
        if (_menuInputController.Down.Pressed())
        {
            HighlightChanged(false);
            currentBtnIndex++;
            if (currentBtnIndex >= Buttons.Length)
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
                currentBtnIndex = Buttons.Length - 1;
            }
            HighlightChanged(true);
        }

        if (_menuInputController.Select.Pressed())
        {
            Buttons[currentBtnIndex].Action?.Invoke();
        }
    }

    private void HighlightChanged(bool hightlight)
    {
        Buttons[currentBtnIndex].Highlighted = hightlight;
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
}