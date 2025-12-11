using System;
using System.Collections.Generic;
using System.Linq;
using ChainDefense.Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChainDefense.Towers
{
    [SelectionBase]
    public class Tower : MonoBehaviour
    {
        private const float DELAY_BETWEEN_SEARCH = .5f;

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
            //TODO: DEBUG
            _currentAttackRange = _towerConfig.BaseAttackRange;
            
            FindTarget();


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

        public void PerformAttack(Enemy enemy)
        {
            enemy.TakeDamage(_currentDamage);
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

        private void OnDrawGizmos()
        {
            if (_currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
            }

            // Optional: Also draw the attack range sphere
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _currentAttackRange);
        }

        public static GameObject SpawnTower(TowerSO config, Vector3 position) =>
            TowerSpawner.Instance.SpawnTower(config, position);
    }
}