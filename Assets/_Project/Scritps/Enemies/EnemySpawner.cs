using UnityEngine;

namespace ChainDefense.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance { get; private set; }

        [SerializeField] private Transform _parent;

        private void Awake()
        {
            Instance = this;
            if (_parent == null)
                _parent = transform;
        }

        public GameObject SpawnEnemy(EnemySO enemySO, Vector3 position) =>
            Instantiate(enemySO.EnemyPrefab, position, Quaternion.identity, _parent);

        public void ReturnEnemy(Enemy enemy) =>
            Destroy(enemy.gameObject);
    }
}