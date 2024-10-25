using Microsoft.Xna.Framework;
using MonoGame.Extended;
using SpiritKing.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritKing.Components;
public class QuadTree
{
    private QuadTree? _topLeft;
    private QuadTree? _topRight;
    private QuadTree? _bottomLeft;
    private QuadTree? _bottomRight;

    public Rectangle Bounds { get; init; }

    private bool _isDivided;

    private int _maximumNodes;

    private int _maximumDivisions;

    private List<ISpatialEntity> _entities;

    public QuadTree(Rectangle bounds, int maximumNodes, int maximumDivisions)
    {
        Bounds = bounds;
        _maximumNodes = maximumNodes;
        _maximumDivisions = maximumDivisions;
        _isDivided = false;
        _entities = [];
    }

    public void Insert(ISpatialEntity entity)
    {
        if (!Bounds.Contains(entity.Bounds))
        {
            throw new ArgumentException("Entity does not fit within Quad Tree Bounds.", nameof(entity));
        }

        if (_entities.Count >= _maximumNodes && _maximumDivisions <= 0 && !_isDivided)
        {
            Split();
        }

        var containingChild = GetContainingChild(entity);

        if (containingChild != null)
        {
            containingChild.Insert(entity);
        }
        else
        {
            _entities.Add(entity);
        }
    }

    public bool Remove(ISpatialEntity entity)
    {
        var containingChild = GetContainingChild(entity);

        var removed = containingChild?.Remove(entity) ?? _entities.Remove(entity);

        if (removed && CountEntities() <= _maximumNodes)
        {
            Merge();
        }

        return removed;
    }

    public IEnumerable<ISpatialEntity> FindCollisions(ISpatialEntity entity)
    {

        var treeNodes = new Queue<QuadTree>();
        var collisions = new List<ISpatialEntity>();

        treeNodes.Enqueue(this);

        while (treeNodes.Count > 0)
        {
            var treeNode = treeNodes.Dequeue();

            if (!entity.Bounds.Intersects(treeNode.Bounds))
            {
                continue;
            }

            collisions.AddRange(treeNode._entities.Where(e => e.Bounds.Intersects(entity.Bounds)));

            if (treeNode._isDivided)
            {
                if (entity.Bounds.Intersects(treeNode._topLeft!.Bounds))
                {
                    treeNodes.Enqueue(treeNode._topLeft);
                }

                if (entity.Bounds.Intersects(treeNode._topRight!.Bounds))
                {
                    treeNodes.Enqueue(treeNode._topRight);
                }

                if (entity.Bounds.Intersects(treeNode._bottomLeft!.Bounds))
                {
                    treeNodes.Enqueue(treeNode._bottomLeft);
                }

                if (entity.Bounds.Intersects(treeNode._bottomRight!.Bounds))
                {
                    treeNodes.Enqueue(treeNode._bottomRight);
                }
            }
        }

        return collisions;
    }

    private int CountEntities()
    {
        var childCount = _isDivided ?
            _topLeft!.CountEntities() + _topRight!.CountEntities() + _bottomLeft!.CountEntities() + _bottomRight!.CountEntities() :
            0;
        return childCount + _entities.Count;
    }

    private void Merge()
    {
        if (_isDivided)
        {
            _entities.AddRange(_topLeft!._entities);
            _entities.AddRange(_topRight!._entities);
            _entities.AddRange(_bottomLeft!._entities);
            _entities.AddRange(_bottomRight!._entities);
            _topLeft = _topRight = _bottomLeft = _bottomRight = null;
        }
    }

    private void Split()
    {
        _topLeft = CreateChild(Bounds.Left, Bounds.Top);
        _topRight = CreateChild(Bounds.Center.X, Bounds.Top);
        _bottomLeft = CreateChild(Bounds.Left, Bounds.Center.Y);
        _bottomRight = CreateChild(Bounds.Center.X, Bounds.Center.Y);

        var entities = _entities.AsReadOnly();

        foreach (var entity in entities)
        {
            var containingChild = GetContainingChild(entity);
            if (containingChild != null)
            {
                containingChild.Insert(entity);
                _entities.Remove(entity);
            }
        }
    }

    private QuadTree? GetContainingChild(ISpatialEntity entity)
    {
        if (_topLeft != null && _topLeft.Bounds.Contains(entity.Bounds))
        {
            return _topLeft;
        }
        if (_topRight != null && _topRight.Bounds.Contains(entity.Bounds))
        {
            return _topRight;
        }
        if (_bottomLeft != null && _bottomLeft.Bounds.Contains(entity.Bounds))
        {
            return _bottomLeft;
        }
        if (_bottomRight != null && _bottomRight.Bounds.Contains(entity.Bounds))
        {
            return _bottomRight;
        }
        return null;
    }

    private QuadTree CreateChild(int x, int y) => new(new(x, y, Bounds.Width / 2, Bounds.Height / 2), _maximumNodes, _maximumDivisions - 1);
}
