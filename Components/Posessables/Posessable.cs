using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using SpiritKing.Components.Interfaces;
using SpiritKing.Components.States;
using SpiritKing.Controllers;
using SpiritKing.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpiritKing.Components.Posessables;

public class Posessable : IPosessable
{
    public int DrawOrder => 1;
    public bool IsHighlighted { get; set; } = false;
    public Stats Stats { get; set; }
    public virtual PlayerState PlayerState { get; set; }
    public virtual Vector2 Position { get; set; }
    public virtual Vector2 Velocity { get; set; }
    public virtual CollisionShape Collider { get; set; }
    public virtual CollisionShape ColliderCheck { get; set; }
    public virtual CollisionShape EnemyAIFieldOfView { get; set; }

    public virtual CollisionShape PosessableCollider { get; set; }
    public virtual List<CollisionShape> Colliders { get; set; }
    public Game Game { get; private set; }
    public Texture2D Sprite { get; set; }

    public static event Action<Posessable> PosessableDied;

    public static event Action<IPosessable> AttemptPossess;

    public static event Action<IPosessable> PosessableSwitched;

    public static event Action<IPosessable> GetPosessableCollidable;

    public static event Action<Attack> PlayerAttacked;

    public static event Action<float> UpdateHealthBar;

    public static event Action<float, bool> UpdateStaminaBar;

    public static event Action<bool> UpdatePosessCanActivate;

    public const float POSESS_TIMER_WAIT_TIME = 3f;

    public float PosessTimerValue { get; set; }
    public bool IsPosessed { get; set; } = false;
    public Line PosessRay { get; set; }

    public virtual Attack NormalAttack { get; set; }

    private Vector2 NextPosition { get; set; }

    private ParticleController _particleController;
    private CollisionShape _wallCollider;
    private CollisionShape _groundCollider;
    private CollisionShape _ceilingCollider;

    private OutlineRectF _outlineRect;

    private RectangleF _canGroundRay;
    private RectangleF _canShiftLeftRay;
    private RectangleF _canShiftRightRay;
    private bool jumpCanActivate = true;

    private GameTime _exhaustionTimer;
    private GameTime _posessCooldownTimer;

    private readonly float _coyoteTime = 0.2f;
    private float _coyoteTimeCounter;
    private readonly float _jumpBufferTime = 0.2f;
    private float _jumpBufferCounter;

    private EnemyHealthBar _enemyHealthBar;

