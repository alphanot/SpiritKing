using Microsoft.Xna.Framework;
using SpiritKing.Controllers;

namespace SpiritKing.Components.Posessables;

internal class Hound : Posessable
{
    public override Attack NormalAttack { get; set; }

    public Hound(Game game, Vector2 position, GameWorldHandler gameWorld, bool isPosesed = false) : base(game, new Stats()
    {
        WalkSpeed = 205,
        RunSpeed = 500,
        Acceleration = 37,
        JumpStrength = -520,
        NormalAttackDamage = 10,
        HeavyAttackDamage = 17.5F,
        MaxHealth = 100,
        MaxStamina = 210,
        HealthRegenSpeed = 2.5F,
        StaminaRegenSpeed = 52F,
        Height = 44,
        Width = 88,
        Name = "Hound",
        Health = 100,
        Stamina = 210,
        Color = Color.LightSlateGray,
    }, gameWorld, isPosesed)
    {
        Position = position;
        NormalAttack = new Attack(new CollisionShape(Position.X, Position.Y, 10, 44), 0.2f, 0.2f, 10, 30, 10);
    }
}