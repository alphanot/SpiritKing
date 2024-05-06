using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritKing.Components.Posessables;
using System;
using RectangleF = MonoGame.Extended.RectangleF;

namespace SpiritKing.Components.Nodes;

public class HUD : Interfaces.IDrawable, Interfaces.IUpdateable
{
    public int DrawOrder => 100;

    public bool Enabled => true;

    public int UpdateOrder => 1;

    public bool Visible => true;

    private float _MAX_HEALTH = 0;
    private float _MAX_STAMINA = 0;
    private float _currentHealth = 0;
    private float _currentStamina = 0;
    private bool _posessCanActivate = false;

    private readonly Texture2D _healthSprite;
    private RectangleF _healthRect;
    private Vector2 _healthPosition = Vector2.Zero;
    private Color _healthColor = Color.Red;

    private readonly Texture2D _staminaSprite;
    private RectangleF _staminaRect;
    private Vector2 _staminaPosition = Vector2.Zero;
    private Color _staminaColor = Color.Green;

    private readonly Texture2D _posessTimerSprite;
    private RectangleF _posessCanActivateRect;
    private Vector2 _posessTimerPosition = Vector2.Zero;
    private Color _posessCanActivateColor = Color.SlateBlue;

    private readonly GameTime _healthFlashTimer;

    private bool _healthFlashEnabled;
    private readonly SpriteFont _font;

    public HUD(Game game, float maxHealth, float maxStamina, float currentHealth, float currentStamina, bool isExhausted)
    {
        _font = game.Content.Load<SpriteFont>("Fonts/HUDText");
        SetHUD(maxHealth, maxStamina, currentHealth, currentStamina, isExhausted);
        _healthSprite = new Texture2D(game.GraphicsDevice, 1, 1);
        _healthSprite.SetData(new[] { Color.White });

        _staminaSprite = new Texture2D(game.GraphicsDevice, 1, 1);
        _staminaSprite.SetData(new[] { Color.White });
        _staminaColor = isExhausted ? Color.Yellow : Color.Green;

        _posessTimerSprite = new Texture2D(game.GraphicsDevice, 1, 1);
        _posessTimerSprite.SetData(new[] { Color.White });

        _healthPosition = new Vector2(50, 50);
        _staminaPosition = new Vector2(50, 75);
        _posessTimerPosition = new Vector2(50, 100);

        Posessable.UpdateHealthBar += SetCurrentHealth;
        Posessable.UpdateStaminaBar += SetCurrentStamina;
        Posessable.UpdatePosessCanActivate += SetPosessCanActivate;
        _healthFlashTimer = new GameTime();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // Draw health bar
        spriteBatch.Draw(_healthSprite, _healthPosition, (Rectangle)_healthRect, _healthColor);
        spriteBatch.DrawString(_font, (int)_currentHealth + "/" + (int)_MAX_HEALTH, _healthPosition + new Vector2(0, -2), Color.White);

        // Draw stamina bar
        spriteBatch.Draw(_staminaSprite, _staminaPosition, (Rectangle)_staminaRect, _staminaColor);
        spriteBatch.DrawString(_font, (int)_currentStamina + "/" + (int)_MAX_STAMINA, _staminaPosition + new Vector2(0, -2), Color.White);

        if (_posessCanActivate)
        {
            spriteBatch.Draw(_posessTimerSprite, _posessTimerPosition, (Rectangle)_posessCanActivateRect, _posessCanActivateColor);
        }
    }

    public void Dispose()
    {
        _healthSprite?.Dispose();
        _staminaSprite?.Dispose();
        _posessTimerSprite?.Dispose();
    }

    public void Update(GameTime gameTime)
    {
        _healthFlashTimer.ElapsedGameTime += gameTime.ElapsedGameTime;
        if (_healthFlashEnabled)
        {
            _healthColor = Color.White;
        }

        if (_healthFlashTimer.ElapsedGameTime > TimeSpan.FromMilliseconds(300))
        {
            _healthFlashEnabled = false;
            _healthFlashTimer.ElapsedGameTime = TimeSpan.Zero;
            _healthColor = Color.Red;
        }
    }

    public void SetCurrentHealth(float currentHealth)
    {
        _currentHealth = currentHealth;
        _healthRect.Width = _currentHealth / _MAX_HEALTH * _MAX_HEALTH;
        _healthFlashEnabled = true;
    }

    public void SetCurrentStamina(float currentStamina, bool isExhausted)
    {
        _currentStamina = currentStamina;
        _staminaRect.Width = _currentStamina / _MAX_STAMINA * _MAX_STAMINA;
        _staminaColor = isExhausted ? Color.Yellow : Color.Green;
    }

    public void SetHUD(float maxHealth, float maxStamina, float currentHealth, float currentStamina, bool isExhausted)
    {
        _MAX_HEALTH = maxHealth;
        _MAX_STAMINA = maxStamina;
        _currentHealth = currentHealth;
        _currentStamina = currentStamina;
        _healthRect = new RectangleF(0, 0, _MAX_HEALTH, 20);
        _staminaRect = new RectangleF(0, 0, _MAX_STAMINA, 20);
        _posessCanActivateRect = new RectangleF(0, 0, 20, 20);
        SetCurrentHealth(currentHealth);
        SetCurrentStamina(currentStamina, isExhausted);
    }

    public void SetPosition(Vector2 position)
    {
        _healthPosition = position + new Vector2(50, 50);
        _staminaPosition = position + new Vector2(50, 75);
        _posessTimerPosition = position + new Vector2(50, 100);
    }

    public void SetPosessCanActivate(bool canPosess)
    {
        _posessCanActivate = canPosess;
    }
}