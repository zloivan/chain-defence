using System.Collections.Generic;
using System.Linq;
using ChainDefense.Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChainDefense.Towers
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private TowerSO _towerConfig;
        [SerializeField] private TextMeshPro _levelNumberLabel; //TODO: DEBUG
        
        [FormerlySerializedAs("_levelModifierSO")] [SerializeField]
        private List<LevelModifier> _levelModifierSOList;

        private int _currentDamage;
        private float _currentAttackRange;
        private float _currentAttackSpeed;

        private int _currentLevel;
        private float _attackCooldownTimer;

        private Enemy _currentTarget;
        private List<LevelModifier> _levelModifiersList = new();

        public static GameObject SpawnTower(TowerSO config, Vector3 position) =>
            TowerSpawner.Instance.SpawnTower(config, position);

        public void SelfDestruct() =>
            TowerSpawner.Instance.ReturnTower(this);

        public void SetLevel(int level)
        {
            _currentLevel = level;
            
            ClearModifiers();
            ApplyModifiers(GetModifiers(level));

            _levelNumberLabel.text = _currentLevel.ToString();//TODO: DEBUG
        }

        private void ApplyModifiers(List<LevelModifier> modifiers)
        {
            _levelModifiersList.AddRange(modifiers);
        }

        private void ClearModifiers()
        {
            _levelModifiersList.Clear();
        }

        private List<LevelModifier> GetModifiers(int levelIndexModifier) =>
            _levelModifierSOList.Where(l => l.LevelIndexModifier <= levelIndexModifier).ToList();
    }
}