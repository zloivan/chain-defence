using System;
using DG.Tweening;
using UnityEngine;

namespace ChainDefense.Towers
{
    public class TowerVisual : MonoBehaviour
    {
        private Tower _tower;
        private Sequence _sequence;
        private void Awake()
        {
            _tower = GetComponentInParent<Tower>();
            
            _tower.OnTowerFinishAttack += TowerFinishOnTowerFinishAttack;
            
            _sequence = DOTween.Sequence();
            _sequence.SetAutoKill(false);
            
            _sequence.Append(
                transform.DOScale(1.2f, .1f).SetEase(Ease.OutQuad)
            );
            
            _sequence.Append(
                transform.DOScale(1f, .1f).SetEase(Ease.InQuad)
            );
        }

        private void OnDestroy() =>
            _sequence.Kill();

        private void TowerFinishOnTowerFinishAttack(object sender, Tower.AttackInfoEventArts attackInfo) =>
            _sequence.Restart();
    }
}