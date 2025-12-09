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

        private void Awake()
        {
            _boardGrid = BoardGrid.Instance;
        }

        public void SpawnRandomBallAtGridPosition(GridPosition position)//TODO: Add pool
        {
            var randomIndex = Random.Range(0, _ballConfigList.Count);
            var ballPrefab = _ballConfigList[randomIndex];
            var worldPosition = _boardGrid.GetWorldPosition(position);
            Instantiate(ballPrefab.BallPrefab, worldPosition, Quaternion.identity);
        }
    }
}