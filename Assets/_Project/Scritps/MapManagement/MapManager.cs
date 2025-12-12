using System.Collections.Generic;
using ChainDefense.PathFinding;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;

namespace ChainDefense.MapManagement
{
    public class MapManager : SingletonBehaviour<MapManager>
    {
        [SerializeField] private MapSO _defaultMap;
        //TODO: temp, prepare additive scenes with environment
        [SerializeField] private MeshRenderer _ground;

        private PathManager _pathManager;

        private void Start()
        {
            _pathManager = PathManager.Instance;
            
            LoadMap(_defaultMap);
        }

        public void LoadMap(MapSO currentLevelMap)
        {
            _pathManager.SetupPathPrefab(currentLevelMap.PathPrefab.transform);
            _ground.SetMaterials(new List<Material> { currentLevelMap.GroundMaterial });
        }
    }
}