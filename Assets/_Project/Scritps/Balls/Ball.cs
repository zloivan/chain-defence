using System;
using ChainDefense.GameGrid;
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

        private void Start()
        {
            _boardGrid = BoardGrid.Instance;
            UpdateBoardPosition();
        }

        public void Setup(BallSpawner spawner) =>
            _ballSpawner = spawner;

        private void UpdateBoardPosition()
        {
            var boardposition = _boardGrid.GetGridPosition(transform.position);

            var isSlotEmpty = _boardGrid.IsSlotEmpty(boardposition);
            if (!_boardGrid.IsValidGridPosition(boardposition))
            {
                Debug.LogWarning("Outside of board bounds", this);
                return;
            }

            if (isSlotEmpty)
            {
                //Center to cell
                transform.position = _boardGrid.GetWorldPosition(boardposition);
                _boardGrid.PlaceBallAtPosition(boardposition, this);
            }
            else
            {
                Debug.LogWarning("Slot already occupied", this);
            }
        }

        public BallSO GetBallColorSO() =>
            _ball;

        public override string ToString() =>
            GetBallColorSO().Name;

        public void DestroyBall() =>
            _ballSpawner.ReturnBall(this);

        public void SetSelected() =>
            OnBallSelected?.Invoke(this, EventArgs.Empty);

        public void Deselect() =>
            OnBallDeselected?.Invoke(this, EventArgs.Empty);
    }
}