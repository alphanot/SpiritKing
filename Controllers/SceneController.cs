
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritKing.Components;
using SpiritKing.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritKing.Controllers
{
    public class SceneController
    {
        public List<Scene> Scenes { get; set; } = new List<Scene>();

        public Scene CurrentScene { get; set; }

        public SceneController() { }

        public SceneController(List<Scene> scenes)
        {
            Scenes = scenes;
        }

        public virtual void InitScenes()
        {
            Scenes.Sort((a, b) => a.DrawOrder.CompareTo(b.DrawOrder));
        }

        public virtual void SetScene(Scene scene)
        {
            if (CurrentScene != null)
            {
                CurrentScene.SceneSwitched -= Scene_SwitchScene;
            }
            CurrentScene = scene;
            CurrentScene.SceneSwitched += Scene_SwitchScene;
        }

        public virtual void SetScene(int sceneIndex)
        {
            CurrentScene = Scenes.ElementAt(sceneIndex);
        }

        public void Update(GameTime gameTime)
        {
            CurrentScene.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) 
        {
            CurrentScene.Draw(gameTime, spriteBatch);
        }
        private void Scene_SwitchScene(Scene nextScene)
        {
            CurrentScene.Dispose();
            CurrentScene = nextScene;
        }
    }
}
