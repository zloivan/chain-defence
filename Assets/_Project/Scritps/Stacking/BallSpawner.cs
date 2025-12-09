using System;
using System.Collections.Generic;
using IKhom.GridSystems._Samples.LevelGrid;
using IKhom.GridSystems.Runtime.core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IKhom.GridSystems._Samples.helpers
{
    public class BallSpawner : MonoBehaviour
    {
        [SerializeField] private List<BallSO> _ballConfigList;

        private BoardGrid _boardGrid;

        private void Awake() =>
            _boardGrid = BoardGrid.Instance;

        public void SpawnRandomBallAtGridPosition(GridPosition position) //TODO: Add pool
        {
            var ballPrefab = GetRandomBall();

            SpawnBall(ballPrefab, position);
        }

        //TODO: Add pool
        public void ReturnBall(Ball ball) =>
            Destroy(ball.gameObject);

        private BallSO GetRandomBall()
        {
            var randomIndex = Random.Range(0, _ballConfigList.Count);
            var ballPrefab = _ballConfigList[randomIndex];
            return ballPrefab;
        }

        private void SpawnBall(BallSO ballPrefab, GridPosition gridPosition)
        {
            var worldPosition = _boardGrid.GetWorldPosition(gridPosition);
            var ball = Instantiate(ballPrefab.BallPrefab, worldPosition, Quaternion.identity);
            ball.GetComponent<Ball>().Setup(this);
        }
    }
}