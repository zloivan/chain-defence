using System;
using ChainDefense.Enemies;
using UnityEngine;
using UnityEngine.Serialization;

namespace IKhom.StateMachineSystem.Runtime
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
        [FormerlySerializedAs("Enemies")] public WaveConfig[] Waves;
        public float DelayBeforeStarting;
    }
}