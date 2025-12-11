using ChainDefense.Enemies;
using UnityEngine;

namespace ChainDefense.Towers
{
    
    public class Tower : MonoBehaviour
    {
        [SerializeField] private TowerSO _towerConfig;
       
        private int _level;
        private float _attackCooldown;
        private Enemy _currentTarget;

        public static GameObject SpawnTower(TowerSO config, Vector3 position) =>
            TowerSpawner.Instance.SpawnTower(config, position);

        public void SelfDestruct() =>
            TowerSpawner.Instance.ReturnTower(this);
    }
}