    protected Posessable(Game game, Stats stats, bool isPosessed = false)
    {
        IsPosessed = isPosessed;
        Stats = stats;
        Game = game;
        Sprite = new Texture2D(game.GraphicsDevice, 1, 1);
        Sprite.SetData(new[] { Color.White });
        _outlineRect = new OutlineRectF(Sprite, Position.X, Position.Y, Stats.Width, Stats.Height, 4);

        PlayerState = new PlayerState();
        Position = new Vector2(100, 100);
        Velocity = new Vector2(0, 0);
        Collider = new CollisionShape(Position.X, Position.Y, Stats.Width, Stats.Height);

        ColliderCheck = new CollisionShape(Position.X - 100, Position.Y - 100, Stats.Width + 200, Stats.Height + 200);
        Colliders = new List<CollisionShape>();
        PosessableCollider = new CollisionShape(Position.X - 100, Position.Y - 100, Stats.Width + 200, Stats.Height + 200);
        EnemyAIFieldOfView = new CollisionShape(Position.X - 400, Position.Y - 400, Stats.Width + 800, Stats.Height + 800);
        Platform.GetCollidable += Signal_GetCollidable;
        PlayerAttacked += Signal_PlayerAttacked;
        _canGroundRay = new RectangleF(new Vector2(Position.X - 10, Position.Y + (Stats.Height * 0.8F)), new Vector2(Stats.Width + 20, 1));
        _canShiftLeftRay = new RectangleF(new Vector2(Position.X + (Stats.Width * 0.8F), Position.Y - 25), new Vector2(1, Stats.Height));
        _canShiftRightRay = new RectangleF(new Vector2(Position.X + (Stats.Width * 0.2F), Position.Y - 25), new Vector2(1, Stats.Height));
        PosessRay = new Line(new Point2(Position.X + (Stats.Width / 2), Position.Y + (Stats.Height / 2)), new Point2(400, 1));
        _exhaustionTimer = new GameTime();
        _posessCooldownTimer = new GameTime();

        _enemyHealthBar = new EnemyHealthBar(game, Stats.Health, Stats.MaxHealth, (int)Stats.Width);

        _particleController = new ParticleController(game.GraphicsDevice, new Vector2(0, 0));

        _particleController.AddEmitter(
            new ParticleEmitter(_particleController.TextureRegion, 100, TimeSpan.FromSeconds(0.5),
                Profile.Line(new Vector2(1, 0), Stats.Width))
            {
                Parameters = new ParticleReleaseParameters
                {
                    Speed = new Range<float>(0f, 10f),
                    Quantity = 1,
                    Rotation = new Range<float>(-1f, 1f),
                    Scale = new Range<float>(1.0f, 5.0f)
                },
                Modifiers =
                    {
                        new AgeModifier
                        {
                            Interpolators =
                            {
                                new ColorInterpolator
                                {
                                    StartValue = new HslColor(0.0f, 0.0f, 0.9f),
                                    EndValue = new HslColor(0.0f, 0.0f, 1.0f)
                                }
                            }
                        },
                        new RotationModifier {RotationRate = -2.1f},
                        new RectangleContainerModifier {Width = 800, Height = 480},
                        new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 50f},
                    }
            });
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Sprite, Position, Collider.Shape.ToRectangle(), Stats.Color);
        _particleController.Draw(gameTime, spriteBatch);
        if (IsPosessed)
        {
            spriteBatch.Draw(Sprite, PosessRay.Target, new Rectangle(0, 0, 2, 2), Color.Green);
        }
        else
        {
            _enemyHealthBar.Draw(gameTime, spriteBatch);
        }

        if (IsHighlighted)
        {
            _outlineRect.Draw(spriteBatch);
        }

        if (NormalAttack.IsActive)
        {
            spriteBatch.Draw(Sprite, NormalAttack.AttackCollisionShape.Shape.Position, NormalAttack.AttackCollisionShape.Shape.ToRectangle(), Color.White);
        }
    }

    public void Dispose()
    {
        _particleController.Dispose();
        _particleController = null;
        Platform.GetCollidable -= Signal_GetCollidable;
        PlayerAttacked -= Signal_PlayerAttacked;

        IsPosessed = false;
        Stats = null;
        Game = null;
        Sprite.Dispose();
        Sprite = null;
        _outlineRect.Dispose();
        _outlineRect = null;

        PlayerState = null;
        Position = Vector2.Zero;
        Position = Vector2.Zero;
        Velocity = Vector2.Zero;
        Collider.Dispose();
        Collider = null;

        EnemyAIFieldOfView.Dispose();
        EnemyAIFieldOfView = null;

        ColliderCheck = null;
        Colliders.Clear();
        Colliders = null;
        PosessableCollider.Dispose();
        PosessableCollider = null;

        _canGroundRay = RectangleF.Empty;
        _canShiftLeftRay = RectangleF.Empty;
        _canShiftRightRay = RectangleF.Empty;
        PosessRay = null;
        _exhaustionTimer = null;
        _posessCooldownTimer = null;

        _enemyHealthBar.Dispose();
        _enemyHealthBar = null;
    }

    public void Update(GameTime gameTime)
    {
        var seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Velocity = new Vector2(Velocity.X, Velocity.Y + GetGravity(seconds));
        NextPosition = Position;
        if (IsPosessed)
        {
            _posessCooldownTimer.ElapsedGameTime += gameTime.ElapsedGameTime;
            UpdatePosessCanActivate?.Invoke(_posessCooldownTimer.ElapsedGameTime.TotalSeconds > POSESS_TIMER_WAIT_TIME);
            HandleInput(seconds);
        }
        else
        {
            GetPosessableCollidable?.Invoke(this);
        }
        SetCollisions();
        HandleState(seconds);

        NextPosition += new Vector2(Velocity.X * seconds, Velocity.Y * seconds);

        MoveChildren(NextPosition);
        SetCollisions();

        UpdatePositionFromVelocity(seconds);
        MoveChildren(Position);

        HandleParticles(gameTime);
    }

    public bool PosessIsReady()
    {
        return _posessCooldownTimer.ElapsedGameTime.TotalSeconds > POSESS_TIMER_WAIT_TIME;
    }

    public bool CanBePosessed()
    {
        return Stats.Health < (Stats.MaxHealth / 2);
    }

    private void SetCollisions()
    {
        PlayerState.CollidingY = PlayerState.CollidingYState.None;
        PlayerState.CollidingX = PlayerState.CollidingXState.None;

        foreach (var collider in Colliders)
        {
            if (Collider.IsCollidingTop(collider) && !_canGroundRay.Intersects(collider.Shape))
            {
                PlayerState.CollidingY = PlayerState.CollidingYState.Ground;
                _groundCollider = collider;
            }
            else if (Collider.IsCollidingLeft(collider) &&
                !_canShiftLeftRay.Intersects(collider.Shape))
            {
                PlayerState.CollidingX = PlayerState.CollidingXState.Right;
                _wallCollider = collider;
            }
            else if (Collider.IsCollidingRight(collider) &&
                !_canShiftRightRay.Intersects(collider.Shape))
            {
                PlayerState.CollidingX = PlayerState.CollidingXState.Left;
                _wallCollider = collider;
            }
            else if (Collider.IsCollidingBottom(collider))
            {
                PlayerState.CollidingY = PlayerState.CollidingYState.Ceiling;
                _ceilingCollider = collider;
            }
        }
    }

    private static float GetGravity(float seconds)
    {
        return Globals.GRAVITY * seconds;
    }

    private void HandleInput(float seconds)
    {
        InputController.GetState();
        if (InputController.GetLeftStickX(InputController.GameState.Game) < 0)
        {
            PlayerState.MovementX = PlayerState.MovementStateX.MoveLeft;
        }
        else if (InputController.GetLeftStickX(InputController.GameState.Game) > 0)
        {
            PlayerState.MovementX = PlayerState.MovementStateX.MoveRight;
        }
        else
        {
            PlayerState.MovementX = Math.Abs(Velocity.X) > 10 ? PlayerState.MovementStateX.Slowing : PlayerState.MovementStateX.Idle;
        }

        PlayerState.IsRunning = InputController.IsPressed(Buttons.LeftShoulder, InputController.GameState.Game);

        if (PlayerState.CollidingY == PlayerState.CollidingYState.Ground)
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= seconds;
        }

        if (InputController.IsFirstPress(Buttons.A, InputController.GameState.Game))
        {
            _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= seconds;
        }

        if (InputController.IsPressed(Buttons.A, InputController.GameState.Game))
        {
            if (_jumpBufferCounter > 0)
            {
                jumpCanActivate = true;
            }
            if (jumpCanActivate && _coyoteTimeCounter > 0f)
            {
                jumpCanActivate = false;
                PlayerState.MovementY = PlayerState.MovementStateY.Jumped;
                _coyoteTimeCounter = 0;
                _jumpBufferCounter = 0;
            }
            else
            {
                PlayerState.MovementY = PlayerState.MovementStateY.Jumping;
            }
        }
        else
        {
            if (PlayerState.CollidingY == PlayerState.CollidingYState.Ground)
            {
                jumpCanActivate = true;
                PlayerState.MovementY = PlayerState.MovementStateY.Idle;
            }
            else
            {
                PlayerState.MovementY = PlayerState.MovementStateY.Falling;
            }
        }

        PosessRay.Target = new Point2((InputController.GetRightStickX(InputController.GameState.Game) * 300) + PosessRay.Position.X, -(InputController.GetRightStickY(InputController.GameState.Game) * 300) + PosessRay.Position.Y);

        Stats.Stamina -= NormalAttack.Update(seconds, PlayerAttacked, PlayerState.IsExhausted, Buttons.Y);
    }

    private void HandleState(float seconds)
    {
        var velocityDecay = Math.Sign(Velocity.X) * (10 * seconds);
        if (PlayerState.MovementY == PlayerState.MovementStateY.Jumped)
        {
            Velocity = new Vector2(Velocity.Y, Stats.JumpStrength);
            PlayerState.CollidingY = PlayerState.CollidingYState.None;
        }
        else if (PlayerState.MovementY == PlayerState.MovementStateY.Falling && Velocity.Y < 0)
        {
            Velocity = new Vector2(Velocity.X, Velocity.Y + GetGravity(seconds));
        }

        if (PlayerState.MovementX == PlayerState.MovementStateX.Idle)
        {
            Velocity = new Vector2(Velocity.X - velocityDecay, Velocity.Y);
        }
        else if (PlayerState.MovementX == PlayerState.MovementStateX.Slowing)
        {
            Velocity = new Vector2(Math.Sign(Velocity.X) * Math.Abs(Velocity.X) / (Stats.Acceleration / 2), Velocity.Y);
        }
        else if (PlayerState.MovementX == PlayerState.MovementStateX.MoveLeft &&
            PlayerState.CollidingX != PlayerState.CollidingXState.Left)
        {
            Velocity = new Vector2(Velocity.X - Stats.Acceleration, Velocity.Y);
            PlayerState.LastDirection = PlayerState.LastLookState.Left;
        }
        else if (PlayerState.MovementX == PlayerState.MovementStateX.MoveRight &&
            PlayerState.CollidingX != PlayerState.CollidingXState.Right)
        {
            Velocity = new Vector2(Velocity.X + Stats.Acceleration, Velocity.Y);
            PlayerState.LastDirection = PlayerState.LastLookState.Right;
        }
        else if (PlayerState.MovementX == PlayerState.MovementStateX.KnockedBack)
        {
            if (Math.Abs(Velocity.X) < 2)
            {
                PlayerState.MovementX = PlayerState.MovementStateX.Idle;
            }
            else
            {
                Velocity = new(Velocity.X - velocityDecay, Velocity.Y);
            }
        }

        if (Stats.Stamina < 0)
        {
            PlayerState.IsExhausted = true;
        }

        if (PlayerState.IsExhausted)
        {
            _exhaustionTimer.ElapsedGameTime += TimeSpan.FromSeconds(seconds);
        }

        if (_exhaustionTimer.ElapsedGameTime.TotalSeconds > 2.1)
        {
            PlayerState.IsExhausted = false;
            _exhaustionTimer.ElapsedGameTime = TimeSpan.Zero;
        }

        var speed = PlayerState.IsRunning && !PlayerState.IsExhausted ? Stats.RunSpeed : Stats.WalkSpeed;

        if (Math.Abs(Velocity.X) > speed)
        {
            Velocity = new Vector2(Math.Sign(Velocity.X) * speed, Velocity.Y);
        }

        if (Math.Abs(Velocity.X) > Globals.MAX_SPEED.X)
        {
            Velocity = new Vector2(Math.Sign(Velocity.X) * Globals.MAX_SPEED.X, Velocity.Y);
        }

        if (Math.Abs(Velocity.Y) > Globals.MAX_SPEED.Y)
        {
            Velocity = new Vector2(Math.Sign(Velocity.X) * Globals.MAX_SPEED.X, Math.Sign(Velocity.Y) * Globals.MAX_SPEED.Y);
        }

        if (PlayerState.IsRunning && !PlayerState.IsExhausted)
        {
            Stats.Stamina -= Stats.StaminaRegenSpeed * seconds;
        }
        else
        {
            Stats.Stamina += Stats.StaminaRegenSpeed * seconds;
            if (Stats.Stamina > Stats.MaxStamina)
            {
                Stats.Stamina = Stats.MaxStamina;
            }
        }
        if (IsPosessed)
        {
            UpdateStaminaBar?.Invoke(Stats.Stamina, PlayerState.IsExhausted);
        }
    }

    private void HandleParticles(GameTime gameTime)
    {
        _particleController.Update(gameTime);

        if (PlayerState.CollidingY == PlayerState.CollidingYState.Ground && Math.Abs(Velocity.X) > 20)
        {
            _particleController.SetQuantity(0, 2);
        }
        else
        {
            _particleController.SetQuantity(0, 0);
        }
    }

    private void UpdatePositionFromVelocity(float seconds)
    {
        //stand on ground or ceiling
        if (PlayerState.CollidingY == PlayerState.CollidingYState.Ground && Velocity.Y > 0)
        {
            Velocity = new Vector2(Velocity.X, 0);
            Position = new Vector2(Position.X, _groundCollider.Shape.Top - Collider.Shape.Height);
        }
        else if (PlayerState.CollidingY == PlayerState.CollidingYState.Ceiling)
        {
            Velocity = new Vector2(Velocity.X, 0);
            Position = new Vector2(Position.X, _ceilingCollider.Shape.Bottom);
        }

        // stick to wall
        switch (PlayerState.CollidingX)
        {
            case PlayerState.CollidingXState.Right:
                Position = new Vector2(_wallCollider.Shape.Left - Stats.Width, Position.Y);
                Velocity = new Vector2(0, Velocity.Y);
                break;

            case PlayerState.CollidingXState.Left:
                Position = new Vector2(_wallCollider.Shape.Right, Position.Y);
                Velocity = new Vector2(0, Velocity.Y);
                break;

            default:
                _wallCollider = null;
                break;
        }
        PlayerState.CollidingX = PlayerState.CollidingXState.None;

        Position += new Vector2(Velocity.X * seconds, Velocity.Y * seconds);
    }

    private void MoveChildren(Vector2 _position)
    {
        Collider.SetPosition(_position);
        ColliderCheck.SetPosition(new Vector2(Position.X - 100, Position.Y - 100));
        PosessableCollider.SetPosition(new Vector2(Position.X - 100, Position.Y - 100));
        _particleController.SetPosition(new Vector2(_position.X + (Stats.Width / 2), _position.Y + Stats.Height));
        _canGroundRay.Position = new Vector2(Position.X - 10, Position.Y + (Stats.Height * 0.8F));
        _canShiftLeftRay.Position = new Vector2(Position.X + (Stats.Width * 0.8F), Position.Y - 25);
        _canShiftRightRay.Position = new Vector2(Position.X + (Stats.Width * 0.2F), Position.Y - 25);
        PosessRay.Position = new Point2(Position.X + (Stats.Width / 2), Position.Y + (Stats.Height / 2));
        EnemyAIFieldOfView.SetPosition(new Vector2(Position.X - 400, Position.Y - 400));

        _enemyHealthBar.Position = new Vector2(Position.X, Position.Y - 10);

        if (PlayerState.LastDirection == PlayerState.LastLookState.Left)
        {
            NormalAttack.AttackCollisionShape.SetPosition(new Vector2(Position.X - NormalAttack.AttackCollisionShape.Shape.Width, Position.Y + (Stats.Height / 2)));
        }
        else if (PlayerState.LastDirection == PlayerState.LastLookState.Right)
        {
            NormalAttack.AttackCollisionShape.SetPosition(new Vector2(Position.X + Stats.Width, Position.Y + (Stats.Height / 2)));
        }
        // highlight
        _outlineRect.Position = _position;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="knockbackDirection"> which way this should be knocked back. Left to right with -1 being left and 1 being right.</param>
    private void ReceiveDamage(int damage, float knockbackDirection, float knockbackStrenght)
    {
        Stats.Health -= damage;
        if (IsPosessed)
        {
            UpdateHealthBar?.Invoke(Stats.Health);
        }
        else
        {
            _enemyHealthBar.SetCurrentHealth(Stats.Health);
        }
        Velocity = new(knockbackDirection * knockbackStrenght, Velocity.Y - knockbackStrenght);
        PlayerState.MovementX = PlayerState.MovementStateX.KnockedBack;

        if (Stats.Health < 1)
        {
            Die();
            IsPosessed = false;
        }
    }

    private void Die()
    {
        Debug.Print("Posessable.Die()");
        PosessableDied?.Invoke(this);
    }

    private void Signal_GetCollidable(CollisionShape shape)
    {
        Colliders.Remove(shape);
        if (ColliderCheck.IsColliding(shape))
        {
            Colliders.Add(shape);
        }
    }

    private void Signal_PlayerAttacked(Attack attack)
    {
        var attackPosition = attack.AttackCollisionShape.GetPosition();
        if (Collider.IsColliding(attack.AttackCollisionShape))
        {
            var knockbackDirection = attackPosition.X > Position.X ? -1 : (float)1;
            // attacker is on the right
            ReceiveDamage(attack.BaseDamage, knockbackDirection, attack.KnockBack);
        }
    }

    public void Posess()
    {
        IsHighlighted = false;
        IsPosessed = true;
        PosessRay.Target = PosessRay.Position;
        Velocity = new Vector2(0, Velocity.Y);
        PlayerState.MovementX = PlayerState.MovementStateX.Idle;
        _posessCooldownTimer.ElapsedGameTime = TimeSpan.Zero;
    }

    public void Unposess()
    {
        IsPosessed = false;
        PosessRay.Target = this.PosessRay.Position;
        Velocity = new Vector2(0, this.Velocity.Y);
        PlayerState.MovementX = PlayerState.MovementStateX.Idle;
    }
}