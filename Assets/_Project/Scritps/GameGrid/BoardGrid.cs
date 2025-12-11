using System;
using System.Collections.Generic;
using System.Linq;
using ChainDefense.Balls;
using ChainDefense.GridSystem;
using ChainDefense.GridSystem.core;
using UnityEngine;

namespace ChainDefense.GameGrid
{
    public class BoardGrid : MonoBehaviour//TODO: Add support for towers
    {
        public event EventHandler OnNewBallPlaced;
        public event EventHandler OnBallRemoved;
        public event EventHandler OnBallMoved;
        
        
        public static BoardGrid Instance { get; private set; }

        private const float SLOT_SIZE = 2f;
        [SerializeField] private int _width;
        [SerializeField] private int _height;

        [SerializeField] private GameObject _gridDebug;
        [SerializeField] private bool _enableDebug;


        private GridSystemBase<GridObject<Ball>> _gridSystem;

        private void Awake()
        {
            Instance = this;

            _gridSystem = new GridSystemSquare<GridObject<Ball>>(
                _width,
                _height,
                (grid, pos) => new GridObject<Ball>(grid, pos),
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

        public bool IsSlotEmpty(GridPosition gridPos) =>
            _gridSystem.GetGridObject(gridPos).Get() == null;

        public void AddBallToPosition(GridPosition gridPos, Ball ball)
        {
            _gridSystem.GetGridObject(gridPos).Add(ball);
            OnNewBallPlaced?.Invoke(this, EventArgs.Empty);
        }

        public Ball GetBallAtPosition(GridPosition position) =>
            _gridSystem.GetGridObject(position).Get();
        
        public bool TryGetBall(GridPosition position, out Ball ball)
        {
            if (_gridSystem.IsValidGridPosition(position) && !IsSlotEmpty(position))
            {
                ball = GetBallAtPosition(position);
                return true;
            }

            ball = null;
            return false;
        }

        public void RemoveBallAtPosition(GridPosition position, Ball targetBall)
        {
            _gridSystem.GetGridObject(position).Remove(targetBall);
            OnBallRemoved?.Invoke(this, EventArgs.Empty);
        }

        public void MoveBall(GridPosition from, GridPosition to, Ball targetBall)
        {
           RemoveBallAtPosition(from, targetBall);
           AddBallToPosition(to, targetBall);
           
           OnBallMoved?.Invoke(this, EventArgs.Empty);
        }

        public List<GridPosition> GetAllValidNeighbors(GridPosition position)
        {
            var neighbors = _gridSystem.GetNeighbors(position);
            neighbors.Add(position + new GridPosition(1, 1)); //Top-right
            neighbors.Add(position + new GridPosition(1, -1)); //Bottom-right
            neighbors.Add(position + new GridPosition(-1, 1)); //Top-left
            neighbors.Add(position + new GridPosition(-1, -1)); //Bottom-left

            return neighbors.Where(n => _gridSystem.IsValidGridPosition(n)).ToList();
        }
    }
}