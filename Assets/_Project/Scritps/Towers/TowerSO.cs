using UnityEngine;

namespace ChainDefense.Towers
{
    public enum TowerAttackType
    {
        SingleTarget,
        AOE,
        Slow,
        Chain
    }
    
    [CreateAssetMenu(fileName = "Tower", menuName = "Configs/Tower SO", order = 0)]
    public class TowerSO : ScriptableObject//TODO: Figure out he way to add effects differently per tower not via enum
    {
        public int BaseDamage;
        public float BaseAttackRange;
        public float BaseAttackSpeed;
        public TowerAttackType AttackType;
        public float AoeRadius;
        public float SlowPercentage;
        public int ChainTargets;
        public float SlowDuration;
        public GameObject TowerPrefab;
    }
}