using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;

namespace SpiritKing.Controllers
{
    public static class InputController
    {
        public enum InputMode
        {
            MenuController,
            PlayerController
        }

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

        public static bool IsPressed(Buttons button)
        {
            if (IsReady)
            {
                return currentGamePadState.IsButtonDown(button);
            }
            else
            {
                return false;
            }
        }

        public static bool IsFirstPress(Buttons button)
        {
            if (IsReady)
            {
                return currentGamePadState.IsButtonDown(button) && !previousGamePadState.IsButtonDown(button);
            }
            else
            {
                return false;
            }
        }

        public static float GetLeftStickX()
        {
            if (IsReady)
            {
                return currentGamePadState.ThumbSticks.Left.X;
            }
            else
            {
                return 0;
            }
        }

        public static float GetLeftStickY()
        {
            if (IsReady)
            {
                return currentGamePadState.ThumbSticks.Left.Y;
            }
            else
            {
                return 0;
            }
        }

        public static float GetRightStickX()
        {
            if (IsReady)
            {
                return currentGamePadState.ThumbSticks.Right.X;
            }
            else
            {
                return 0;
            }
        }

        public static float GetRightStickY()
        {
            if (IsReady)
            {
                return currentGamePadState.ThumbSticks.Right.Y;
            }
            else
            {
                return 0;
            }
        }

        public static bool GetRightStickPastDeadzone()
        {
            if (IsReady)
            {
                return (Math.Abs(currentGamePadState.ThumbSticks.Right.Y) > RightStickDeadzone) || (Math.Abs(currentGamePadState.ThumbSticks.Right.X) > RightStickDeadzone);
            }
            return false;
        }

        public static bool GetLeftStickPastDeadzone()
        {
            if (IsReady)
            {
                return (Math.Abs(currentGamePadState.ThumbSticks.Left.Y) > LeftStickDeadzone) || (Math.Abs(currentGamePadState.ThumbSticks.Left.X) > LeftStickDeadzone);
            }
            return false;
        }

        public static void RumbleController(bool IsRumbling)
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

        public static void BumpController()
        {
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
            new Timer(x => StopRumble(), null, 1, Timeout.Infinite);
        }

        public static void StopRumble()
        {
            GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
        }
    }
}