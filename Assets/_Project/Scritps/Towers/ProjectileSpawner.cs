using UnityEngine;
using UnityEngine.Pool;

namespace ChainDefense.Towers
{
    public class ProjectileSpawner : MonoBehaviour
    {
        [Header("Projectile Prefabs")]
        [SerializeField] private GameObject _fireProjectilePrefab;
        [SerializeField] private GameObject _iceProjectilePrefab;
        [SerializeField] private GameObject _poisonProjectilePrefab;
        [SerializeField] private GameObject _lightningProjectilePrefab;

        [Header("Impact Effect Prefabs")]
        [SerializeField] private GameObject _impactExplosionPrefab;
        [SerializeField] private GameObject _iceSlowAuraPrefab;
        [SerializeField] private GameObject _poisonCloudPrefab;
        [SerializeField] private GameObject _chainLightningArcPrefab;

        [Header("Pool Settings")]
        [SerializeField] private int _defaultPoolCapacity = 10;
        [SerializeField] private int _maxPoolSize = 50;

        [Header("Parent Transform")]
        [SerializeField] private Transform _projectileParent;
        [SerializeField] private Transform _impactParent;

        private ObjectPool<Projectile> _fireProjectilePool;
        private ObjectPool<Projectile> _iceProjectilePool;
        private ObjectPool<Projectile> _poisonProjectilePool;
        private ObjectPool<Projectile> _lightningProjectilePool;

        private ObjectPool<GameObject> _impactExplosionPool;
        private ObjectPool<GameObject> _iceSlowAuraPool;
        private ObjectPool<GameObject> _poisonCloudPool;
        private ObjectPool<GameObject> _chainLightningArcPool;

        private void Awake()
        {
            if (_projectileParent == null)
                _projectileParent = transform;

            if (_impactParent == null)
                _impactParent = transform;

            InitializePools();
        }

        private void InitializePools()
        {
            _fireProjectilePool = CreateProjectilePool(_fireProjectilePrefab);
            _iceProjectilePool = CreateProjectilePool(_iceProjectilePrefab);
            _poisonProjectilePool = CreateProjectilePool(_poisonProjectilePrefab);
            _lightningProjectilePool = CreateProjectilePool(_lightningProjectilePrefab);

            _impactExplosionPool = CreateImpactPool(_impactExplosionPrefab);
            _iceSlowAuraPool = CreateImpactPool(_iceSlowAuraPrefab);
            _poisonCloudPool = CreateImpactPool(_poisonCloudPrefab);
            _chainLightningArcPool = CreateImpactPool(_chainLightningArcPrefab);
        }

        private ObjectPool<Projectile> CreateProjectilePool(GameObject prefab)
        {
            if (prefab == null) return null;

            return new ObjectPool<Projectile>(
                createFunc: () => CreateProjectile(prefab),
                actionOnGet: OnGetProjectile,
                actionOnRelease: OnReleaseProjectile,
                actionOnDestroy: projectile => Destroy(projectile.gameObject),
                collectionCheck: true,
                defaultCapacity: _defaultPoolCapacity,
                maxSize: _maxPoolSize
            );
        }

        private ObjectPool<GameObject> CreateImpactPool(GameObject prefab)
        {
            if (prefab == null) return null;

            return new ObjectPool<GameObject>(
                createFunc: () => CreateImpactEffect(prefab),
                actionOnGet: OnGetImpact,
                actionOnRelease: OnReleaseImpact,
                actionOnDestroy: Destroy,
                collectionCheck: true,
                defaultCapacity: _defaultPoolCapacity,
                maxSize: _maxPoolSize
            );
        }

        private Projectile CreateProjectile(GameObject prefab)
        {
            var projectileGo = Instantiate(prefab, _projectileParent);
            var projectile = projectileGo.GetComponent<Projectile>();

            if (projectile == null)
            {
                projectile = projectileGo.AddComponent<Projectile>();
            }

            return projectile;
        }

        private GameObject CreateImpactEffect(GameObject prefab) =>
            Instantiate(prefab, _impactParent);

        private void OnGetProjectile(Projectile projectile) =>
            projectile.gameObject.SetActive(true);

        private void OnReleaseProjectile(Projectile projectile) =>
            projectile.gameObject.SetActive(false);

        private void OnGetImpact(GameObject impact) =>
            impact.SetActive(true);

        private void OnReleaseImpact(GameObject impact) =>
            impact.SetActive(false);

        public Projectile SpawnProjectile(TowerAttackType attackType, Vector3 position)
        {
            var projectilePool = attackType switch
            {
                TowerAttackType.SingleTarget => _fireProjectilePool,
                TowerAttackType.Slow => _iceProjectilePool,
                TowerAttackType.AOE => _poisonProjectilePool,
                TowerAttackType.Chain => _lightningProjectilePool,
                _ => null,
            };

            var impactPool = attackType switch
            {
                TowerAttackType.SingleTarget => _impactExplosionPool,
                TowerAttackType.Slow => _iceSlowAuraPool,
                TowerAttackType.AOE => _poisonCloudPool,
                TowerAttackType.Chain => _chainLightningArcPool,
                _ => null,
            };

            if (projectilePool == null) 
                return null;

            var projectile = projectilePool.Get();
            projectile.transform.position = position;
            projectile.transform.rotation = Quaternion.identity;
            projectile.SetupSpawner(this, projectilePool, impactPool);

            return projectile;
        }

        public void ReturnProjectile(Projectile projectile, ObjectPool<Projectile> pool)
        {
            if (pool != null)
                pool.Release(projectile);
            else
                Destroy(projectile.gameObject);
        }

        public GameObject SpawnImpactEffect(ObjectPool<GameObject> impactPool, Vector3 position)
        {
            if (impactPool == null) 
                return null;

            var impact = impactPool.Get();
            impact.transform.position = position;
            impact.transform.rotation = Quaternion.identity;

            return impact;
        }

        public void ReturnImpactEffect(GameObject impact, ObjectPool<GameObject> pool)
        {
            if (pool != null)
                pool.Release(impact);
            else
                Destroy(impact);
        }
    }
}