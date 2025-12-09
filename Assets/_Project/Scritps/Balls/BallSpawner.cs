using System.Collections.Generic;
using ChainDefense.GameGrid;
using ChainDefense.GridSystem.core;
using UnityEngine;

namespace ChainDefense.Balls
{
    public class BallSpawner : MonoBehaviour
    {
        [SerializeField] private List<BallSO> _ballConfigList;

        private BoardGrid _boardGrid;

        private void Awake() =>
            _boardGrid = BoardGrid.Instance;

        public void SpawnBallAtGridPosition(int ballIndex, GridPosition position) =>
            SpawnBall(ballIndex, position);

        //TODO: Add pool
        public void ReturnBall(Ball ball) =>
            Destroy(ball.gameObject);

        //TODO: Add pool
        private void SpawnBall(int ballIndex, GridPosition gridPosition)
        {
            var worldPosition = _boardGrid.GetWorldPosition(gridPosition);
            var ball = Instantiate(_ballConfigList[ballIndex].BallPrefab, worldPosition, Quaternion.identity);
            ball.GetComponent<Ball>().Setup(this);
        }
    }
}