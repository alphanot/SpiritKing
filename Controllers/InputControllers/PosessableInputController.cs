using Apos.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using SpiritKing.Utils;
using System.Reflection.Metadata.Ecma335;

namespace SpiritKing.Controllers.InputControllers;

public class PosessableInputController
{
    public static int PlayerIndex { get; set; }

    public PosessableInputController() => PlayerIndex = 0;
    public PosessableInputController(int playerIndex) => PlayerIndex = playerIndex;

    public readonly ICondition MoveLeft = new AnyCondition(
        new KeyboardCondition(Keys.Left),
        new GamePadStickCondition(GamePadStickCondition.GamePadStick.Left, GamePadStickCondition.StickAxis.X, GamePadStickCondition.AxisDirection.Negative, PlayerIndex)
    );

    public readonly ICondition MoveRight = new AnyCondition(
        new KeyboardCondition(Keys.Right),
        new GamePadStickCondition(GamePadStickCondition.GamePadStick.Left, GamePadStickCondition.StickAxis.X, GamePadStickCondition.AxisDirection.Positive, PlayerIndex)
    );

    public readonly ICondition Run = new AnyCondition(
        new KeyboardCondition(Keys.LeftShift),
        new GamePadCondition(GamePadButton.LeftShoulder, PlayerIndex)
    );

    public readonly ICondition Jump = new AnyCondition(
        new KeyboardCondition(Keys.Space),
        new GamePadCondition(GamePadButton.A, PlayerIndex)
    );

    public readonly ICondition Posess = new AnyCondition(
        new KeyboardCondition(Keys.Q),
        new GamePadCondition(GamePadButton.RightShoulder, PlayerIndex)
    );

    public readonly ICondition NormalAttack = new AnyCondition(
        new KeyboardCondition(Keys.W),
        new GamePadCondition(GamePadButton.X, PlayerIndex)
    );

    public readonly ICondition HeavyAttack = new AnyCondition(
        new KeyboardCondition(Keys.E),
        new GamePadCondition(GamePadButton.Y, PlayerIndex)
    );

    public Vector2 GetLeftStickValue() => InputHelper.NewGamePad[PlayerIndex].ThumbSticks.Left;

    public Vector2 GetRightStickValue() => InputHelper.NewGamePad[PlayerIndex].ThumbSticks.Right;

    public bool IsRightStickMoving()
    {
        return System.Math.Abs(InputHelper.NewGamePad[PlayerIndex].ThumbSticks.Right.X) > 0.2f ||
            System.Math.Abs(InputHelper.NewGamePad[PlayerIndex].ThumbSticks.Right.Y) > 0.2f;
    }
}
