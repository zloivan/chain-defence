using System;
using ChainDefense.Utilities;
using IKhom.UtilitiesLibrary.Runtime.components;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ChainDefense.Core
{
    public class InputController : SingletonBehaviour<InputController>
    {
        public event EventHandler<Vector3> OnDragStart;
        public event EventHandler<Vector3> OnDrag;
        public event EventHandler OnDragEnd;

        private bool _isDragging;

        private EventSystem _eventSystem;

        protected override void Awake()
        {
            base.Awake();

            _eventSystem = EventSystem.current;
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (!_isDragging && !IsPointerOverUI(touch.fingerId))
                            StartDrag();
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

            if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
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
            OnEndDrag();
        }

        private void HandleDrag()
        {
            OnDrag?.Invoke(this, PointerToWorld.GetPointerPositionInWorld());
        }

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

        private bool IsPointerOverUI(int touchId = -1)
        {
            if (_eventSystem == null)
                return false;

            return touchId >= 0
                ? _eventSystem.IsPointerOverGameObject(touchId)
                : _eventSystem.IsPointerOverGameObject();
        }
    }
}