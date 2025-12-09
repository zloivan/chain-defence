using UnityEngine;

namespace IKhom.GridSystems._Samples.helpers
{
    public class TileStack : MonoBehaviour
    {
        public int Age;
        public string FName;

        public override string ToString() =>
            $"{nameof(Age)}: {Age}, {nameof(FName)}: {FName}";
    }
}