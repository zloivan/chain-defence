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
        [SerializeField] private Gradient _lineGradient;

        private bool _isDrawing;
        private Vector3 _pointerEndPosition;
        private const int BASE_CAPACITY = 32;
        private Vector3[] _positionsBuffer = new Vector3[BASE_CAPACITY];
        private int _currentPositionCount;

        private void Awake() =>
            ConfigureLineRenderer();

        private void ConfigureLineRenderer()
        {
            _lineRenderer.positionCount = 0;
            _lineRenderer.colorGradient = _lineGradient;
            _lineRenderer.numCornerVertices = 5;
            _lineRenderer.numCapVertices = 5;
        }

        private void Start()
        {
            _chainValidator.OnHeadChangedPosition += ChainValidator_OnHeadChangedPosition;
            _chainValidator.OnChainBreak += ChainValidator_OnChainBreak;
        }

        private void OnDestroy()
        {
            _chainValidator.OnHeadChangedPosition -= ChainValidator_OnHeadChangedPosition;
            _chainValidator.OnChainBreak -= ChainValidator_OnChainBreak;
        }

        private void Update()
        {
            if (!_isDrawing || _currentPositionCount == 0) return;

            var pointerPosition = PointerToWorld.GetPointerPositionInWorld();
            const float HEIGHT_OFFSET = 1f;
            pointerPosition.y = HEIGHT_OFFSET;
            var lastHeadPosition = _positionsBuffer[_currentPositionCount - 1];
            var directionToPointer = pointerPosition - lastHeadPosition;
            var distanceToPointer = directionToPointer.magnitude;

            if (distanceToPointer <= _maxLineDistance)
            {
                _pointerEndPosition = pointerPosition;
            }
            else
            {
                _pointerEndPosition = lastHeadPosition + directionToPointer.normalized * _maxLineDistance;
            }

            UpdateLineRenderer();
        }

        private void ChainValidator_OnChainBreak(object sender, EventArgs e)
        {
            _lineRenderer.positionCount = 0;
            _isDrawing = false;
            _currentPositionCount = 0;
        }

        private void ChainValidator_OnHeadChangedPosition(object sender, Vector3 newHeadPosition)
        {
            var connectedPositions = _chainValidator.GetConnectedPositions();
            _currentPositionCount = connectedPositions.Length;

            if (_currentPositionCount > _positionsBuffer.Length)
            {
                _positionsBuffer = new Vector3[_currentPositionCount + 16];
            }

            Array.Copy(connectedPositions, _positionsBuffer, _currentPositionCount);
            _isDrawing = true;
        }

        private void UpdateLineRenderer()
        {
            _lineRenderer.positionCount = _currentPositionCount + 1;
            _lineRenderer.SetPositions(_positionsBuffer);
            _lineRenderer.SetPosition(_currentPositionCount, _pointerEndPosition);
        }
    }
}