using ChainDefense.PathFinding;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChainDefense.MapManagement
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private MapSO _defaultMap;
        [SerializeField] private bool _useMockMap;

        private PathManager _pathManager;

        private void Start()
        {
            _pathManager = ServiceLocator.ForSceneOf(this).Get<PathManager>();

            if (_useMockMap)
            {
                LoadMap(_defaultMap);
            }
        }

        public void LoadMap(MapSO currentLevelMap)
        {
            _pathManager.SetupPathPrefab(currentLevelMap.PathPrefab.transform);
            SceneManager.LoadScene(currentLevelMap.SceneName, LoadSceneMode.Additive);
        }
    }
}