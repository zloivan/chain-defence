using UnityEngine;

namespace ChainDefense.Enemies
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "Configs/Enemy SO", order = 0)]
    public class EnemySO : ScriptableObject
    {
        public int MaxHealth;
        public float MoveSpeed;
        public int BaseDamage;
        public GameObject EnemyPrefab;
    }
}