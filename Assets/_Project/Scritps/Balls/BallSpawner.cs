using System;
using System.Collections.Generic;
using ChainDefense.GameGrid;
using ChainDefense.GridSystem.core;
using UnityEngine;

namespace ChainDefense.Balls
{
    public class BallSpawner : MonoBehaviour
    {
        public static BallSpawner Instance { get; private set; }

        [SerializeField] private List<BallSO> _ballConfigList;
        [SerializeField] private Transform _parent;

        private BoardGrid _boardGrid;

        private void Awake()
        {
            Instance = this;

            if (_parent == null)
            {
                _parent = transform;
            }
        }

        private void Start()
        {
            _boardGrid = BoardGrid.Instance;
        }

        public Ball SpawnBallAtGridPosition(int ballIndex, GridPosition position) =>
            SpawnBall(ballIndex, position);

        //TODO: Add pool
        public void ReturnBall(Ball ball) =>
            Destroy(ball.gameObject);

        //TODO: Add pool
        private Ball SpawnBall(int ballIndex, GridPosition gridPosition)
        {
            var worldPosition = _boardGrid.GetWorldPosition(gridPosition);
            var ballGameObject = Instantiate(_ballConfigList[ballIndex].BallPrefab, worldPosition, Quaternion.identity,
                _parent);
            var ball = ballGameObject.GetComponent<Ball>();
            ball.SetupSpawner(this);
            return ball;
        }
    }
}