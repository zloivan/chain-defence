using ChainDefense.Balls;
using UnityEngine;

namespace ChainDefense.Towers
{
    [CreateAssetMenu(fileName = "Pair", menuName = "Config/Tower Ball Pair SO", order = 0)]
    public class TowerBallPairSO : ScriptableObject
    {
        public TowerSO TowerConfig;
        public BallSO BallConfig;
    }
}