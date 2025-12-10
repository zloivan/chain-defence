using System;
using ChainDefense.GameGrid;
using ChainDefense.GridSystem.core;
using DG.Tweening;
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

        private void Start()
        {
            _currentGridPosition = _boardGrid.GetGridPosition(transform.position);
            _boardGrid.AddBallToPosition(_currentGridPosition, this);
        }

        private void OnDisable() =>
            transform.DOKill();

        public void SetupSpawner(BallSpawner spawner)
        {
            _ballSpawner = spawner;
        }

        private void Update()
        {
            UpdateUnitPosition();
        }

        public void UpdateUnitPosition()
        {
            var newGridPosition = _boardGrid.GetGridPosition(transform.position);
            
            if (newGridPosition == _currentGridPosition)
                return;

            var oldGridPosition = _currentGridPosition;
            _currentGridPosition = newGridPosition;
            
            _boardGrid.MoveBall(oldGridPosition, newGridPosition, this);
        }

        public BallSO GetBallColorSO() =>
            _ball;

        public override string ToString() =>
            GetBallColorSO().Name;

        public void DestroyBall()
        {
            _boardGrid.RemoveBallAtPosition(_boardGrid.GetGridPosition(transform.position), this);
            _ballSpawner.ReturnBall(this);
        }

        public void SetSelected() =>
            OnBallSelected?.Invoke(this, EventArgs.Empty);

        public void Deselect() =>
            OnBallDeselected?.Invoke(this, EventArgs.Empty);

        public GridPosition GetGridPosition() =>
            _currentGridPosition;
    }
}