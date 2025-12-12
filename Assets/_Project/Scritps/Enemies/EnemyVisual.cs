using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ChainDefense.Enemies
{
    public class EnemyVisual : MonoBehaviour
    {
        private static readonly int ColorPropertyId = Shader.PropertyToID("_BaseColor");
        [SerializeField] private Color _slowColor;
        [SerializeField] private Color _damageColor = Color.red;
        [SerializeField] private float _damageFlashDuration = 0.1f;

        private MaterialPropertyBlock _mpb;
        private Enemy _enemy;
        private MeshRenderer _meshRenderer;
        private bool _isFlashingDamage;
        private bool _isSlowed;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _mpb = new MaterialPropertyBlock();
        }

        private void Start()
        {
            _enemy = GetComponentInParent<Enemy>();
            _enemy.OnEnemySlowedStart += Enemy_OnEnemySlowed;
            _enemy.OnEnemySlowedFinish += Enemy_OnEnemySlowedFinish;
            _enemy.OnEnemyTakeDamage += Enemy_OnEnemyTakeDamage;
        }

        private void Enemy_OnEnemyTakeDamage(object sender, int damageValue)
        {
            EnemyTakeDamage().Forget();
        }

        private async UniTask EnemyTakeDamage()
        {
            _isFlashingDamage = true;
            UpdateVisual();

            await UniTask.Delay(TimeSpan.FromSeconds(_damageFlashDuration),
                cancellationToken: this.GetCancellationTokenOnDestroy());
            _isFlashingDamage = false;
            UpdateVisual();
        }

        private void Enemy_OnEnemySlowed(object sender, EventArgs e)
        {
            _isSlowed = true;
            UpdateVisual();
        }

        private void Enemy_OnEnemySlowedFinish(object sender, EventArgs e)
        {
            _isSlowed = false;
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (_isFlashingDamage)
            {
                _mpb.SetColor(ColorPropertyId, _damageColor);
                _meshRenderer.SetPropertyBlock(_mpb);
            }
            else if (_isSlowed)
            {
                _mpb.SetColor(ColorPropertyId, _slowColor);
                _meshRenderer.SetPropertyBlock(_mpb);
            }
            else
            {
                _meshRenderer.SetPropertyBlock(null);
            }
        }
    }
}