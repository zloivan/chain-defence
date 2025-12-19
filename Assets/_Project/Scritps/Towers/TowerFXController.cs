using System;
using System.Collections.Generic;
using ChainDefense.Enemies;
using ChainDefense.Events;
using IKhom.EventBusSystem.Runtime;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;

namespace ChainDefense.Towers
{
    [Serializable]
    public struct ProjectileSpeedSettings
    {
        public float ProjectileSpeed;
        public TowerAttackType AttackType;
    }

    public class TowerFXController : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private List<ProjectileSpeedSettings> _projectileSpeed = new();

        private readonly List<Projectile> _activeProjectiles = new();
        private ProjectileSpawner _projectileSpawner;
        private EventBinding<TowerAttackEvent> _towerAttackBinding;


        private void Start()
        {
            _projectileSpawner = ServiceLocator.ForSceneOf(this).Get<ProjectileSpawner>();
            
            _towerAttackBinding = new EventBinding<TowerAttackEvent>(Tower_OnAnyTowerBeginAttack);
            EventBus<TowerAttackEvent>.Register(_towerAttackBinding);
        }

        private void OnDestroy() =>
            EventBus<TowerAttackEvent>.Deregister(_towerAttackBinding);

        private void Tower_OnAnyTowerBeginAttack(TowerAttackEvent attackInfo)
        {
            var config = attackInfo.Tower.GetTowerConfig();
            const float TOWER_HEIGHT = 3.7f;
            const float BARREL_FORWARD_OFFSET = 3f; // Adjust this value based on your tower model

            // Get the direction to the target
            var directionToTarget = (attackInfo.TargetEnemy.transform.position - attackInfo.Tower.transform.position)
                .normalized;

            // Calculate barrel position: base position + height + forward offset in the direction of the target
            var spawnPos = attackInfo.Tower.transform.position
                           + Vector3.up * TOWER_HEIGHT
                           + directionToTarget * BARREL_FORWARD_OFFSET;

            SpawnProjectile(config.AttackType, spawnPos, attackInfo.TargetEnemy, attackInfo.Tower);
        }

        private void SpawnProjectile(TowerAttackType attackType, Vector3 startPos, Enemy target, Tower sourceTower)
        {
            if (_projectileSpawner == null) return;

            var projectile = _projectileSpawner.SpawnProjectile(attackType, startPos);

            if (projectile == null)
                return;

            projectile.Initialize(target, GetProjectileSpeed(attackType), sourceTower);
            _activeProjectiles.Add(projectile);
        }

        private float GetProjectileSpeed(TowerAttackType attackType)
        {
            var settings = _projectileSpeed.Find(s => s.AttackType == attackType);
            return settings.ProjectileSpeed;
        }

        private void Update()
        {
            for (var i = _activeProjectiles.Count - 1; i >= 0; i--)
            {
                if (_activeProjectiles[i] == null || _activeProjectiles[i].IsComplete())
                {
                    _activeProjectiles.RemoveAt(i);
                }
            }
        }
    }
}