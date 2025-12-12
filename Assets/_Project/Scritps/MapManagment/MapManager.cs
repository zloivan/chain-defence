using System.Collections.Generic;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;

namespace DefaultNamespace
{
    public class MapManager : SingletonBehaviour<MapManager>
    {
        [SerializeField] private List<MapSO> _mapsConfigList;
        
        public GameObject GetPathPrefab(int mapIndex) =>
            _mapsConfigList[mapIndex].PathPrefab;
    }
}