using System.Collections.Generic;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;

namespace ChainDefense.MapManagment
{
    public class MapManager : SingletonBehaviour<MapManager>
    {
        [SerializeField] private List<MapSO> _mapsConfigList;
        
        public GameObject GetPathPrefab(int mapIndex) =>
            _mapsConfigList[mapIndex].PathPrefab;
    }
}