using System;
using ChainDefense.Enemies;
using DG.Tweening;
using UnityEngine;

namespace ChainDefense.Towers
{
    public class TowerVisual : MonoBehaviour
    {
        [SerializeField] private Transform _rotatableTransform;
        [SerializeField] private float _rotationSpeed = 5f; 
        private Tower _tower;
        private Sequence _sequence;
        private Enemy _currentTarget;
        private Quaternion _originalRotation;
        private void Awake()
        {
            _tower = GetComponentInParent<Tower>();
            
            _tower.OnTowerBeginAttack += TowerFinishOnTowerFinishAttack;
            _tower.OnTargetChanged += Tower_OnTargetChanged;
            _currentTarget = _tower.GetCurrentTarget();
            _sequence = DOTween.Sequence();
            _sequence.SetAutoKill(false);
            
            _sequence.Append(
                transform.DOScale(1.2f, .1f).SetEase(Ease.OutQuad)
            );
            
            _sequence.Append(
                transform.DOScale(1f, .1f).SetEase(Ease.InQuad)
            );
            
            _originalRotation = transform.rotation;
        }

        private void Update()
        {
            HandleRotation();
        }

        private void Tower_OnTargetChanged(object sender, Enemy enemy) =>
            _currentTarget = enemy;

        private void OnDestroy() =>
            _sequence.Kill();

        private void TowerFinishOnTowerFinishAttack(object sender, AttackInfoEventArts attackInfo) =>
            _sequence.Restart();
        
        private void HandleRotation()
        {
            if (_currentTarget == null)
            {
                _rotatableTransform.rotation =
                    Quaternion.Lerp(_rotatableTransform.rotation, _originalRotation, Time.deltaTime * _rotationSpeed);
                return;
            }

            var rotationDirection = (_currentTarget.transform.position - transform.position).normalized;
            
            _rotatableTransform.rotation =
                Quaternion.Lerp(_rotatableTransform.rotation, Quaternion.LookRotation(rotationDirection), Time.deltaTime * _rotationSpeed);            
        }
    }
}