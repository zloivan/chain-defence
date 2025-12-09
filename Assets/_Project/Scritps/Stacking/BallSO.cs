using UnityEngine;

namespace IKhom.GridSystems._Samples.helpers
{
    
    
    [CreateAssetMenu(fileName = "New Ball Color", menuName = "Config/BallSO", order = 0)]
    public class BallSO : ScriptableObject
    {
        public GameObject BallPrefab;
        public string Name;
    }
}