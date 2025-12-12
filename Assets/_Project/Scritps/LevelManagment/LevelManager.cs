using System;
using System.Collections.Generic;
using ChainDefense.Balls;
using ChainDefense.ChainManagment;
using ChainDefense.GameGrid;
using ChainDefense.GridSystem.core;
using ChainDefense.PlayerBase;
using ChainDefense.Utilities;
using DG.Tweening;
using IKhom.StateMachineSystem.Runtime;
using UnityEngine;

namespace ChainDefense.Core
{
    public class LevelManager : MonoBehaviour //TODO: TEMP SOLUTION, FIX AND REFACTOR. Move board filling to separate class
    {
        private enum LevelState
        {
            Gameplay,
            RefillBalls,
            ReorderBalls,
            LevelComplete
        }

        private BallSpawner _ballSpawner;
        private BoardGrid _boardGrid;
        private ChainValidator _chainValidator;
        private LevelState _currentState;
        private bool _isRefillCompleted;
        private bool _isReorderCompleted;
        private readonly Dictionary<Ball, Vector3> _ballTargetPositions = new();
        private readonly List<GridPosition> _allOccupiedPositions = new();
        private WaveManager _waveManager;
        private BaseManager _baseManager;

        private void Awake()
        {
            Application.targetFrameRate = 30;
        }

        private void Start()
        {
            _boardGrid = BoardGrid.Instance;
            _ballSpawner = BallSpawner.Instance;
            _chainValidator = ChainValidator.Instance;
            _waveManager = WaveManager.Instance;
            _baseManager = BaseManager.Instance;

            _chainValidator.OnChainDestroyed += ChainValidator_OnChainDestroyed;
            _waveManager.OnAllWavesCompleted += WaveManager_OnAllWavesCompleted;
            _baseManager.OnGameOver += BaseManager_OnGameOver;

            FillBoardWithGuaranteedDistribution();
        }

        private void BaseManager_OnGameOver(object sender, EventArgs e)
        {
            Debug.Log("Game Over!");
        }

        private void WaveManager_OnAllWavesCompleted(object sender, EventArgs e)
        {
            Debug.Log("Victory!");
        }

        private void ChainValidator_OnChainDestroyed(object sender, List<Ball> e)
        {
            _isReorderCompleted = false;
            _currentState = LevelState.ReorderBalls;
        }

        private void Update()
        {
            switch (_currentState)
            {
                case LevelState.Gameplay:
                    break;
                case LevelState.ReorderBalls:
                    if (!_isReorderCompleted)
                    {
                        ReorderBallsOnBoard();
                        _isReorderCompleted = true;
                    }

                    break;
                case LevelState.RefillBalls:
                    if (!_isRefillCompleted)
                    {
                        FillBoardWithGuaranteedDistribution();
                        _isRefillCompleted = true;
                    }

                    break;
                case LevelState.LevelComplete:
                    break;
            }
        }


        private void ReorderBallsOnBoard()
        {
            _ballTargetPositions.Clear();
            var allGridPositions = _boardGrid.GetAllGridPositions();
            _allOccupiedPositions.Clear();

            foreach (var gridPosition in allGridPositions)
            {
                if (!_boardGrid.IsSlotEmpty(gridPosition))
                {
                    _allOccupiedPositions.Add(gridPosition);
                }
            }

            for (var index = 0; index < _allOccupiedPositions.Count; index++)
            {
                var ocupiedGridPosition = _allOccupiedPositions[index];
                var newGridPosition = GotLowestGridPosition(ocupiedGridPosition, _allOccupiedPositions);

                if (_boardGrid.TryGetBall(ocupiedGridPosition, out var ball))
                {
                    _ballTargetPositions.Add(ball, _boardGrid.GetWorldPosition(newGridPosition));
                }

                _allOccupiedPositions[index] = newGridPosition;
            }

            foreach (var (ball, newPosition) in _ballTargetPositions)
            {
                ball.transform.DOMoveZ(newPosition.z, 0.3f, true).SetEase(Ease.InOutBounce);
            }

            _isRefillCompleted = false;
            _currentState = LevelState.RefillBalls;
        }

        private GridPosition GotLowestGridPosition(GridPosition startPosition, List<GridPosition> occupiedPositions)
        {
            var targetPosition = startPosition;

            var newPosition = startPosition + new GridPosition(0, -1);

            while (!occupiedPositions.Contains(newPosition) && _boardGrid.IsValidGridPosition(newPosition))
            {
                targetPosition = newPosition;
                newPosition += new GridPosition(0, -1);
            }

            return targetPosition;
        }

        private void FillBoardWithGuaranteedDistribution()
        {
            var allGridPositions = _boardGrid.GetAllGridPositions();
            var totalSlots = allGridPositions.Count;

            var ballIndices = new List<int>();

            var numberOfBallTypes = 4;
            var ballsPerType = totalSlots / numberOfBallTypes;
            var remainder = totalSlots % numberOfBallTypes;

            for (var ballType = 0; ballType < numberOfBallTypes; ballType++)
            {
                var count = ballsPerType;

                if (ballType < remainder)
                    count++;

                for (var i = 0; i < count; i++)
                {
                    ballIndices.Add(ballType);
                }
            }

            ballIndices.ShuffleList();

            for (var i = 0; i < allGridPositions.Count; i++)
            {
                if (!_allOccupiedPositions.Contains(allGridPositions[i]))
                {
                    _ballSpawner.SpawnBallAtGridPosition(ballIndices[i], allGridPositions[i]);
                }
            }
        }
    }
}