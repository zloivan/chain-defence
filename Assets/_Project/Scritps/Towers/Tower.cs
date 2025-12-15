using System;
using System.Collections.Generic;
using System.Linq;
using ChainDefense.Enemies;
using ChainDefense.Events;
using Cysharp.Threading.Tasks;
using IKhom.EventBusSystem.Runtime;
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

    public class AttackInfoEventArts : EventArgs
    {
        public Enemy TargetEnemy;
        public int DamageDealt;
    }

    [SelectionBase]
    public class Tower : MonoBehaviour
    {
        private const float DELAY_BETWEEN_SEARCH = .5f;


        //STATIC
        public static GameObject SpawnTower(TowerSO config, Vector3 position) =>
            TowerSpawner.Instance.SpawnTower(config, position);

        public static event EventHandler OnAnyTowerSpawned
        {
            add => TowerSpawner.Instance.OnAnyTowerSpawned += value;
            remove => TowerSpawner.Instance.OnAnyTowerSpawned -= value;
        }

        public static event EventHandler<AttackInfoEventArts> OnAnyTowerBeginAttack;

        //INSTANCE EVENTS
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

        //Cache for performance
        private readonly Collider[] _enemyCollidersArray = new Collider[20];
        private readonly List<Enemy> _visibleEnemiesList = new();
        private readonly List<Enemy> _enemiesClosestToBase = new();
        private Enemy _closestEnemy;

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

        private void FindTarget()
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

                //Dont attack outside of screen
                if (enemy.GetWaypointIndex() < 1)
                {
                    continue;
                }

                _visibleEnemiesList.Add(enemy);
            }

            if (_visibleEnemiesList.Count == 0)
                return;

            var maxWaypointIndex = int.MinValue;
            _enemiesClosestToBase.Clear();

            //Find the enemy with the highest waypoint index
            foreach (var enemy in _visibleEnemiesList)
            {
                var waypointIndex = enemy.GetWaypointIndex();

                if (waypointIndex > maxWaypointIndex)
                {
                    maxWaypointIndex = waypointIndex;
                    _enemiesClosestToBase.Clear();
                    _enemiesClosestToBase.Add(enemy);
                }
                else if (waypointIndex == maxWaypointIndex)
                {
                    _enemiesClosestToBase.Add(enemy);
                }
            }

            //Find the closest enemy
            _closestEnemy = null;
            var closestDistance = float.MaxValue;
            foreach (var enemy in _enemiesClosestToBase)
            {
                var distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance >= closestDistance)
                    continue;

                closestDistance = distance;
                _closestEnemy = enemy;
            }

            _currentTarget = _closestEnemy;
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
                    ApplyAOE(target, config);
                    break;
                case TowerAttackType.Slow:
                    target.ApplySlow(config.SlowPercentage, config.SlowDuration).Forget();
                    break;
                case TowerAttackType.Chain:
                    ApplyChainDamage(target, config).Forget();

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(config), config, null);
            }
        }

        private void ApplyAOE(Enemy target, TowerSO config)
        {
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
        }

        private async UniTask ApplyChainDamage(Enemy primaryTarget, TowerSO config)
        {
            //For not hitting same target
            var hitEnemies = new HashSet<Enemy> { primaryTarget };
            var currentTarget = primaryTarget;
            var currentDamageMultiplier = config.ChainDamagePercentage;

            for (var i = 0; i < config.ChainTargets - 1; i++)
            {
                if (config.ChainBounceDelay > 0)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(config.ChainBounceDelay));
                }

                if (currentTarget == null || currentTarget.GetIsDead())
                    break;

                var numberOfTargets = Physics.OverlapSphereNonAlloc(
                    currentTarget.transform.position,
                    config.ChainRadius,
                    _enemyCollidersArray,
                    _enemyLayerMask
                );

                if (numberOfTargets == 0)
                    break;

                Enemy nearestEnemy = null;
                var nearestDistance = float.MaxValue;

                for (var j = 0; j < numberOfTargets; j++)
                {
                    var enemy = _enemyCollidersArray[j].GetComponent<Enemy>();

                    if (enemy == null || hitEnemies.Contains(enemy) || enemy.GetIsDead())
                        continue;

                    var distance = Vector3.Distance(currentTarget.transform.position, enemy.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEnemy = enemy;
                    }
                }

                if (nearestEnemy == null)
                    break;

                var chainDamage = Mathf.CeilToInt(_currentDamage * currentDamageMultiplier);
                nearestEnemy.TakeDamage(chainDamage);

                hitEnemies.Add(nearestEnemy);
                currentTarget = nearestEnemy;

                currentDamageMultiplier *= config.ChainDamagePercentage;
            }
        }

        private void PerformAttack(Enemy enemy)
        {
            OnTowerBeginAttack?.Invoke(this, new AttackInfoEventArts
            {
                TargetEnemy = _currentTarget,
                DamageDealt = _currentDamage
            });

            EventBus<TowerAttackEvent>.Raise(new TowerAttackEvent(_towerConfig.AttackType));

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
                _currentAttackRange *= levelModifier.AttackRangeModifier;
                _currentAttackSpeed *= levelModifier.AttackSpeedModifier;
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

            // Draw Chain radius at target position if this is a Chain tower
            if (_towerConfig != null && _towerConfig.AttackType == TowerAttackType.Chain && _currentTarget != null)
            {
                Gizmos.color = new Color(0f, 1f, 1f, 0.5f); // Cyan
                Gizmos.DrawWireSphere(_currentTarget.transform.position, _towerConfig.ChainRadius);
            }
        }

        public TowerSO GetTowerConfig() =>
            _towerConfig;
    }
}