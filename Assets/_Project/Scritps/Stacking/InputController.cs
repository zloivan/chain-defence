using System;
using MergeDefence.Utilities;
using UnityEngine;

namespace MergeDefence.Stacking
{
    public class InputController : MonoBehaviour
    {
        public event EventHandler<Vector3> OnDragStart;
        public event EventHandler<Vector3> OnDragEnd;
        public event EventHandler<Vector3> OnDrag;
        public event EventHandler OnInterrupted;
        
        public static InputController Instance { get; private set; }
        

        private bool _isDragging;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!_isDragging)
                    StartDrag();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_isDragging)
                    OnEndDrag();
            }

            if (_isDragging)
                HandleDrag();
        }

        public void InterruptDragging()
        {
            _isDragging = false;
            OnInterrupted?.Invoke(this, EventArgs.Empty);
        }

        private void HandleDrag() =>
            OnDrag?.Invoke(this, PointerToWorld.GetPointerPositionInWorld());

        private void StartDrag()
        {
            _isDragging = true;
            OnDragStart?.Invoke(this, PointerToWorld.GetPointerPositionInWorld());
        }

        private void OnEndDrag()
        {
            _isDragging = false;
            OnDragEnd?.Invoke(this, PointerToWorld.GetPointerPositionInWorld());
        }
    }
}