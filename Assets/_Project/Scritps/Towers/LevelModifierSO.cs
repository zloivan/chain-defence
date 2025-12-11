using System;
using UnityEngine;

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
    
    [CreateAssetMenu(fileName = "LevelInfo", menuName = "Configs/Tower Level Info SO", order = 0)]
    public class LevelModifierSO : ScriptableObject
    {
        public LevelModifier _levelModifier;
    }
}