using System;
using System.Collections.Generic;
using System.Linq;
using ChainDefense.Balls;
using ChainDefense.ChainManagment;
using UnityEngine;

namespace ChainDefense.Towers
{
    public class TowerManager : MonoBehaviour
    {
        [SerializeField] private List<TowerBallPairSO> _towerBallMapping;

        private ChainValidator _chainValidator;
        private void Start()
        {
            _chainValidator = ChainValidator.Instance;
            
            
            _chainValidator.OnChainDestroyed += ChainValidator_OnChainDestroyed;
        }

        private void ChainValidator_OnChainDestroyed(object sender, List<Ball> e)
        {
            foreach (var ball in e)
            {
                Debug.Log(ball.GetGridPosition());
            }
        }

        private TowerSO GetTowerTypeFromBall(BallSO ballType)
        {
            var pair = _towerBallMapping.FirstOrDefault(p => p.BallConfig == ballType);
            return pair == null ? null : pair.TowerConfig;
        }
        
        
    }
}