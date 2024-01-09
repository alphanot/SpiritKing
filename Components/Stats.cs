using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SpiritKing.Components
{
    public class Stats
    {
        public float WalkSpeed { get; set; } = 0;
        public float RunSpeed { get; set; } = 0;

        public float Acceleration { get; set; } = 0;
        public float JumpStrength { get; set; } = 0;

        public float NormalAttackDamage { get; set; } = 0;
        public float HeavyAttackDamage { get; set; } = 0;

        public float MaxHealth { get; set; } = 0;
        public float MaxStamina { get; set; } = 0;
        public float HealthRegenSpeed { get; set; } = 0;
        public float StaminaRegenSpeed { get; set; } = 0;
        public float Health {  get; set; } = 0;
        public float Stamina { get; set; } = 0;

        public float Height { get; set; } = 0;
        public float Width { get; set; } = 0;
        public string Name { get; set; } = "";

        public Color Color { get; set; } = Color.White;
        public Stats() { }

    }
}
