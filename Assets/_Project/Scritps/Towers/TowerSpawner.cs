using System;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;

namespace ChainDefense.Towers
{
    public class TowerSpawner : SingletonBehaviour<TowerSpawner>
    {
        [SerializeField] private Transform _parent;
        public event EventHandler OnAnyTowerSpawned;

        protected override void Awake()
        {
            if (_parent == null)
                _parent = transform;
        }

        public GameObject SpawnTower(TowerSO towerConfig, Vector3 position) =>
            Instantiate(towerConfig.TowerPrefab, position, Quaternion.identity, _parent);

        public void ReturnTower(Tower tower) //TODO: Pooling
        {
            Destroy(tower.gameObject);
        }
    }
}