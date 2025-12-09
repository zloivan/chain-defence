using System;
using UnityEngine;

namespace ChainDefense.ChainManagment
{
    public class ChainVisualizer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _maxLineDistance = 0.5f;
        
        private ChainValidator _chainValidator;
        
        
        
        private void Start()
        {
            _chainValidator = GetComponent<ChainValidator>();
            _chainValidator.OnHeadChangedPosition += ChainValidator_OnHeadChangedPosition;
        }

        private void ChainValidator_OnHeadChangedPosition(object sender, Vector3 newHeadPosition)
        {
            //throw new NotImplementedException();
        }

        
    }
}