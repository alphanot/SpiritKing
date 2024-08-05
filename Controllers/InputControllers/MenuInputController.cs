using Apos.Input;
using Microsoft.Xna.Framework.Input;

namespace SpiritKing.Controllers.InputControllers;
public class MenuInputController
{
    public readonly ICondition Up = new AnyCondition(
       new KeyboardCondition(Keys.Up),
       new GamePadStickCondition(GamePadStickCondition.GamePadStick.Left, GamePadStickCondition.StickAxis.Y, GamePadStickCondition.AxisDirection.Positive, 0),
       new GamePadCondition(GamePadButton.Up, 0)
   );

    public readonly ICondition Down = new AnyCondition(
        new KeyboardCondition(Keys.Down),
        new GamePadStickCondition(GamePadStickCondition.GamePadStick.Left, GamePadStickCondition.StickAxis.Y, GamePadStickCondition.AxisDirection.Negative, 0),
        new GamePadCondition(GamePadButton.Down, 0)
    );

    public readonly ICondition Left = new AnyCondition(
       new KeyboardCondition(Keys.Left),
       new GamePadStickCondition(GamePadStickCondition.GamePadStick.Left, GamePadStickCondition.StickAxis.X, GamePadStickCondition.AxisDirection.Negative, 0),
       new GamePadCondition(GamePadButton.Left, 0)
   );

    public readonly ICondition Right = new AnyCondition(
        new KeyboardCondition(Keys.Right),
        new GamePadStickCondition(GamePadStickCondition.GamePadStick.Left, GamePadStickCondition.StickAxis.X, GamePadStickCondition.AxisDirection.Positive, 0),
        new GamePadCondition(GamePadButton.Right, 0)
    );

    public readonly ICondition Select = new AnyCondition(
        new KeyboardCondition(Keys.Enter),
        new GamePadCondition(GamePadButton.A, 0)
    );

    public readonly ICondition Back = new AnyCondition(
        new KeyboardCondition(Keys.Escape),
        new KeyboardCondition(Keys.Back),
        new GamePadCondition(GamePadButton.B, 0)
    );
}
