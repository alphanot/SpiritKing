using Microsoft.Xna.Framework.Input;
using SpiritKing.Controllers;
using System;

namespace SpiritKing.Components;

public class Attack
{
    public CollisionShape AttackCollisionShape { get; set; }

    public float AttackDuration { get; private set; }

    public float AttackDurationCounter { get; set; } = 0;

    public float AttackCooldown { get; private set; }

    public float AttackCooldownCounter { get; set; } = 0;

    public int BaseDamage { get; set; }

    public float StaminaDrain { get; set; }

    public float KnockBack { get; set; }

    public bool IsActive { get; set; } = false;

    public bool IsReady { get; set; } = true;

    public bool IsCoolingDown { get; set; } = false;

    public Attack(CollisionShape collisionShape, float attackSpeed, float attackCooldown, int baseDamage, float staminaDrain, float knockBack)
    {
        AttackCollisionShape = collisionShape;
        AttackDuration = attackSpeed;
        AttackCooldown = attackCooldown;
        BaseDamage = baseDamage;
        KnockBack = knockBack;
        StaminaDrain = staminaDrain;
    }

    public float Update(float seconds, Action<Attack> func, bool isExausted, Buttons btn)
    {
        if (IsActive)
        {
            AttackDurationCounter -= seconds;
        }

        if (AttackDurationCounter < 0 && IsActive)
        {
            IsCoolingDown = true;
            AttackCooldownCounter = AttackCooldown;
            IsActive = false;
        }

        if (IsCoolingDown)
        {
            AttackCooldownCounter -= seconds;
        }

        if (AttackCooldownCounter < 0 && IsCoolingDown)
        {
            IsReady = true;
            IsCoolingDown = false;
        }

        if (InputController.IsPressed(btn, InputController.GameState.Game) && !isExausted && IsReady && !IsCoolingDown)
        {
            AttackDurationCounter = AttackDuration;
            IsActive = true;
            IsReady = false;
            IsCoolingDown = false;
            func?.Invoke(this);
            return StaminaDrain;
        }
        else
        {
            return 0;
        }
    }
}