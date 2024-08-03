using Microsoft.Xna.Framework;
using MonoGame.Extended;
using SpiritKing.Components.Interfaces;
using SpiritKing.Utils;
using System;
using System.Runtime.CompilerServices;

namespace SpiritKing.Components;
public abstract class SpatialEntity : ISpatialEntity
{
    public Rectangle Bounds => _bounds.ToRectangle();

    protected RectangleF _bounds;

    public Vector2 Size
    {
        get => _bounds.Size;
        set => _bounds.Size = value;
    }

    public Vector2 Position
    {
        get => _bounds.Position;
        set => _bounds.Position = value;
    }

}

