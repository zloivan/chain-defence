using System;
using ChainDefense.GameGrid;
using ChainDefense.GridSystem.core;
using UnityEngine;

namespace ChainDefense.Balls
{
    [SelectionBase]
    public class Ball : MonoBehaviour
    {
        public event EventHandler OnBallSelected;
        public event EventHandler OnBallDeselected;

        [SerializeField] private BallSO _ball;

        private BoardGrid _boardGrid;
        private BallSpawner _ballSpawner;
        private GridPosition _currentGridPosition;

        private void Awake()
        {
            _boardGrid = BoardGrid.Instance;
        }

        public void Setup(BallSpawner spawner)
        {
            _ballSpawner = spawner;
            
            _currentGridPosition = _boardGrid.GetGridPosition(transform.position);
            _boardGrid.PlaceBallAtPosition(_currentGridPosition, this);
        }

        public void UpdateUnitPosition(GridPosition newPosition)//TODO: DRY
        {
            transform.position = _boardGrid.GetWorldPosition(newPosition);
            _boardGrid.MoveBall(_currentGridPosition, newPosition);
            _currentGridPosition = newPosition;
        }

        public BallSO GetBallColorSO() =>
            _ball;

        public override string ToString() =>
            GetBallColorSO().Name;

        public void DestroyBall()
        {
            _boardGrid.RemoveBallAtPosition(_boardGrid.GetGridPosition(transform.position));
            _ballSpawner.ReturnBall(this);
        }

        public void SetSelected() =>
            OnBallSelected?.Invoke(this, EventArgs.Empty);

        public void Deselect() =>
            OnBallDeselected?.Invoke(this, EventArgs.Empty);
    }
}