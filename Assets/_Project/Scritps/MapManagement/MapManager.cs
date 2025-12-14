using System.Collections.Generic;
using ChainDefense.PathFinding;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainDefense.MapManagement
{
    public class MapManager : SingletonBehaviour<MapManager>
    {
        [SerializeField] private MapSO _defaultMap;
        [SerializeField] private bool _useMockMap;
        
        private PathManager _pathManager;

        private void Start()
        {
            _pathManager = PathManager.Instance;

            if (_useMockMap)
            {
                LoadMap(_defaultMap);
            }
        }

        public void LoadMap(MapSO currentLevelMap)
        {
            _pathManager.SetupPathPrefab(currentLevelMap.PathPrefab.transform);
            SceneManager.LoadScene(currentLevelMap._environmentScene.name, LoadSceneMode.Additive);
        }
    }
}