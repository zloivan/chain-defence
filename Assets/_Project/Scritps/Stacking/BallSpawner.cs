using System;
using System.Collections.Generic;
using System.Linq;
using IKhom.GridSystems._Samples.LevelGrid;
using IKhom.GridSystems.Runtime.core;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IKhom.GridSystems._Samples.helpers
{
    public class BallSpawner : MonoBehaviour
    {
        [SerializeField] private List<BallSO> _ballConfigList;

        [ReadOnly][SerializeField]//TODO: DEBUG REMOVE LATER
        private List<int> _numberOfBalls = new();

        private BoardGrid _boardGrid;

        private void Awake()
        {
            _boardGrid = BoardGrid.Instance;
            
            for (var index = 0; index < _ballConfigList.Count; index++)
            {
                _numberOfBalls.Add(0);
            }
        }

        public void SpawnRandomBallAtGridPosition(GridPosition position) =>
            SpawnBall(GetRandomBallIndex(), position);

        public void SpawnBallAtGridPosition(int ballIndex, GridPosition position) =>
            SpawnBall(ballIndex, position);

        //TODO: Add pool
        public void ReturnBall(Ball ball)
        {
            Destroy(ball.gameObject);
            
            var indexOfBall = GetBallIndex(ball.GetBallColorSO());
            if(indexOfBall < 0 || indexOfBall > _ballConfigList.Count -1)
                return;
            
            _numberOfBalls[indexOfBall]--;
        }

        private int GetRandomBallIndex() =>
            Random.Range(0, _ballConfigList.Count);

        //TODO: Add pool
        private void SpawnBall(int ballIndex, GridPosition gridPosition)
        {
            var worldPosition = _boardGrid.GetWorldPosition(gridPosition);
            var ball = Instantiate(_ballConfigList[ballIndex].BallPrefab, worldPosition, Quaternion.identity);
            ball.GetComponent<Ball>().Setup(this);

            
            _numberOfBalls[ballIndex]++;
        }

        private int GetBallIndex(BallSO ballSO) =>
            _ballConfigList.IndexOf(ballSO);
    }
}