using UnityEngine;

namespace ChainDefense.GridSystem.components
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] private TextMesh _debugLabel;
        
        private object _gridObject;

        public virtual void SetGridObject(object gridObject) =>
            _gridObject = gridObject;

        protected virtual void Update()
        {
            if (_gridObject != null)
                _debugLabel.text = _gridObject.ToString();
        }
    }
}