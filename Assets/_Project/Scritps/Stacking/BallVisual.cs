using UnityEngine;

namespace MergeDefence.Stacking
{
 
    public class BallVisual : MonoBehaviour
    {
        [SerializeField] private Ball _ball;

        private void Awake() =>
            _ball ??= GetComponentInParent<Ball>();
    }
}