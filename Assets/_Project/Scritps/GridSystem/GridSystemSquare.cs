using System;
using System.Collections.Generic;
using ChainDefense.GridSystem.core;
using UnityEngine;

namespace ChainDefense.GridSystem
{
    public class GridSystemSquare<T> : GridSystemBase<T>
    {
        public GridSystemSquare(int width, int height, Func<GridSystemBase<T>, GridPosition, T> factory, float cellSize = 1f) 
            : base(width, height, cellSize)
        {
            for (var x = 0; x < GetWidth(); x++)
            {
                for (var z = 0; z < GetHeight(); z++)
                {
                    var gridPosition = new GridPosition(x, z);
                    _gridObjects[x, z] = factory(this, gridPosition);
                }
            }
        }

        public override Vector3 GetWorldPosition(GridPosition gridPos) =>
            new Vector3(gridPos.X, 0, gridPos.Z) * GetCellSize();

        public override GridPosition GetGridPosition(Vector3 worldPosition) =>
            new(
                Mathf.RoundToInt(worldPosition.x / GetCellSize()),
                Mathf.RoundToInt(worldPosition.z / GetCellSize())
            );

        public override List<GridPosition> GetNeighbors(GridPosition pos) =>
            new()
            {
                pos + new GridPosition(-1, 0), //left
                pos + new GridPosition(+1, 0), //right
                pos + new GridPosition(0, +1), //top
                pos + new GridPosition(0, -1), //bot
            };
    }
}