using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;

namespace SpiritKing.Controllers;

public static class InputController
{
    public enum GameState
    {
        Game,
        Menu
    }

    public enum InputMode
    {
        MenuController,
        PlayerController
    }

    private static GameState _gameState;

    private static GamePadState currentGamePadState;
    private static GamePadState previousGamePadState;
    public static bool IsReady { get; set; } = false;

    public static InputMode CurrentInputMode { get; set; }

    public static float RightStickDeadzone { get; set; } = 0.15f;
    public static float LeftStickDeadzone { get; set; } = 0.15f;

    public static GamePadState GetState()
    {
        previousGamePadState = currentGamePadState;
        currentGamePadState = GamePad.GetState(PlayerIndex.One);
        return currentGamePadState;
    }

    public static bool IsPressed(Buttons button, GameState state)
    {
        return state == _gameState && IsReady && currentGamePadState.IsButtonDown(button);
    }

    public static bool IsFirstPress(Buttons button, GameState state)
    {
        return state == _gameState
            && IsReady
            && currentGamePadState.IsButtonDown(button)
            && !previousGamePadState.IsButtonDown(button);
    }

    public static float GetLeftStickX(GameState state)
    {
        return state != _gameState ? 0 : IsReady ? currentGamePadState.ThumbSticks.Left.X : 0;
    }

    public static float GetLeftStickY(GameState state)
    {
        return state != _gameState ? 0 : IsReady ? currentGamePadState.ThumbSticks.Left.Y : 0;
    }

    public static float GetRightStickX(GameState state)
    {
        return state != _gameState ? 0 : IsReady ? currentGamePadState.ThumbSticks.Right.X : 0;
    }

    public static float GetRightStickY(GameState state)
    {
        return state != _gameState ? 0 : IsReady ? currentGamePadState.ThumbSticks.Right.Y : 0;
    }

    public static bool GetRightStickPastDeadzone(GameState state)
    {
        return state == _gameState
            && IsReady
            && ((Math.Abs(currentGamePadState.ThumbSticks.Right.Y) > RightStickDeadzone) || (Math.Abs(currentGamePadState.ThumbSticks.Right.X) > RightStickDeadzone));
    }

    public static bool GetLeftStickPastDeadzone(GameState state)
    {
        return state == _gameState
            && IsReady
            && ((Math.Abs(currentGamePadState.ThumbSticks.Left.Y) > LeftStickDeadzone) || (Math.Abs(currentGamePadState.ThumbSticks.Left.X) > LeftStickDeadzone));
    }

    public static void RumbleController(bool IsRumbling, GameState state)
    {
        if (state != _gameState)
        {
            if (IsRumbling)
            {
                GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
            }
            else
            {
                GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
            }
        }
    }

    public static void BumpController(GameState state)
    {
        if (state != _gameState)
        {
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
            _ = new Timer(x => StopRumble(state), null, 1, Timeout.Infinite);
        }
    }

    public static void StopRumble(GameState state)
    {
        if (state != _gameState)
        {
            GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
        }
    }

    public static void SetGameState(GameState gameState) => _gameState = gameState;

    public static GameState GetGameState() => _gameState;
}