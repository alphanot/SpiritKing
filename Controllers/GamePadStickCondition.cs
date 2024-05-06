using Apos.Input;
using System;
namespace SpiritKing.Controllers;

public class GamePadStickCondition : ICondition
{
    public enum GamePadStick { Left, Right }
    public enum AxisDirection { Positive, Negative }
    public enum StickAxis { X, Y }

    public static float Deadzone { get; set; } = 0.1f;

    private readonly GamePadStick _gamePadStick;
    private readonly StickAxis _stickAxis;
    private readonly AxisDirection _direction;
    private readonly int _gamePadIndex;

    public GamePadStickCondition(GamePadStick gamePadStick, StickAxis stickAxis, AxisDirection direction, int gamePadIndex, float? deadzone = null)
    {
        _gamePadStick = gamePadStick;
        _stickAxis = stickAxis;
        _gamePadIndex = gamePadIndex;
        _direction = direction;
        Deadzone = deadzone == null ? 0.1f : deadzone.Value;
    }

    public bool Pressed(bool canConsume = true) => Pressed(_gamePadStick, _direction, _stickAxis, _gamePadIndex) && InputHelper.IsActive;

    public bool Held(bool canConsume = true) => Held(_gamePadStick, _direction, _stickAxis, _gamePadIndex) && InputHelper.IsActive;

    public bool HeldOnly(bool canConsume = true) => HeldOnly(_gamePadStick, _direction, _stickAxis, _gamePadIndex) && InputHelper.IsActive;
    /// <summary>
    /// Works only some of the time
    /// </summary>
    public bool Released(bool canConsume = true) => Released(_gamePadStick, _direction, _stickAxis, _gamePadIndex) && InputHelper.IsActive;

    public void Consume() { }

    public static bool Pressed(GamePadStick gamePadStick, AxisDirection direction, StickAxis stickAxis, int gamePadIndex)
    {
        var newValue = GetNewValue(gamePadStick, stickAxis, gamePadIndex);
        var oldValue = GetOldValue(gamePadStick, stickAxis, gamePadIndex);
        var newIsPastDeadzone = Math.Abs(newValue) >= Deadzone;
        var oldIsBeforeDeadzone = Math.Abs(oldValue) < Deadzone;
        var isCorrectDirection = direction == AxisDirection.Negative ? newValue < 0 : newValue > 0;

        return newIsPastDeadzone && isCorrectDirection && oldIsBeforeDeadzone;
    }

    public static bool Held(GamePadStick gamePadStick, AxisDirection direction, StickAxis stickAxis, int gamePadIndex)
    {
        var newValue = GetNewValue(gamePadStick, stickAxis, gamePadIndex);
        var isPastDeadzone = Math.Abs(newValue) >= Deadzone;
        var isCorrectDirection = direction == AxisDirection.Negative ? newValue < 0 : newValue > 0;

        return isPastDeadzone && isCorrectDirection;
    }

    public static bool HeldOnly(GamePadStick gamePadStick, AxisDirection direction, StickAxis stickAxis, int gamePadIndex)
    {
        var newValue = GetNewValue(gamePadStick, stickAxis, gamePadIndex);
        var oldValue = GetOldValue(gamePadStick, stickAxis, gamePadIndex);

        var newIsPastDeadzone = Math.Abs(newValue) >= Deadzone;
        var oldIsPastDeadzone = Math.Abs(oldValue) >= Deadzone;

        var isCorrectDirection = direction == AxisDirection.Negative ? newValue < 0 : newValue > 0;

        return newIsPastDeadzone && isCorrectDirection && oldIsPastDeadzone;
    }
    /// <summary>
    /// Works only some of the time
    /// </summary>
    public static bool Released(GamePadStick gamePadStick, AxisDirection direction, StickAxis stickAxis, int gamePadIndex)
    {
        var newValue = GetNewValue(gamePadStick, stickAxis, gamePadIndex);
        var oldValue = GetOldValue(gamePadStick, stickAxis, gamePadIndex);
        bool newIsBeforeDeadzone;
        bool oldIsPastDeadzone;
        if (direction == AxisDirection.Positive)
        {
            newIsBeforeDeadzone = newValue < Deadzone;
            oldIsPastDeadzone = oldValue >= Deadzone;
        }
        else
        {
            newIsBeforeDeadzone = newValue > -Deadzone;
            oldIsPastDeadzone = oldValue <= -Deadzone;
        }

        var isCorrectDirection = direction == AxisDirection.Negative ? newValue < 0 : newValue > 0;

        return oldIsPastDeadzone && newIsBeforeDeadzone && isCorrectDirection;
    }

    private static float GetOldValue(GamePadStick gamePadStick, StickAxis stickAxis, int gamePadIndex)
    {
        Microsoft.Xna.Framework.Vector2 oldStick = gamePadStick == GamePadStick.Left ?
            InputHelper.OldGamePad[gamePadIndex].ThumbSticks.Left :
            InputHelper.OldGamePad[gamePadIndex].ThumbSticks.Right;

        return stickAxis == StickAxis.X ? oldStick.X : oldStick.Y;
    }

    private static float GetNewValue(GamePadStick gamePadStick, StickAxis stickAxis, int gamePadIndex)
    {
        Microsoft.Xna.Framework.Vector2 newStick = gamePadStick == GamePadStick.Left ?
            InputHelper.NewGamePad[gamePadIndex].ThumbSticks.Left :
            InputHelper.NewGamePad[gamePadIndex].ThumbSticks.Right;

        return stickAxis == StickAxis.X ? newStick.X : newStick.Y;
    }
}
