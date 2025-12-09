using System.Collections.Generic;
using ChainDefense.Balls;
using ChainDefense.Core;
using ChainDefense.GameGrid;
using ChainDefense.GridSystem.core;
using UnityEngine;

namespace ChainDefense.ChainManagment
{
    public class ChainValidator : MonoBehaviour
    {
        private const int MIN_DESTROY_NUMBER = 2;
        
        [SerializeField] private BoardGrid _boardGrid;
        [SerializeField] private InputController _inputController;

        private List<Ball> _conntectedList = new();
        private GridPosition _lastConnectedPosition;

        private void Start()
        {
            _inputController.OnDrag += InputController_OnDrag;
            _inputController.OnDragStart += InputController_OnDragStart;
            _inputController.OnDragEnd += InputController_OnDragEnd;
        }

        private void InputController_OnDragStart(object sender, Vector3 worldPos)
        {
            var gridPosition = _boardGrid.GetGridPosition(worldPos);
            if (_boardGrid.TryGetBall(gridPosition, out var first))
            {
                _conntectedList.Add(first);
                first.SetSelected();
                _lastConnectedPosition = gridPosition;
            }
            else
            {
                _inputController.InterruptDragging();
            }
        }

        private void InputController_OnDragEnd(object sender, Vector3 worldPos)
        {
            foreach (var connectedBall in _conntectedList)
            {
                if (_conntectedList.Count > MIN_DESTROY_NUMBER)
                    connectedBall.DestroyBall();
                else
                    connectedBall.Deselect();
            }

            _conntectedList.Clear();
        }

        private void InputController_OnDrag(object sender, Vector3 worldPos)
        {
            //Connect only from last one neighbors
            //Remove connection, if going back
            var dragGridPosition = _boardGrid.GetGridPosition(worldPos);

            if (_boardGrid.TryGetBall(dragGridPosition, out var selectedBall))
            {
                var lastConnectedBall = _conntectedList[^1];
                //Same ball -> ignore
                if (lastConnectedBall == selectedBall)
                    return;

                //Different type -> ignore
                if (lastConnectedBall.GetBallColorSO() != selectedBall.GetBallColorSO())
                    return;


                var validNeighbors = _boardGrid.GetValidNeighbors(_lastConnectedPosition);
                
                //Not neighbor and not in connections -> ignore
                if (!validNeighbors.Contains(dragGridPosition) && !_conntectedList.Contains(selectedBall))
                    return;

                //If in connections already -> remove till that one
                if (_conntectedList.Contains(selectedBall))
                {
                    while (_conntectedList[^1] != selectedBall)
                    {
                        var toRemove = _conntectedList[^1];
                        toRemove.Deselect();
                        _conntectedList.Remove(toRemove);
                    }

                    _lastConnectedPosition = dragGridPosition;
                    return;
                }


                selectedBall.SetSelected();
                _conntectedList.Add(selectedBall);
                _lastConnectedPosition = dragGridPosition;
            }
        }
    }
}