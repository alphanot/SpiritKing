namespace SpiritKing.Components.States
{
    public class PlayerState
    {
        public PlayerState()
        { }

        public MovementStateY MovementY { get; set; }
        public MovementStateX MovementX { get; set; }
        public CollidingXState CollidingX { get; set; }
        public CollidingYState CollidingY { get; set; }
        public LastLookState LastDirection { get; set; }

        public enum LastLookState
        {
            Left,
            Right
        }

        public bool IsCollidingTop { get; set; }

        public bool Grounded = false;
        public bool IsRunning = false;
        public bool IsExhausted = false;

        public enum MovementStateX
        {
            Idle,
            Slowing,
            MoveLeft,
            MoveRight,
            KnockedBack
        }

        public enum MovementStateY
        {
            Idle,
            Jumped,
            Jumping,
            Falling
        }

        public enum CollidingXState
        {
            None,
            Right,
            Left
        }

        public enum CollidingYState
        {
            None,
            Ground,
            Ceiling
        }
    }
}