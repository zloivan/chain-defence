using System;
using IKhom.GridSystems._Samples.LevelGrid;
using UnityEngine;
using UnityEngine.Serialization;

namespace IKhom.GridSystems._Samples.helpers
{
    [SelectionBase]
    public class Ball : MonoBehaviour
    {
        [FormerlySerializedAs("_ballColor")] [SerializeField]
        private BallSO _ball;

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

        public void DestroyBall()//TODO: Return to pool
        {
            _ballSpawner.ReturnBall(this);
           // Destroy(gameObject);
        }
    }
}