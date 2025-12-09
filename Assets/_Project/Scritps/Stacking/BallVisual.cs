using System;
using DG.Tweening;
using UnityEngine;

namespace MergeDefence.Stacking
{
    public class BallVisual : MonoBehaviour
    {
        private Ball _ball;
        private Vector3 _initialScale;

        private void Awake()
        {
            _ball ??= GetComponentInParent<Ball>();
            _ball.OnBallSelected += Ball_OnBallSelected;
            _ball.OnBallDeselected += Ball_OnBallDeselected;
            _initialScale = transform.localScale;
        }

        private void Ball_OnBallDeselected(object sender, EventArgs e)
        {
            transform.DOScale(_initialScale, 0.3f).SetEase(Ease.OutBack);
        }

        private void Ball_OnBallSelected(object sender, EventArgs e)
        {
            transform.DOScale(_initialScale * 1.3f, 0.3f).SetEase(Ease.OutBack);
        }
    }
}