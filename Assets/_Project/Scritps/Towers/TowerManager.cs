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
        [FormerlySerializedAs("_towerBallMapping")] [SerializeField]
        private List<TowerBallPairSO> _towerBallMappingList;

        [SerializeField] private List<LevelDefinition> _levelMappingList;

        private ChainValidator _chainValidator;

        private void Start()
        {
            _chainValidator = ChainValidator.Instance;

            _chainValidator.OnChainDestroyed += ChainValidator_OnChainDestroyed;
        }

        private void ChainValidator_OnChainDestroyed(object sender, List<Ball> destroyedBalls)
        {
            //find center of destroyed balls
            //build a tower at the center of destroyed balls based on ball type
            var indexOfCenterBalls = destroyedBalls.Count / 2;
            var centerBall = destroyedBalls[indexOfCenterBalls];
            var centerPosition = centerBall.GetWorldPosition();
            var towerConfig = GetTowerTypeFromBall(centerBall.GetBallConfig());

            var towerGo = Tower.SpawnTower(towerConfig, centerPosition);
            towerGo.GetComponent<Tower>().SetLevel(GetLevel(destroyedBalls.Count));
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