using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Map", menuName = "Configs/Map SO", order = 0)]
    public class MapSO : ScriptableObject
    {
        public string MapName;
        public GameObject PathPrefab;
        public Material GroundMaterial;
    }
}