using ChainDefense.Balls;
using ChainDefense.ChainManagment;
using ChainDefense.Enemies;
using ChainDefense.GameGrid;
using ChainDefense.LevelManagement;
using ChainDefense.PathFinding;
using ChainDefense.PlayerBase;
using ChainDefense.Towers;
using ChainDefense.Waves;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;

namespace ChainDefense.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Core Systems")]
        [SerializeField] private InputController _inputController;
        [SerializeField] private BoardGrid _boardGrid;
        [SerializeField] private ChainValidator _chainValidator;
        [SerializeField] private GameplayController _gameplayController;
        
        [Header("Ball System")]
        [SerializeField] private BallSpawner _ballSpawner;
        
        [Header("Tower Systems")]
        [SerializeField] private TowerSpawner _towerSpawner;
        [SerializeField] private ProjectileSpawner _projectileSpawner;
        
        [Header("Enemy Systems")]
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private PathManager _pathManager;
        
        [Header("Wave & Level")]
        [SerializeField] private WaveManager _waveManager;
        [SerializeField] private BaseManager _baseManager;
        [SerializeField] private LevelManager _levelManager;
        
        private void Awake() =>
            RegisterServices();

        private void RegisterServices()
        {
            var locator = ServiceLocator.ForSceneOf(this);
            
            locator.Register(_inputController);
            locator.Register(_boardGrid);
            locator.Register(_chainValidator);
            
            locator.Register(_ballSpawner);
            
            locator.Register(_towerSpawner);
            locator.Register(_projectileSpawner);
            
            locator.Register(_enemySpawner);
            locator.Register(_pathManager);
            
            locator.Register(_waveManager);
            locator.Register(_baseManager);
            locator.Register(_levelManager);
            locator.Register(_gameplayController);
        }
    }
}