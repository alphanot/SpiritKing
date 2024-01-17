using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritKing.Components.States;
using SpiritKing.Structs;
using System;
using System.Collections.Generic;

namespace SpiritKing.Components.Interfaces
{
    public interface IPosessable : INode
    {
        public abstract Stats Stats { get; set; }
        public PlayerState PlayerState { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public CollisionShape Collider { get; set; }
        public CollisionShape ColliderCheck { get; set; }
        public CollisionShape EnemyAIFieldOfView { get; set; }

        public CollisionShape PosessableCollider { get; set; }
        public List<CollisionShape> Colliders { get; set; }
        public Texture2D Sprite { get; set; }

        public bool IsPosessed { get; set; }

        public Line PosessRay { get; set; }

        public static event Action<IPosessable> AttemptPossess;

        public static event Action<IPosessable> PosessableSwitched;

        public static event Action<IPosessable> GetPosessableCollidable;

        public static event Action<Attack> PlayerAttacked;

        public Attack NormalAttack { get; set; }

        public float posessTimerValue { get; set; }

        public bool IsHighlighted { get; set; }

        public bool PosessIsReady();

        public bool CanBePosessed();

        public void Posess();

        public void Unposess();
    }
}