using System;
using UnityEngine;

namespace ChainDefense.Enemies
{
    public class EnemyVisual : MonoBehaviour
    {
        private static readonly int ColorPropertyId = Shader.PropertyToID("_BaseColor");
        [SerializeField] private Color _slowColor;

        private MaterialPropertyBlock _mpb;
        private Enemy _enemy;
        private MeshRenderer _meshRenderer;

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
        }

        private void Enemy_OnEnemySlowed(object sender, EventArgs e)
        {
            _mpb.SetColor(ColorPropertyId, _slowColor);
            _meshRenderer.SetPropertyBlock(_mpb);
        }

        private void Enemy_OnEnemySlowedFinish(object sender, EventArgs e) =>
            _meshRenderer.SetPropertyBlock(null);
    }
}