using System.Collections.Generic;
using ChainDefense.MapManagment;
using ChainDefense.Waves;
using UnityEngine;

namespace ChainDefense.LevelManagment
{
    [CreateAssetMenu(fileName = "Level", menuName = "Configs/Leve SO", order = 0)]
    public class LevelSO : ScriptableObject
    {
        public int LevelNumber;
        public MapSO Map;
        public List<WaveSO> Waves;
    }
}