using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Threading;

namespace SpiritKing.Utils;
public static class ControllerUtils
{

    public static void StartRumble(PlayerIndex controllerIndex) => GamePad.SetVibration(controllerIndex, 1.0f, 1.0f);

    public static void BumpController(PlayerIndex controllerIndex)
    {
        GamePad.SetVibration(controllerIndex, 1.0f, 1.0f);
        _ = new Timer(x => StopRumble(controllerIndex), null, 1, Timeout.Infinite);
    }

    public static void StopRumble(PlayerIndex controllerIndex) => GamePad.SetVibration(controllerIndex, 0f, 0f);
}
