using System;
using ChainDefense.Utilities;
using UnityEngine;

namespace ChainDefense.ChainManagment
{
    public class ChainVisualizer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _maxLineDistance = 0.5f;
        [SerializeField] private ChainValidator _chainValidator;


        private bool _isDrawing;
        private Vector3[] _connectedPositions;

        private void Start()
        {
            _chainValidator.OnHeadChangedPosition += ChainValidator_OnHeadChangedPosition;
            _chainValidator.OnChainBreak += ChainValidator_OnChainBreak;

            _lineRenderer.positionCount = 0;
        }

        private void Update()
        {
            if (_isDrawing && _connectedPositions.Length > 0)
            {
                Vector3 pointerPosition = PointerToWorld.GetPointerPositionInWorld();
                pointerPosition.y = .75f;
                Vector3 lastHeadPosition = _connectedPositions[^1];
                Vector3 directionToPointer = pointerPosition - lastHeadPosition;
                float distanceToPointer = directionToPointer.magnitude;
                Vector3 endPosition;

                if (distanceToPointer <= _maxLineDistance)
                {
                    // Pointer is within range, draw to pointer
                    endPosition = pointerPosition;
                }
                else
                {
                    // Pointer is too far, clamp to max distance
                    endPosition = lastHeadPosition + directionToPointer.normalized * _maxLineDistance;
                }

                _lineRenderer.positionCount = _connectedPositions.Length + 1;
                _lineRenderer.SetPositions(_connectedPositions);
                _lineRenderer.SetPosition(_connectedPositions.Length, endPosition);
            }
        }

        private void ChainValidator_OnChainBreak(object sender, EventArgs e)
        {
            _lineRenderer.positionCount = 0;
            _isDrawing = false;
        }

        private void ChainValidator_OnHeadChangedPosition(object sender, Vector3 newHeadPosition)
        {
            _connectedPositions = _chainValidator.GetConnectedPositions();
            _isDrawing = true;
        }
    }
}