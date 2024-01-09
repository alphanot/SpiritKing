using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SpiritKing.Components.Interfaces;
using SpiritKing.Components.Posessables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritKing.Components
{
    public class EnemyHealthBar : INode
    {
        public int DrawOrder => 1;

        private float _MAX_HEALTH = 0;
        private float _currentHealth = 0;

        private int _maxHealthBarWidth = 0;

        private Texture2D _healthSprite;
        private RectangleF _healthRect;
        public Vector2 Position { get; set; } = Vector2.Zero;
        private Color _healthColor = Color.Red;


        public EnemyHealthBar(Game game, float currentHealth, float maxHealth, int maxHealthBarWidth)
        {
            _currentHealth = currentHealth;
            _MAX_HEALTH = maxHealth;
            _maxHealthBarWidth = maxHealthBarWidth;
            _healthRect.Height = 5;
            _healthSprite = new Texture2D(game.GraphicsDevice, 1, 1);
            _healthSprite.SetData(new[] { Color.White });
            SetCurrentHealth(currentHealth);
            Posessable.UpdateHealthBar += SetCurrentHealth;

        }


        public void Dispose()
        {
            _healthSprite?.Dispose();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_healthSprite, Position, (Rectangle)_healthRect, _healthColor);
        }

        public void Update(GameTime gameTime) { }

        public void SetCurrentHealth(float currentHealth)
        {
            _currentHealth = currentHealth;
            _healthRect.Width = (_currentHealth / _MAX_HEALTH) * _maxHealthBarWidth;
        }
    }
}
