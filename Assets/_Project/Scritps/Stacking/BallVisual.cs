using System;
using UnityEngine;

namespace IKhom.GridSystems._Samples.helpers
{
 
    public class BallVisual : MonoBehaviour
    {
        [SerializeField] private Ball _ball;

        private void Awake() =>
            _ball ??= GetComponentInParent<Ball>();
    }
}