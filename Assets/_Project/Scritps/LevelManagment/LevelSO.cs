using System.Collections.Generic;
using DefaultNamespace;
using IKhom.StateMachineSystem.Runtime;
using UnityEngine;

namespace ChainDefense.Core
{
    [CreateAssetMenu(fileName = "Level", menuName = "Configs/Leve SO", order = 0)]
    public class LevelSO : ScriptableObject
    {
        public int LevelNumber;
        public MapSO Map;
        public List<WaveSO> Waves;
    }
}