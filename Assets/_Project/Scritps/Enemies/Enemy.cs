using UnityEngine;

namespace ChainDefense.Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemySO _enemySO;
        
        private int _currentHealth;
        private int _currentWaypointIndex;
    }
}