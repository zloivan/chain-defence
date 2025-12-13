using System.Collections.Generic;
using ChainDefense.MapManagement;
using ChainDefense.Waves;
using UnityEngine;

namespace ChainDefense.LevelManagement
{
    [CreateAssetMenu(fileName = "Level", menuName = "Configs/Leve SO", order = 0)]
    public class LevelSO : ScriptableObject
    {
        public string LevelName;
        public int LevelNumber;
        public MapSO Map;
        public List<WaveSO> Waves;
        public float FirstWaveDelay;
    }
}