using System;
using System.Collections.Generic;
using ChainDefense.Balls;
using ChainDefense.ChainManagment;
using ChainDefense.Enemies;
using ChainDefense.Events;
using ChainDefense.GameGrid;
using ChainDefense.GridSystem.core;
using ChainDefense.LevelManagement;
using ChainDefense.PlayerBase;
using ChainDefense.Utilities;
using ChainDefense.Waves;
using DG.Tweening;
using IKhom.EventBusSystem.Runtime;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;

namespace ChainDefense.Core
{
    //TODO: TEMP SOLUTION, FIX AND REFACTOR. Move board filling to separate class
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private bool _debugSpeedup;

        public event EventHandler OnGamePaused;
        public event EventHandler OnGameResumed;

        private enum GameplayState
        {
            Gameplay,
            RefillBalls,
            ReorderBalls,
            LevelComplete
        }

        private BoardGrid _boardGrid;
        private ChainValidator _chainValidator;
        private GameplayState _currentState;
        private bool _isRefillCompleted;
        private bool _isReorderCompleted;
        private readonly Dictionary<Ball, Vector3> _ballTargetPositions = new();
        private readonly List<GridPosition> _allOccupiedPositions = new();
        private WaveManager _waveManager;
        private bool _isGamePaused;
        private BallSpawner _ballSpawner;
        private EnemySpawner _enemySpawner;
        private EventBinding<AllWavesCompletedEvent> _waveCompleteBinding;
        private EventBinding<BaseDestroyedEvent> _baseDestroyBinding;

        private void Awake()
        {
            Application.targetFrameRate = 30;
        }

        private void Start()
        {
            _boardGrid = ServiceLocator.ForSceneOf(this).Get<BoardGrid>();
            _chainValidator = ServiceLocator.ForSceneOf(this).Get<ChainValidator>();
            _waveManager = ServiceLocator.ForSceneOf(this).Get<WaveManager>();
            _ballSpawner = ServiceLocator.ForSceneOf(this).Get<BallSpawner>();
            _enemySpawner = ServiceLocator.ForSceneOf(this).Get<EnemySpawner>();
            _chainValidator.OnChainDestroyed += ChainValidator_OnChainDestroyed;

            _waveCompleteBinding = new EventBinding<AllWavesCompletedEvent>(FillBoardWithGuaranteedDistribution);
            EventBus<AllWavesCompletedEvent>.Register(_waveCompleteBinding);
            _baseDestroyBinding = new EventBinding<BaseDestroyedEvent>(OnBaseDestroyed);
            EventBus<BaseDestroyedEvent>.Register(_baseDestroyBinding);
            
            FillBoardWithGuaranteedDistribution();
        }

        private void OnDestroy()
        {
            EventBus<AllWavesCompletedEvent>.Deregister(_waveCompleteBinding);
            EventBus<BaseDestroyedEvent>.Deregister(_baseDestroyBinding);
        }

        private void OnBaseDestroyed()
        {
            Debug.Log("Game Over!");

            _waveManager.Clear();
            _enemySpawner.ClearAllEnemies();
        }

        private void ChainValidator_OnChainDestroyed(object sender, List<Ball> e)
        {
            _isReorderCompleted = false;
            _currentState = GameplayState.ReorderBalls;
        }

        private void Update()
        {
            switch (_currentState)
            {
                case GameplayState.Gameplay:
                    break;
                case GameplayState.ReorderBalls:
                    if (!_isReorderCompleted)
                    {
                        ReorderBallsOnBoard();
                        _isReorderCompleted = true;
                    }

                    break;
                case GameplayState.RefillBalls:
                    if (!_isRefillCompleted)
                    {
                        FillBoardWithGuaranteedDistribution();
                        _isRefillCompleted = true;
                    }

                    break;
                case GameplayState.LevelComplete:
                    break;
            }

            Time.timeScale = _debugSpeedup ? 2f : 1f;
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
            _currentState = GameplayState.RefillBalls;
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

        public void SwitchPauseState()
        {
            _isGamePaused = !_isGamePaused;
            if (_isGamePaused)
            {
                Time.timeScale = 0f;
                OnGamePaused?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Time.timeScale = 1f;
                OnGameResumed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}