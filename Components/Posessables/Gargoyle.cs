using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpiritKing.Components.Posessables
{
    internal class Gargoyle : Posessable
    {
        public override Attack NormalAttack { get; set; }

        public Gargoyle(Game game, bool isPosesed = false) : base(game, new Stats()
        {
            WalkSpeed = 175,
            RunSpeed = 320,
            Acceleration = 25,
            JumpStrength = -800,
            NormalAttackDamage = 10,
            HeavyAttackDamage = 17.5F,
            MaxHealth = 100,
            MaxStamina = 170,
            HealthRegenSpeed = 2.5F,
            StaminaRegenSpeed = 43.7F,
            Height = 100,
            Width = 64,
            Name = "Gargoyle",
            Health = 100,
            Stamina = 170,
            Color = Color.DarkKhaki,

        }, isPosesed) {
            NormalAttack = new Attack(new CollisionShape(Position.X, Position.Y, 70, 80), 0.2f, 0.2f, 10, 30, 10);
        }
    }
}
