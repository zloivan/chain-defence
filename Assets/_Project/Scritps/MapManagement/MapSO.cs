using UnityEngine;

namespace ChainDefense.MapManagement
{
    [CreateAssetMenu(fileName = "Map", menuName = "Configs/Map SO", order = 0)]
    public class MapSO : ScriptableObject
    {
        public string MapName;
        public GameObject PathPrefab;
        public string SceneName;
    }
}