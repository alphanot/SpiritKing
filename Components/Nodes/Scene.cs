﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using SpiritKing.Components.Interfaces;
using SpiritKing.Controllers;
using System;
using System.Collections.Generic;

namespace SpiritKing.Components.Nodes;

public abstract class Scene : Interfaces.IDrawable, Interfaces.IUpdateable
{
    public List<Interfaces.IDrawable> DrawableNodes = new();
    public List<Interfaces.IUpdateable> UpdateableNodes = new();

    public string Name { get; internal set; }

    public MusicController MusicController { get; private set; }

    public OrthographicCamera Camera { get; set; }

    public event Action<Scene>? SceneSwitched;

    public int DrawOrder { get; } = 0;

    public virtual Game Game { get; set; }

    public Color BackgroundColor { get; set; }

    public bool Enabled { get; protected set; } = true;

    public int UpdateOrder => 1;

    public bool Visible { get; protected set; } = true;

    public Texture2D _logo;

    protected SaveDataController SaveDataController { get; private set; } = new SaveDataController();
    public Scene(Game game)
    {
        Game = game;
        var viewportAdapter = new BoxingViewportAdapter(game.Window, game.GraphicsDevice, 1280, 720);
        Camera = new OrthographicCamera(viewportAdapter);
        MusicController = new(game.Content);
        BackgroundColor = Color.DarkGray;
    }

    public virtual void Update(GameTime gameTime)
    {
        foreach (var node in UpdateableNodes)
        {
            if (node.Enabled)
            {
                node.Update(gameTime);
            }
        }
    }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Game.GraphicsDevice.Clear(BackgroundColor);
        var transformMatrix = Camera.GetViewMatrix();
        spriteBatch.Begin(transformMatrix: transformMatrix);
        
        foreach (var node in DrawableNodes)
        {
            if (node.Visible)
            {
                node.Draw(gameTime, spriteBatch);
            }
        }
        spriteBatch.End();
    }

    public void AddNode(INode node)
    {
        if (node is Interfaces.IDrawable)
        {
            DrawableNodes.Add(node as Interfaces.IDrawable);
        }
        if (node is Interfaces.IUpdateable)
        {
            UpdateableNodes.Add(node as Interfaces.IUpdateable);
        }
    }
    public virtual void SortNodes()
    {
        DrawableNodes.Sort((a, b) => a.DrawOrder.CompareTo(b.DrawOrder));
        UpdateableNodes.Sort((a, b) => a.UpdateOrder.CompareTo(b.UpdateOrder));
    }

    public virtual void Dispose()
    {
        foreach (var node in DrawableNodes)
        {
            node.Dispose();
        }
        MusicController.Unload();
    }

    //TODO: this is stupid, at least change name.
    protected virtual void OnSceneSwitched(Scene scene) => SceneSwitched?.Invoke(scene);
}