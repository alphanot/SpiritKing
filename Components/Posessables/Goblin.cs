using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpiritKing.Components.Posessables
{
    internal class Goblin : Posessable
    {
        public override Attack NormalAttack { get; set; }

        public Goblin(Game game, bool isPosesed = false) : base(game, new Stats()
        {
            WalkSpeed = 175,
            RunSpeed = 400,
            Acceleration = 25,
            JumpStrength = -600,
            NormalAttackDamage = 10,
            HeavyAttackDamage = 17.5F,
            MaxHealth = 100,
            MaxStamina = 170,
            HealthRegenSpeed = 2.5F,
            StaminaRegenSpeed = 43.7F,
            Height = 64,
            Width = 48,
            Name = "Goblin",
            Health = 100,
            Stamina = 170,
            Color = Color.DarkOliveGreen,

        }, isPosesed) {
            NormalAttack = new Attack(new CollisionShape(Position.X, Position.Y, 100, 10), 0.2f, 0.2f, 10, 30, 10);
        }
    }
}
