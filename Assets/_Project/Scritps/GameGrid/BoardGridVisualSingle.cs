using UnityEngine;

namespace ChainDefense.GameGrid
{
    public class BoardGridVisualSingle : MonoBehaviour
    {
        private static readonly int EmissionColorPropertyId = Shader.PropertyToID("_EmissionColor");
        private static readonly int BaseColorPropertyId = Shader.PropertyToID("_BaseColor");

        [SerializeField] private float _emissionIntensity = 2f;

        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _mpb;
        private Color _originalColor;
        private bool _isHighlighted;
        private Material _sharedMaterial; // Cached reference to avoid GetComponent calls

        private void Awake()
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _mpb = new MaterialPropertyBlock();
        }

        public void Show(Material matForVisual)
        {
            if (_sharedMaterial != matForVisual)
            {
                _sharedMaterial = matForVisual;
                _meshRenderer.sharedMaterial = matForVisual; // Use shared, not instanced
                _originalColor = matForVisual.GetColor(BaseColorPropertyId);

                // Enable emission keyword on SHARED material (one-time cost for all cells)
                if (!matForVisual.IsKeywordEnabled("_EMISSION"))
                {
                    matForVisual.EnableKeyword("_EMISSION");
                }
            }

            _meshRenderer.enabled = true;
            _isHighlighted = false;

            // Reset MPB to original state
            _mpb.Clear();
            _meshRenderer.SetPropertyBlock(_mpb);
        }

        public void Hide()
        {
            _meshRenderer.enabled = false;
            _isHighlighted = false;
        }

        public void Highlight()
        {
            if (!_meshRenderer.enabled || _isHighlighted) return;

            // Override emission via MPB (no material instance created)
            _mpb.SetColor(EmissionColorPropertyId, _originalColor * _emissionIntensity);
            _meshRenderer.SetPropertyBlock(_mpb);

            _isHighlighted = true;
        }

        public void RemoveHighlight()
        {
            if (!_meshRenderer.enabled || !_isHighlighted) return;

            // Clear MPB overrides (uses shared material's default values)
            _mpb.Clear();
            _meshRenderer.SetPropertyBlock(_mpb);

            _isHighlighted = false;
        }
    }
}