using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Particles;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SpiritKing.Components.Interfaces;

namespace SpiritKing.Controllers
{
    public class ParticleController : INode
    {
        public ParticleEffect ParticleEffect { get; set; }
        public TextureRegion2D TextureRegion { get; private set; }

        public int DrawOrder => 1;

        private Texture2D _particleTexture;

        public ParticleController(GraphicsDevice graphicsDevice, Vector2 position)
        {
            _particleTexture = new Texture2D(graphicsDevice, 1, 1);
            _particleTexture.SetData(new[] { Color.White });
            TextureRegion = new TextureRegion2D(_particleTexture);
            ParticleEffect = new ParticleEffect(autoTrigger: false)
            {
                Position = position,
                Emitters = new List<ParticleEmitter>()
            };
        }

        public void SetEmitters(List<ParticleEmitter> emitters)
        {
            ParticleEffect.Emitters = emitters;
        }

        public void AddEmitter(ParticleEmitter emitter)
        {
            ParticleEffect.Emitters.Add(emitter);
        }

        public void SetPosition(Vector2 position)
        {
            ParticleEffect.Position = position;
        }

        public void Dispose()
        {
            _particleTexture.Dispose();
            ParticleEffect.Dispose();
        }

        public void SetQuantity(int emitterIndex, int quantity)
        {
            ParticleEffect.Emitters[emitterIndex].Parameters.Quantity = quantity;
        }

        public void Update(GameTime gameTime)
        {
            ParticleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ParticleEffect);
        }
    }
}
