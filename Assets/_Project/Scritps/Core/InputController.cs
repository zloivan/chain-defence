using System;
using ChainDefense.Utilities;
using UnityEngine;

namespace ChainDefense.Core
{
    public class InputController : MonoBehaviour
    {
        public event EventHandler<Vector3> OnDragStart;
        public event EventHandler<Vector3> OnDrag;
        public event EventHandler OnDragEnd;

        public static InputController Instance { get; private set; }
        

        private bool _isDragging;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                var touch = UnityEngine.Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (!_isDragging) StartDrag();
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (_isDragging) HandleDrag();
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (_isDragging) OnEndDrag();
                        break;
                }
                return;
            }
            
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (!_isDragging)
                    StartDrag();
            }

            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                if (_isDragging)
                    OnEndDrag();
            }

            if (_isDragging)
                HandleDrag();
        }

        public void InterruptDragging()
        {
            OnEndDrag();
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
            OnDragEnd?.Invoke(this, EventArgs.Empty);
        }
    }
}