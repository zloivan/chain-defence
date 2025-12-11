using UnityEngine;

namespace ChainDefense.Towers
{
    public class TowerSpawner : MonoBehaviour
    {
        public static TowerSpawner Instance { get; private set; }

        [SerializeField] private Transform _parent;

        private void Awake()
        {
            Instance = this;
            if (_parent == null)
                _parent = transform;
        }

        public GameObject SpawnTower(TowerSO towerConfig, Vector3 position) =>
            Instantiate(towerConfig.TowerPrefab, position, Quaternion.identity, _parent);

        public void ReturnTower(Tower tower)//TODO: Pooling
        {
            Destroy(tower.gameObject);
        }
    }
}