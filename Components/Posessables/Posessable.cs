using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Tweening;
using SpiritKing.Components.Interfaces;
using SpiritKing.Components.Nodes;
using SpiritKing.Components.Nodes.Collectables;
using SpiritKing.Components.States;
using SpiritKing.Controllers;
using SpiritKing.Controllers.InputControllers;
using SpiritKing.Structs;
using SpiritKing.Utils;
using System;
using System.Collections.Generic;

namespace SpiritKing.Components.Posessables;

public abstract class Posessable : SpatialEntity, Interfaces.IDrawable, Interfaces.IUpdateable
{
    public int DrawOrder { get; set; } = 1;
    public bool IsHighlighted { get; set; } = false;
    public Stats Stats { get; set; }
    public PlayerState PlayerState { get; set; }

    public Vector2 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }
    Vector2 velocity;

    public RectangleF PosessableCollider { get; set; }
    public Texture2D Sprite { get; set; }

    public static event Action<Posessable>? PosessableDied; // TODO: possibly stupid(might be able to do it all through controller

    public static event Action<Attack>? PlayerAttacked; // TODO: stupid.

    public static event Action<float>? UpdateHealthBar; // TODO: should be name as an event from the context of this class, so something like "HealthChanged"

    public static event Action<float, bool>? UpdateStaminaBar;

    public static event Action<bool>? UpdatePosessCanActivate;

    public static event Func<ISpatialEntity, IEnumerable<ISpatialEntity>>? GetCurrentCollisions;
    public static event Action<ICollectable>? PickedUpCollectible;

    public const float POSESS_TIMER_WAIT_TIME = 3f;

    private IEnumerable<ISpatialEntity> _currentCollisions = [];

    public bool IsPosessed { get; set; } = false;
    public Line PosessRay { get; set; }

    public abstract Attack NormalAttack { get; set; }
    public PosessableInputController InputController { get; set; }

    public bool Enabled { get; set; } = true;

    public int UpdateOrder { get; set; } = 1;

    public bool Visible { get; set; } = true;

    private ParticleController _particleController;

    private OutlineRectF _outlineRect;

    private bool jumpCanActivate = true;

    private GameTime _exhaustionTimer;
    private GameTime _posessCooldownTimer;

    private readonly float _coyoteTime = 0.2f;
    private float _coyoteTimeCounter;
    private readonly float _jumpBufferTime = 0.2f;
    private float _jumpBufferCounter;

    private EnemyHealthBar _enemyHealthBar;

    private float _stunTimer;
    private readonly float _STUN_TIMER_MAX_TIME = 0.2f;
    private bool _stunned = false;

    public EnemyAI EnemyAI { get; set; }

    private DamageTextController DamageTextController { get; set; }

    protected Posessable(Game game, Stats stats, bool isPosessed = false)
    {
        DamageTextController = new DamageTextController(game);
        EnemyAI = new();
        InputController = new PosessableInputController();
        IsPosessed = isPosessed;
        Stats = stats;
        Sprite = new Texture2D(game.GraphicsDevice, 1, 1);
        Sprite.SetData(new[] { Color.White });
        _outlineRect = new OutlineRectF(Sprite, Position.X, Position.Y, Stats.Width, Stats.Height, 4);

        PlayerState = new PlayerState();
        Position = new Vector2(100, 100);
        Velocity = new Vector2(0, 0);

        Size = new(Stats.Width, Stats.Height);

        PosessableCollider = new RectangleF(Position.X - 100, Position.Y - 100, Stats.Width + 200, Stats.Height + 200);
        EnemyAI.EnemyAIFieldOfView = new RectangleF(Position.X - 400, Position.Y - 400, Stats.Width + 800, Stats.Height + 800);

        PlayerAttacked += Signal_PlayerAttacked;
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
        spriteBatch.Draw(Sprite, Position, Bounds, Stats.Color);
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
            _outlineRect.Draw(gameTime, spriteBatch);
        }

        if (NormalAttack.IsActive)
        {
            spriteBatch.Draw(Sprite, NormalAttack.Position, NormalAttack.Bounds, Color.White);
        }
        DamageTextController.Draw(gameTime, spriteBatch);

    }

    void IDisposable.Dispose()
    {
        _particleController.Dispose();
        PlayerAttacked -= Signal_PlayerAttacked;
        IsPosessed = false;
        Sprite.Dispose();
        _outlineRect.Dispose();
        Position = Vector2.Zero;
        Velocity = Vector2.Zero;
        EnemyAI.Dispose(true);
        PosessableCollider = RectangleF.Empty;
        _enemyHealthBar.Dispose();
        DamageTextController.Dispose();
    }

    public async void Update(GameTime gameTime)
    {
        DamageTextController.Update(gameTime);
        var seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (IsPosessed)
        {
            HandleInput(seconds);
        }
        else
        {
            HandleAIInput(seconds);
        }
        HandleState(seconds);
        HandleStamina(seconds);
        UpdatePositionFromVelocity(seconds);
        MoveChildren(Position);
        _currentCollisions = GetCurrentCollisions?.Invoke(this) ?? [];
        ResolveCollisions();
        MoveChildren(Position);
        if (IsPosessed)
        {
            _posessCooldownTimer.ElapsedGameTime += gameTime.ElapsedGameTime;
            UpdatePosessCanActivate?.Invoke(_posessCooldownTimer.ElapsedGameTime.TotalSeconds > POSESS_TIMER_WAIT_TIME);
        }
        HandleParticles(gameTime);
    }

    private void HandleInput(float seconds)
    {
        if (InputController.MoveLeft.Held())
        {
            PlayerState.MovementX = PlayerState.MovementStateX.MoveLeft;
        }
        else if (InputController.MoveRight.Held())
        {
            PlayerState.MovementX = PlayerState.MovementStateX.MoveRight;
        }
        else
        {
            PlayerState.MovementX = Math.Abs(Velocity.X) > 10 ? PlayerState.MovementStateX.Slowing : PlayerState.MovementStateX.Idle;
        }

        PlayerState.IsRunning = InputController.Run.Held();

        if (InputController.Jump.Pressed())
        {
            _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= seconds;
        }

        if (PlayerState.CollidingY == PlayerState.CollidingYState.Ground)
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= seconds;
        }

        if (InputController.Jump.Held())
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

        PosessRay.Target = new Point2((InputController.GetRightStickValue().X * 300) + PosessRay.Position.X, -(InputController.GetRightStickValue().Y * 300) + PosessRay.Position.Y);
        //Todo: This sucks. Don't put this kind of logic in this function
        Stats.Stamina -= NormalAttack.Update(seconds, PlayerAttacked, PlayerState.IsExhausted, InputController.NormalAttack.Pressed());
    }

    private void HandleAIInput(float seconds)
    {
        if (!_stunned)
        {
            if (EnemyAI.MovementX < 0)
            {
                PlayerState.MovementX = PlayerState.MovementStateX.MoveLeft;
            }
            else if (EnemyAI.MovementX > 0)
            {
                PlayerState.MovementX = PlayerState.MovementStateX.MoveRight;
            }
            else
            {
                PlayerState.MovementX = Math.Abs(Velocity.X) > 10 ? PlayerState.MovementStateX.Slowing : PlayerState.MovementStateX.Idle;
            }
        }
        else
        {
            _stunTimer += seconds;
            if (_stunTimer > _STUN_TIMER_MAX_TIME)
            {
                _stunned = false;
            }
        }
        PlayerState.IsRunning = EnemyAI.RunActivated;

        if (EnemyAI.Jump == EnemyAI.JumpState.Pressed)
        {
            _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= seconds;
        }

        if (PlayerState.CollidingY == PlayerState.CollidingYState.Ground)
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= seconds;
        }

        if (EnemyAI.Jump == EnemyAI.JumpState.Held)
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

        //Todo: This sucks. Don't put this kind of logic in this function
        Stats.Stamina -= NormalAttack.Update(seconds, PlayerAttacked, PlayerState.IsExhausted, EnemyAI.NormalAttackActivated);
    }

    private void HandleState(float seconds)
    {
        // Move player along Y axis
        if (PlayerState.MovementY == PlayerState.MovementStateY.Jumped)
        {
            velocity.Y = Stats.JumpStrength;
        }
        else
        {
            velocity.Y += Globals.GRAVITY * seconds;
        }

        // Move player along X axis
        if (PlayerState.MovementX == PlayerState.MovementStateX.Idle)
        {
            velocity.X = 0;
        }
        else if (PlayerState.MovementX == PlayerState.MovementStateX.Slowing)
        {
            velocity.X /= Stats.Acceleration / 2;
        }
        else if (PlayerState.MovementX == PlayerState.MovementStateX.MoveLeft)
        {
            velocity.X -= Stats.Acceleration;
            PlayerState.LastDirection = PlayerState.LastLookState.Left;
        }
        else if (PlayerState.MovementX == PlayerState.MovementStateX.MoveRight)
        {
            velocity.X += Stats.Acceleration;
            PlayerState.LastDirection = PlayerState.LastLookState.Right;
        }

        // set max speed of X velocity
        var speed = PlayerState.IsRunning && !PlayerState.IsExhausted ? Stats.RunSpeed : Stats.WalkSpeed;
        if (Math.Abs(Velocity.X) > speed)
        {
            velocity.X = Math.Sign(Velocity.X) * speed;
        }
        if (Math.Abs(Velocity.X) > Globals.MAX_SPEED.X)
        {
            velocity.X = Math.Sign(Velocity.X) * Globals.MAX_SPEED.X;
        }

        // set max speed of Y velocity
        if (Math.Abs(Velocity.Y) > Globals.MAX_SPEED.Y)
        {
            velocity.Y = Math.Sign(Velocity.Y) * Globals.MAX_SPEED.Y;
        }
    }

    public void HandleStamina(float seconds)
    {
        // Handle player exhaustion
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

        // handle stamina drain
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

    private void UpdatePositionFromVelocity(float seconds)
    {
        Position += velocity * seconds;
        Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
    }

    private void MoveChildren(Vector2 _position)
    {
        PosessableCollider = PosessableCollider with { Position = new Vector2(Position.X - 100, Position.Y - 100) };
        _particleController.SetPosition(new Vector2(_position.X + (Stats.Width / 2), _position.Y + Stats.Height));
        PosessRay.Position = new Point2(Position.X + (Stats.Width / 2), Position.Y + (Stats.Height / 2));
        EnemyAI.EnemyAIFieldOfView = EnemyAI.EnemyAIFieldOfView with { Position = new Vector2(Position.X - 400, Position.Y - 400) };

        _enemyHealthBar.Position = new Vector2(Position.X, Position.Y - 10);

        if (PlayerState.LastDirection == PlayerState.LastLookState.Left)
        {
            NormalAttack.Position = new Vector2(Position.X - NormalAttack.Bounds.Width, Position.Y + (Stats.Height / 2));
        }
        else if (PlayerState.LastDirection == PlayerState.LastLookState.Right)
        {
            NormalAttack.Position = new Vector2(Position.X + Stats.Width, Position.Y + (Stats.Height / 2));
        }
        // highlight
        _outlineRect.Position = _position;
    }

    private void ResolveCollisions()
    {
        PlayerState.CollidingY = PlayerState.CollidingYState.None;
        PlayerState.CollidingX = PlayerState.CollidingXState.None;
        foreach (var entity in _currentCollisions)
        {
            if (entity is Platform)
            {
                var intersectionArea = Bounds.Intersection(entity.Bounds);
                if (intersectionArea.Height > intersectionArea.Width)
                {
                    if (Bounds.IsCollidingLeft(entity.Bounds))
                    {
                        velocity.X = 0;
                        Position = Position with { X = entity.Bounds.Right };
                        PlayerState.CollidingX = PlayerState.CollidingXState.Left;
                    }
                    else if (Bounds.IsCollidingRight(entity.Bounds))
                    {
                        velocity.X = 0;
                        Position = Position with { X = entity.Bounds.Left - Bounds.Width };
                        PlayerState.CollidingX = PlayerState.CollidingXState.Right;
                    }
                }
                else
                {
                    if (Bounds.IsCollidingBottom(entity.Bounds))
                    {
                        velocity.Y = 0;
                        Position = Position with { Y = entity.Bounds.Top - Bounds.Height };
                        PlayerState.CollidingY = PlayerState.CollidingYState.Ground;
                    }
                    else if (Bounds.IsCollidingTop(entity.Bounds))
                    {
                        velocity.Y = 0;
                        Position = Position with { Y = entity.Bounds.Bottom };
                        PlayerState.CollidingY = PlayerState.CollidingYState.Ceiling;
                    }
                }
            }
            else if (entity is HealthItem healthItem)
            {
                if (Stats.Health + healthItem.HealthValue < Stats.MaxHealth)
                {
                    Stats.Health += healthItem.HealthValue;
                }
                else
                {
                    Stats.Health = Stats.MaxHealth;
                }

                UpdateHealthBar?.Invoke(Stats.Health);
                PickedUpCollectible?.Invoke(healthItem);
            }
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

    public bool PosessIsReady()
    {
        return _posessCooldownTimer.ElapsedGameTime.TotalSeconds > POSESS_TIMER_WAIT_TIME;
    }

    public bool CanBePosessed()
    {
        return Stats.Health < (Stats.MaxHealth / 2);
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
        PlayerState.IsRunning = false;
        PlayerState.MovementX = PlayerState.MovementStateX.Idle;
    }

    private void ReceiveDamage(int damage, float knockbackDirection, float knockbackStrenght)
    {
        Stats.Health -= damage;
        if (IsPosessed)
        {
            DamageTextController.Hit(new Vector2(Position.X + (Stats.Width / 2), Position.Y + (Stats.Height / 2)), damage, EasingFunctions.QuadraticOut, 0.5f, Color.IndianRed);
            UpdateHealthBar?.Invoke(Stats.Health);
        }
        else
        {
            _enemyHealthBar.SetCurrentHealth(Stats.Health);
            DamageTextController.Hit(new Vector2(Position.X + (Stats.Width / 2), Position.Y + (Stats.Height / 2)), damage, EasingFunctions.QuadraticOut, 0.5f, Color.White);
        }
        Velocity = new(knockbackDirection * knockbackStrenght, Velocity.Y - knockbackStrenght);
        PlayerState.MovementX = PlayerState.MovementStateX.KnockedBack;
        _stunned = true;
        _stunTimer = 0;
        if (Stats.Health < 1)
        {
            Die();
            IsPosessed = false;
        }
    }

    private void Die()
    {
        PosessableDied?.Invoke(this);
    }

    private void Signal_PlayerAttacked(Attack attack)
    {
        if (Bounds.Intersects(attack.Bounds))
        {
            var knockbackDirection = attack.Position.X > Position.X ? -1 : 1;
            ReceiveDamage(attack.BaseDamage, knockbackDirection, attack.KnockBack);
        }
    }
}