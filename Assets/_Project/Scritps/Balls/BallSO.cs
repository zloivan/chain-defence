using UnityEngine;

namespace ChainDefense.Balls
{
    [CreateAssetMenu(fileName = "New Ball Color", menuName = "Configs/Ball SO", order = 0)]
    public class BallSO : ScriptableObject
    {
        public GameObject BallPrefab;
        public string Name;
    }
}