using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritKing.Components;
using SpiritKing.Components.Interfaces;

namespace SpiritKing.Controllers
{
    public class MenuController : INode
    {
        public MenuButton[] Buttons;
        private int currentBtnIndex;

        public int DrawOrder => 1;

        public MenuController(Game game, MenuButton[] buttons)
        {
            Buttons = buttons;
            currentBtnIndex = 0;
        }

        public void Update(GameTime gameTime)
        {
            InputController.GetState();
            if (InputController.IsFirstPress(Microsoft.Xna.Framework.Input.Buttons.DPadDown, InputController.GameState.Menu))
            {
                HighlightChanged(false);
                currentBtnIndex++;
                if (currentBtnIndex >= Buttons.Length)
                {
                    currentBtnIndex = 0;
                }
                HighlightChanged(true);
            }
            else if (InputController.IsFirstPress(Microsoft.Xna.Framework.Input.Buttons.DPadUp, InputController.GameState.Menu))
            {
                HighlightChanged(false);
                currentBtnIndex--;
                if (currentBtnIndex < 0)
                {
                    currentBtnIndex = Buttons.Length - 1;
                }
                HighlightChanged(true);
            }

            if (InputController.IsFirstPress(Microsoft.Xna.Framework.Input.Buttons.A, InputController.GameState.Menu))
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
}