using System.Collections.Generic;
using ChainDefense.GridSystem.components;
using UnityEngine;

namespace ChainDefense.GridSystem.core
{
    public abstract class GridSystemBase<TGridObject>
    {
        protected readonly TGridObject[,] _gridObjects;
        private readonly int _width;
        private readonly int _height;
        private readonly float _cellSize;

        protected GridSystemBase(int width, int height, float cellSize)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _gridObjects = new TGridObject[width, height];
        }

        public abstract Vector3 GetWorldPosition(GridPosition gridPos);
        
        public abstract GridPosition GetGridPosition(Vector3 worldPosition);

        public TGridObject GetGridObject(GridPosition gridPos) =>
            _gridObjects[gridPos.X, gridPos.Z];

        public int GetWidth() =>
            _width;

        public int GetHeight() =>
            _height;

        public float GetCellSize() =>
            _cellSize;
        
        public virtual bool IsValidGridPosition(GridPosition gridPos) =>
            gridPos.X >= 0
            && gridPos.X < _width
            && gridPos.Z >= 0
            && gridPos.Z < _height;

        public void CreateDebugObjects(Transform prefab, Transform parent)
        {
            for (var x = 0; x < _width; x++)
            {
                for (var z = 0; z < _height; z++)
                {
                    var gridPosition = new GridPosition(x, z);

                    var debugObj = Object.Instantiate(prefab, GetWorldPosition(gridPosition), Quaternion.identity,
                        parent);

                    debugObj.GetComponent<GridDebugObject>().SetGridObject(GetGridObject(gridPosition));
                }
            }
        }

        public abstract List<GridPosition> GetNeighbors(GridPosition pos);
        
    }
}