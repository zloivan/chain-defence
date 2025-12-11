using UnityEngine;
using UnityEngine.Serialization;

namespace ChainDefense.Towers
{
    [CreateAssetMenu(fileName = "Tower", menuName = "Configs/Tower SO", order = 0)]
    public class TowerSO : ScriptableObject
    {
        public int BaseDamage;
        [FormerlySerializedAs("AttackRange")] public float BaseAttackRange;
        [FormerlySerializedAs("AttackSpeed")] public float BaseAttackSpeed;
        public GameObject TowerPrefab;
        public LevelModifierSO _levelingModifier;
    }
}