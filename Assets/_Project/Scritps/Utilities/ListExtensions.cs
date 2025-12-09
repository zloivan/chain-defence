using System.Collections.Generic;
using UnityEngine;

namespace IKhom.GridSystems._Samples.helpers
{
    public static class ListExtensions
    {
        public static void ShuffleList<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
    }
}