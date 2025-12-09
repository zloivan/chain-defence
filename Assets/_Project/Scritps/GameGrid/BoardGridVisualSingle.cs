using UnityEngine;

namespace IKhom.GridSystems._Samples.LevelGrid
{
    public class BoardGridVisualSingle : MonoBehaviour
    {
        private const string EMISSION = "_EMISSION";
        private static readonly int ColorPropertyId = Shader.PropertyToID("_EmissionColor");

        [SerializeField] private float _emissionIntensity = 2f;

        private MeshRenderer _meshRenderer;
        private Material _instanceMaterial;
        private Color _originalColor;
        private bool _isHighlighted;

        private void Awake() 
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        public void Show(Material matForVisual)
        {
            _meshRenderer.enabled = true;
            _instanceMaterial = new Material(matForVisual);
            _meshRenderer.material = _instanceMaterial;

            _originalColor = _instanceMaterial.color;


            _instanceMaterial.DisableKeyword(EMISSION);
            _isHighlighted = false;
        }

        public void Hide()
        {
            _meshRenderer.enabled = false;
            _isHighlighted = false;

            if (!_instanceMaterial)
                return;

            Destroy(_instanceMaterial);
            _instanceMaterial = null;
        }

        public void Highlight()
        {
            if (!_meshRenderer.enabled || _isHighlighted || !_instanceMaterial)
                return;

            _instanceMaterial.EnableKeyword(EMISSION);
            _instanceMaterial.SetColor(ColorPropertyId, _originalColor * _emissionIntensity);

            _isHighlighted = true;
        }

        public void RemoveHighlight()
        {
            if (!_meshRenderer.enabled || !_isHighlighted || !_instanceMaterial)
                return;

            _instanceMaterial.DisableKeyword(EMISSION);
            _instanceMaterial.SetColor(ColorPropertyId, Color.black);

            _isHighlighted = false;
        }
    }
}