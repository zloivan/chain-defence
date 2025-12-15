using ChainDefense.Enemies;
using IKhom.EventBusSystem.Runtime;
using UnityEngine;
using UnityEngine.Pool;

namespace ChainDefense.Towers
{
    public class Projectile : MonoBehaviour
    {
        private const float MIN_DISTANCE_TO_TARGET = 0.1f;

        private Enemy _target;
        private float _speed;
        private ObjectPool<GameObject> _impactPool;
        private ObjectPool<Projectile> _projectilePool;
        private ProjectileSpawner _projectileSpawner;
        private bool _isComplete;
        
        private Tower _sourceTower;

        public void SetupSpawner(ProjectileSpawner spawner, ObjectPool<Projectile> projectilePool,
            ObjectPool<GameObject> impactPool)
        {
            _projectileSpawner = spawner;
            _projectilePool = projectilePool;
            _impactPool = impactPool;
        }

        public void Initialize(Enemy target, float speed, Tower sourceTower)
        {
            _target = target;
            _speed = speed;
            _sourceTower = sourceTower;
            _isComplete = false;
        }

        private void Update()
        {
            if (_isComplete) return;

            // Destroy projectile if target dies while flying
            if (_target == null || _target.GetIsDead())
            {
                DestroySelf();
                return;
            }

            var targetPos = _target.transform.position + Vector3.up * 0.5f;
            var distanceBeforeMoving = Vector3.Distance(targetPos, transform.position);
            var direction = (targetPos - transform.position).normalized;

            transform.position += direction * (_speed * Time.deltaTime);
            var distanceAfterMoving = Vector3.Distance(targetPos, transform.position);

            // Check if we overshot the target
            if (distanceBeforeMoving >= distanceAfterMoving
                && distanceAfterMoving >= MIN_DISTANCE_TO_TARGET)
            {
                return;
            }

            // Hit the target
            transform.position = targetPos;
            OnHitTarget();
            DestroySelf();
        }

        private void OnHitTarget()
        {
            SpawnImpact(_target.transform.position + Vector3.up * 0.5f);
            _sourceTower.ApplyDamageAndEffects(_target);
        }

        private void SpawnImpact(Vector3 position)
        {
            if (_impactPool != null && _projectileSpawner != null)
            {
                var impact = _projectileSpawner.SpawnImpactEffect(_impactPool, position);

                if (impact != null)
                {
                    StartCoroutine(ReturnImpactToPoolAfterDelay(impact, 2f));
                }
            }
        }

        private System.Collections.IEnumerator ReturnImpactToPoolAfterDelay(GameObject impact, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (impact != null && _projectileSpawner != null)
            {
                _projectileSpawner.ReturnImpactEffect(impact, _impactPool);
            }
        }

        public void DestroySelf()
        {
            _isComplete = true;

            if (_projectileSpawner != null)
            {
                _projectileSpawner.ReturnProjectile(this, _projectilePool);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public bool IsComplete() =>
            _isComplete;
    }
}