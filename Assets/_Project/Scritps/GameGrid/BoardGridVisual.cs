using System;
using System.Collections.Generic;
using System.Linq;
using ChainDefense.Core;
using ChainDefense.GridSystem.core;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;

namespace ChainDefense.GameGrid
{
    public class BoardGridVisual : MonoBehaviour
    {
        enum GridVisualType
        {
            White,
            Yellow,
            Blue
        }

        [Serializable]
        struct GridVisualTypeMaterial
        {
            public GridVisualType GridVisualType;
            public Material Material;
        }

        [SerializeField] private GridVisualType _idleColor = GridVisualType.White;
        [SerializeField] private GameObject _visualPrefab;
        [SerializeField] private List<GridVisualTypeMaterial> _gridVisualTypeMaterialsList;

        private GridPosition _selectedGridPosition;
        private BoardGrid _boardGrid;
        private BoardGridVisualSingle[,] _boarGridVisuals;
        private InputController _inputController;


        private void Start()
        {
            _boardGrid = GetComponent<BoardGrid>();

            _inputController = ServiceLocator.ForSceneOf(this).Get<InputController>();
            _inputController.OnDrag += InputController_OnDrag;
            _inputController.OnDragEnd += InputController_OnDragEnd;
            _selectedGridPosition = new GridPosition(-1, -1);

            InstantiateVisualSingles();
            UpdateVisuals();
        }

        private void OnDestroy()
        {
            _inputController.OnDrag -= InputController_OnDrag;
            _inputController.OnDragEnd -= InputController_OnDragEnd;
        }

        private void InputController_OnDragEnd(object sender, EventArgs e)
        {
            if (_boardGrid.IsValidGridPosition(_selectedGridPosition))
                _boarGridVisuals[_selectedGridPosition.X, _selectedGridPosition.Z].RemoveHighlight();
        }

        private void InputController_OnDrag(object sender, Vector3 e)
        {
            var newSelected = _boardGrid.GetGridPosition(e);

            if (newSelected == _selectedGridPosition || !_boardGrid.IsValidGridPosition(newSelected))
                return;

            if (_boardGrid.IsValidGridPosition(_selectedGridPosition))
                _boarGridVisuals[_selectedGridPosition.X, _selectedGridPosition.Z].RemoveHighlight();

            _selectedGridPosition = newSelected;
            _boarGridVisuals[_selectedGridPosition.X, _selectedGridPosition.Z].Highlight();
        }

        private void UpdateVisuals()
        {
            HideAllVisuals();

            //TODO: TEMP
            ShowGridVisualList(_boardGrid.GetAllGridPositions(), _idleColor);
        }

        private void ShowGridVisualList(List<GridPosition> validGridPositions, GridVisualType visualType)
        {
            foreach (var gridPosition in validGridPositions)
            {
                _boarGridVisuals[gridPosition.X, gridPosition.Z].Show(GetMaterialForGridVisual(visualType));
            }
        }

        private Material GetMaterialForGridVisual(GridVisualType gridVisualType)
        {
            var matForGridPos = _gridVisualTypeMaterialsList.FirstOrDefault(tm => tm.GridVisualType == gridVisualType)
                .Material;

            Debug.Assert(matForGridPos, "No material found for grid visual type " + gridVisualType);
            return matForGridPos;
        }

        private void InstantiateVisualSingles()
        {
            _boarGridVisuals = new BoardGridVisualSingle[_boardGrid.GetWidth(), _boardGrid.GetHeight()];

            for (int x = 0; x < _boardGrid.GetWidth(); x++)
            {
                for (int z = 0; z < _boardGrid.GetHeight(); z++)
                {
                    var gridVisualSingle = Instantiate(_visualPrefab, transform)
                        .GetComponent<BoardGridVisualSingle>();

                    var targetGridPosition = new GridPosition(x, z);
                    gridVisualSingle.transform.position = _boardGrid.GetWorldPosition(targetGridPosition);

                    gridVisualSingle.Hide();
                    _boarGridVisuals[x, z] = gridVisualSingle;
                }
            }
        }

        private void HideAllVisuals()
        {
            foreach (var boardGridVisualSingle in _boarGridVisuals)
            {
                boardGridVisualSingle.Hide();
            }
        }
    }
}