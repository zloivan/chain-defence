using System;
using System.Collections.Generic;
using System.Linq;
using IKhom.GridSystems._Samples.helpers;
using IKhom.GridSystems.Runtime;
using IKhom.GridSystems.Runtime.core;
using UnityEngine;

namespace IKhom.GridSystems._Samples.LevelGrid
{
    public class BoardGrid : MonoBehaviour
    {
        public event EventHandler OnNewStackPlaced;
        public event EventHandler OnStackremoved;
        public static BoardGrid Instance { get; private set; }

        private const float SLOT_SIZE = 2f;
        [SerializeField] private int _width;
        [SerializeField] private int _height;

        [SerializeField] private GameObject _gridDebug;
        [SerializeField] private bool _enableDebug;


        private GridSystemBase<GridObject<TileStack>> _gridSystem;

        private void Awake()
        {
            Instance = this;

            _gridSystem = new GridSystemSquare<GridObject<TileStack>>(
                _width,
                _height,
                (grid, pos) => new GridObject<TileStack>(grid, pos),
                SLOT_SIZE);

            if (_enableDebug)
                _gridSystem.CreateDebugObjects(_gridDebug.transform, transform);
        }

        public int GetWidth() =>
            _gridSystem.GetWidth();

        public int GetHeight() =>
            _gridSystem.GetHeight();

        public Vector3 GetWorldPosition(GridPosition targetGridPosition) =>
            _gridSystem.GetWorldPosition(targetGridPosition);

        public bool IsValidGridPosition(GridPosition gridPosition) =>
            _gridSystem.IsValidGridPosition(gridPosition);

        public GridPosition GetGridPosition(Vector3 worldPointerPosition) =>
            _gridSystem.GetGridPosition(worldPointerPosition);

        public List<GridPosition> GetAllGridPositions()
        {
            var result = new List<GridPosition>();
            for (var x = 0; x < _gridSystem.GetWidth(); x++)
            {
                for (var z = 0; z < _gridSystem.GetHeight(); z++)
                {
                    result.Add(new GridPosition(x, z));
                }
            }

            return result;
        }

        public List<GridPosition> GetNeighbors(GridPosition targetPosition) =>
            _gridSystem.GetNeighbors(targetPosition);

        public List<GridPosition> GetValidNeighbors(GridPosition targetPosition) =>
            GetNeighbors(targetPosition).Where(IsValidGridPosition).ToList();

        public bool IsSlotEmpty(GridPosition gridPos) =>
            _gridSystem.GetGridObject(gridPos).Get() == null;

        public void PlaceStackAtPosition(GridPosition gridPos, TileStack tileStack)
        {
            var gridObject = _gridSystem.GetGridObject(gridPos);

            if (gridObject.Get() != null)
            {
                return;
            }

            gridObject.Set(tileStack);
            OnNewStackPlaced?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveStackAtPosition(GridPosition position)
        {
            var gridObj = _gridSystem.GetGridObject(position);
            var stack = gridObj.Get();

            if (stack == null)
                return;

            Destroy(stack.gameObject);
            gridObj.ClearObjectList();
            OnStackremoved?.Invoke(this, EventArgs.Empty);
        }
    }
}