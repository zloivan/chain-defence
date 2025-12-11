using System;
using System.Collections.Generic;
using System.Linq;
using ChainDefense.Balls;
using ChainDefense.ChainManagment;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChainDefense.Towers
{
    [Serializable]
    public struct LevelDefinition
    {
        public Vector2Int DestroyedBallRange;
        public int LevelNumber;
    }

    public class TowerManager : MonoBehaviour
    {
        private const int MAX_NUMBER_OF_TOWERS = 10;

        [SerializeField] private List<TowerBallPairSO> _towerBallMappingList;
        [SerializeField] private List<LevelDefinition> _levelMappingList;

        private int _spawnedTowersNumber;
        private ChainValidator _chainValidator;

        private void Start()
        {
            _chainValidator = ChainValidator.Instance;

            _chainValidator.OnChainDestroyed += ChainValidator_OnChainDestroyed;
        }

        //TODO: Add visualization for tower spawn and tower range
        private void ChainValidator_OnChainDestroyed(object sender, List<Ball> destroyedBalls)
        {
            if (_spawnedTowersNumber >= MAX_NUMBER_OF_TOWERS)
                return;

            var indexOfCenterBalls = destroyedBalls.Count / 2;
            var centerBall = destroyedBalls[indexOfCenterBalls];
            var chainCenterPosition = centerBall.GetWorldPosition();
            var towerConfig = GetTowerTypeFromBall(centerBall.GetBallConfig());

            var towerGo = Tower.SpawnTower(towerConfig, chainCenterPosition);
            towerGo.GetComponent<Tower>().SetLevel(GetLevel(destroyedBalls.Count));
            _spawnedTowersNumber++;
        }

        private TowerSO GetTowerTypeFromBall(BallSO ballType)
        {
            var pair = _towerBallMappingList.FirstOrDefault(p => p.BallConfig == ballType);
            return pair == null ? null : pair.TowerConfig;
        }

        private int GetLevel(int destroyedBalls)
        {
            foreach (var levelDefinition in _levelMappingList)
            {
                if (levelDefinition.DestroyedBallRange.x <= destroyedBalls
                    && destroyedBalls <= levelDefinition.DestroyedBallRange.y)
                {
                    return levelDefinition.LevelNumber;
                }
            }

            Debug.LogWarning("Not found level for destroyed balls: " + destroyedBalls);
            return 1;
        }
    }
}