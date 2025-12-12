using UnityEngine;
using UnityEngine.Serialization;

namespace ChainDefense.Enemies
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "Configs/Enemy SO", order = 0)]
    public class EnemySO : ScriptableObject
    {
        public int MaxHealth;
        [FormerlySerializedAs("MoveSpeed")] public float BaseMoveSpeed;
        public int BaseDamage;
        public GameObject EnemyPrefab;
    }
}