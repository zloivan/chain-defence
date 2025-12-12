using System;
using System.Collections.Generic;
using System.Linq;
using ChainDefense.Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChainDefense.Towers
{
    [Serializable]
    public struct LevelModifier
    {
        public int LevelIndexModifier;
        public float DamageModifier;
        public float AttackSpeedModifier;
        public float AttackRangeModifier;
    }

    [SelectionBase]
    public class Tower : MonoBehaviour
    {
        private const float DELAY_BETWEEN_SEARCH = .5f;

        public class AttackInfoEventArts : EventArgs
        {
            public Enemy TargetEnemy;
            public int DamageDealt;
        }

        public event EventHandler<AttackInfoEventArts> OnTowerFinishAttack;
        public event EventHandler<AttackInfoEventArts> OnTowerBeginAttack;

        [SerializeField] private TowerSO _towerConfig;
        [SerializeField] private TextMeshPro _levelNumberLabel; //TODO: DEBUG
        [SerializeField] private List<LevelModifier> _levelModifierSOList;
        [SerializeField] private LayerMask _enemyLayerMask;

        private int _currentDamage;
        private float _currentAttackRange;
        private float _currentAttackSpeed;

        private int _currentLevel;
        private float _attackCooldownTimer;

        private Enemy _currentTarget;
        private float _findDelayTimer;
        private TowerAttackType _attackType;

        private readonly List<LevelModifier> _levelModifiersList = new();
        private readonly Collider[] _enemyCollidersArray = new Collider[20];
        private readonly List<Enemy> _visibleEnemiesList = new();

        private void Awake()
        {
            _currentDamage = _towerConfig.BaseDamage;
            _currentAttackRange = _towerConfig.BaseAttackRange;
            _currentAttackSpeed = _towerConfig.BaseAttackSpeed;
        }

        private void Update()
        {
            FindTarget();
            HandleAttack();
        }

        private void HandleAttack()
        {
            if (_currentTarget == null)
                return;

            if (_attackCooldownTimer <= _currentAttackSpeed)
            {
                _attackCooldownTimer += Time.deltaTime;
                return;
            }

            _attackCooldownTimer = 0;
            PerformAttack(_currentTarget);
        }

        private void FindTarget() //TODO: look for other way of prioritizing enemies & optimize if needed
        {
            if (_findDelayTimer <= DELAY_BETWEEN_SEARCH)
            {
                _findDelayTimer += Time.deltaTime;
                return;
            }

            _findDelayTimer = 0;

            // If we have a target, check if it's still in range
            //or if target is dead
            if (_currentTarget != null
                && (Vector3.Distance(transform.position, _currentTarget.transform.position) > _currentAttackRange
                    || _currentTarget.GetIsDead()))
            {
                _currentTarget = null;
            }

            if (_currentTarget != null)
                return;

            var numColliders = Physics.OverlapSphereNonAlloc(
                transform.position,
                _currentAttackRange,
                _enemyCollidersArray,
                _enemyLayerMask
            );

            //No visible enemies
            if (numColliders == 0)
                return;

            _visibleEnemiesList.Clear();
            for (var i = 0; i < numColliders; i++)
            {
                var enemy = _enemyCollidersArray[i].GetComponent<Enemy>();
                _visibleEnemiesList.Add(enemy);
            }

            var maxWaypointIndex = _visibleEnemiesList.Max(e => e.GetWaypointIndex());
            var enemiesClosestToBase =
                _visibleEnemiesList.Where(e => e.GetWaypointIndex() == maxWaypointIndex).ToList();

            Enemy closestEnemy = null;
            var closestDistance = float.MaxValue;
            foreach (var enemy in enemiesClosestToBase)
            {
                var distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance >= closestDistance)
                    continue;

                closestDistance = distance;
                closestEnemy = enemy;
            }

            _currentTarget = closestEnemy;
        }

        public void SetLevel(int level)
        {
            _currentLevel = level;

            ClearModifiers();
            ApplyModifiers(GetModifiers(level));

            _levelNumberLabel.text = _currentLevel.ToString(); //TODO: DEBUG
        }

        private void ApplyAttackType(Enemy target, TowerSO config)
        {
            switch (config.AttackType)
            {
                case TowerAttackType.SingleTarget:
                    break;
                case TowerAttackType.AOE:

                    var numberOfTargets = Physics.OverlapSphereNonAlloc(
                        target.transform.position,
                        config.AoeRadius,
                        _enemyCollidersArray,
                        _enemyLayerMask
                    );

                    if (numberOfTargets > 1)
                    {
                        for (var i = 0; i < numberOfTargets; i++)
                        {
                            var enemy = _enemyCollidersArray[i].GetComponent<Enemy>();
                            if (enemy == target || enemy == null)
                                continue;
                            
                            var aoeDamage = Mathf.CeilToInt(_currentDamage * config.AoeDamagePercentage);
                            enemy.TakeDamage(aoeDamage);
                        }
                    }

                    break;
                case TowerAttackType.Slow:
                    target.ApplySlow(config.SlowPercentage, config.SlowDuration).Forget();
                    break;
                case TowerAttackType.Chain:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(config), config, null);
            }
        }

        public void PerformAttack(Enemy enemy)
        {
            OnTowerBeginAttack?.Invoke(this, new AttackInfoEventArts
            {
                TargetEnemy = _currentTarget,
                DamageDealt = _currentDamage
            });

            enemy.TakeDamage(_currentDamage);
            ApplyAttackType(enemy, _towerConfig);

            OnTowerFinishAttack?.Invoke(this, new AttackInfoEventArts
            {
                TargetEnemy = _currentTarget,
                DamageDealt = _currentDamage
            });
        }

        private void ApplyModifiers(List<LevelModifier> modifiers)
        {
            _levelModifiersList.AddRange(modifiers);

            foreach (var levelModifier in _levelModifiersList)
            {
                _currentDamage = Mathf.CeilToInt(_currentDamage * levelModifier.DamageModifier);
                _currentAttackRange = Mathf.CeilToInt(_currentAttackRange * levelModifier.AttackRangeModifier);
                _currentAttackSpeed = Mathf.CeilToInt(_currentAttackSpeed * levelModifier.AttackSpeedModifier);
            }
        }

        private void ClearModifiers()
        {
            _levelModifiersList.Clear();

            _currentDamage = _towerConfig.BaseDamage;
            _currentAttackRange = _towerConfig.BaseAttackRange;
            _currentAttackSpeed = _towerConfig.BaseAttackSpeed;
        }

        private List<LevelModifier> GetModifiers(int levelIndexModifier) =>
            _levelModifierSOList.Where(l => l.LevelIndexModifier <= levelIndexModifier).ToList();

        private void OnDrawGizmosSelected()
        {
            if (_currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
            }

            // Draw the attack range sphere
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _currentAttackRange);

            // Draw AOE radius at target position if this is an AOE tower
            if (_towerConfig != null && _towerConfig.AttackType == TowerAttackType.AOE && _currentTarget != null)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Orange
                Gizmos.DrawWireSphere(_currentTarget.transform.position, _towerConfig.AoeRadius);
            }
        }

        public static GameObject SpawnTower(TowerSO config, Vector3 position) =>
            TowerSpawner.Instance.SpawnTower(config, position);
    }
}