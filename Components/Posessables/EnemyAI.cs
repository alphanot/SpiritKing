using Newtonsoft.Json.Linq;
using SpiritKing.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpiritKing.Components.Posessables;
public class EnemyAI : IDisposable
{
    private bool disposedValue;

    // -1(left) to 1(right) mocking left thumbstick x axis
    public float MovementX { get; set; }
    // mocking A button
    public JumpState Jump { get; set; } = JumpState.Released;
    // mocking left bumper button
    public bool RunActivated { get; set; }

    public bool HeavyAttackActivated { get; set; }
    public bool NormalAttackActivated { get; set; }
    public virtual CollisionShape EnemyAIFieldOfView { get; set; }

    public float PauseBeforeNormalAttack { get; set; } = 0.4f;
    public float PauseBeforeHeavyAttack { get; set; } = 1f;

    public GameTimeDelay DelayNormalAttack;
    public GameTimeDelay StopNormalAttack;

    public Thread AttackThread { get; }

    public enum JumpState
    {
        Held,
        Pressed,
        Released
    }

    public EnemyAI()
    {
        DelayNormalAttack = new GameTimeDelay(NormalAttack, PauseBeforeNormalAttack);
        StopNormalAttack = new GameTimeDelay(NormalAttackStopped, 1f);
    }
    private void NormalAttack()
    {
        NormalAttackActivated = true;
        StopNormalAttack.Start();
    }

    private void NormalAttackStopped()
    {
        if (NormalAttackActivated)
        {
            NormalAttackActivated = false;
        }
    }
    /// call one of these functions at every update loop
    public void PressJump() => Jump = Jump == JumpState.Released ? JumpState.Pressed : JumpState.Held;
    public void ReleaseJump() => Jump = JumpState.Released;
    ///

    public virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {

                EnemyAIFieldOfView.Dispose();
                EnemyAIFieldOfView = null;
            }
            disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
