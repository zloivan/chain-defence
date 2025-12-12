using System;
using ChainDefense.Enemies;
using UnityEngine;

namespace ChainDefense.Waves
{
    [Serializable]
    public class WaveConfig
    {
        public EnemySO EnemyType;
        public int EnemyCount;
        public float TimeBetweenSpawns;
    }

    [CreateAssetMenu(fileName = "New Wave", menuName = "Configs/Wave SO", order = 0)]
    public class WaveSO : ScriptableObject
    {
        public WaveConfig[] EnemyTypes;
        public float DelayBeforeStarting;
    }
}