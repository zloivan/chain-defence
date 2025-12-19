using System;
using System.Collections.Generic;
using ChainDefense.Balls;
using ChainDefense.Core;
using ChainDefense.Events;
using ChainDefense.GameGrid;
using ChainDefense.GridSystem.core;
using IKhom.EventBusSystem.Runtime;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;

namespace ChainDefense.ChainManagment
{
    public class ChainValidator : MonoBehaviour
    {
        public event EventHandler<Vector3> OnHeadChangedPosition;
        public event EventHandler OnChainBreak;
        public event EventHandler<List<Ball>> OnChainDestroyed;
        private const int MIN_DESTROY_NUMBER = 3;

        private BoardGrid _boardGrid;
        private InputController _inputController;

        private readonly List<Ball> _conntectedList = new();
        private GridPosition _lastConnectedPosition;

        private void Start()
        {
            _inputController = ServiceLocator.ForSceneOf(this).Get<InputController>();
            _boardGrid = ServiceLocator.ForSceneOf(this).Get<BoardGrid>();
            
            _inputController.OnDrag += InputController_OnDrag;
            _inputController.OnDragStart += InputController_OnDragStart;
            _inputController.OnDragEnd += InputController_OnDragEnd;
        }

        private void OnDestroy()
        {
            _inputController.OnDrag -= InputController_OnDrag;
            _inputController.OnDragStart -= InputController_OnDragStart;
            _inputController.OnDragEnd -= InputController_OnDragEnd;
        }

        private void InputController_OnDragStart(object sender, Vector3 worldPos)
        {
            var gridPosition = _boardGrid.GetGridPosition(worldPos);
            if (_boardGrid.TryGetBall(gridPosition, out var first))
            {
                _conntectedList.Add(first);
                first.SetSelected();
                _lastConnectedPosition = gridPosition;
                OnHeadChangedPosition?.Invoke(this, worldPos);
                EventBus<BallDragStartedEvent>.Raise(new BallDragStartedEvent());
            }
            else
            {
                _inputController.InterruptDragging();
            }
        }

        private void InputController_OnDragEnd(object sender, EventArgs e)
        {
            if (_conntectedList.Count >= MIN_DESTROY_NUMBER)
            {
                foreach (var connectedBall in _conntectedList)
                {
                    connectedBall.DestroyBall();
                }

                OnChainDestroyed?.Invoke(this, _conntectedList);
                EventBus<ChainDestroyedEvent>.Raise(new ChainDestroyedEvent());
            }
            else
            {
                foreach (var connectedBall in _conntectedList)
                {
                    connectedBall.Deselect();
                }
                
                EventBus<InvalidChainEvent>.Raise(new InvalidChainEvent());
                
            }
            OnChainBreak?.Invoke(this, EventArgs.Empty);
            
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
                if (lastConnectedBall.GetBallConfig() != selectedBall.GetBallConfig())
                    return;


                var validNeighbors = _boardGrid.GetAllValidNeighbors(_lastConnectedPosition);

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
                    OnHeadChangedPosition?.Invoke(this, worldPos);
                    return;
                }


                selectedBall.SetSelected();
                _conntectedList.Add(selectedBall);
                _lastConnectedPosition = dragGridPosition;
                OnHeadChangedPosition?.Invoke(this, worldPos);
            }
        }

        public Vector3[] GetConnectedPositions() //TODO: Lot of garbage
        {
            var positions = new Vector3[_conntectedList.Count];
            for (var i = 0; i < positions.Length; i++)
            {
                positions[i] = _conntectedList[i].transform.position;
            }

            return positions;
        }
    }
}