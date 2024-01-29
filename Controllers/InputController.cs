using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;

namespace SpiritKing.Controllers
{
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

        public static float RightStickDeadzone { get; set; } = 0.1f;
        public static float LeftStickDeadzone { get; set; } = 0.1f;

        public static GamePadState GetState()
        {
            previousGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            return currentGamePadState;
        }

        public static bool IsPressed(Buttons button, GameState state)
        {
            if(state != _gameState)
            {
                return false;
            }
            if (IsReady)
            {
                return currentGamePadState.IsButtonDown(button);
            }
            else
            {
                return false;
            }
        }

        public static bool IsFirstPress(Buttons button, GameState state)
        {
            if (state != _gameState)
            {
                return false;
            }
            if (IsReady)
            {
                return currentGamePadState.IsButtonDown(button) && !previousGamePadState.IsButtonDown(button);
            }
            else
            {
                return false;
            }
        }

        public static float GetLeftStickX(GameState state)
        {
            if (state != _gameState)
            {
                return 0;
            }
            if (IsReady)
            {
                return currentGamePadState.ThumbSticks.Left.X;
            }
            else
            {
                return 0;
            }
        }

        public static float GetLeftStickY(GameState state)
        {
            if (state != _gameState)
            {
                return 0;
            }
            if (IsReady)
            {
                return currentGamePadState.ThumbSticks.Left.Y;
            }
            else
            {
                return 0;
            }
        }

        public static float GetRightStickX(GameState state)
        {
            if (state != _gameState)
            {
                return 0;
            }
            if (IsReady)
            {
                return currentGamePadState.ThumbSticks.Right.X;
            }
            else
            {
                return 0;
            }
        }

        public static float GetRightStickY(GameState state)
        {
            if (state != _gameState)
            {
                return 0;
            }
            if (IsReady)
            {
                return currentGamePadState.ThumbSticks.Right.Y;
            }
            else
            {
                return 0;
            }
        }

        public static bool GetRightStickPastDeadzone(GameState state)
        {
            if (state != _gameState)
            {
                return false;
            }
            if (IsReady)
            {
                return (Math.Abs(currentGamePadState.ThumbSticks.Right.Y) > RightStickDeadzone) || (Math.Abs(currentGamePadState.ThumbSticks.Right.X) > RightStickDeadzone);
            }
            return false;
        }

        public static bool GetLeftStickPastDeadzone(GameState state)
        {
            if (state != _gameState)
            {
                return false;
            }
            if (IsReady)
            {
                return (Math.Abs(currentGamePadState.ThumbSticks.Left.Y) > LeftStickDeadzone) || (Math.Abs(currentGamePadState.ThumbSticks.Left.X) > LeftStickDeadzone);
            }
            return false;
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
                new Timer(x => StopRumble(state), null, 1, Timeout.Infinite);
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
}