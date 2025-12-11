using UnityEngine;

namespace ChainDefense.Towers
{
    [CreateAssetMenu(fileName = "Tower", menuName = "Configs/Tower SO", order = 0)]
    public class TowerSO : ScriptableObject
    {
        public int BaseDamage;
        public float AttackRange;
        public float AttackSpeed;
    }
}