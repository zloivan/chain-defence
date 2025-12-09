using System;
using System.Collections.Generic;
using MergeDefence.GameGrid;
using MergeDefence.GridSystem.core;
using MergeDefence.Stacking;
using MergeDefence.Utilities;
using UnityEngine;

namespace MergeDefence.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BallSpawner _ballSpawner;
        [SerializeField] private BoardGrid _boardGrid;
        [SerializeField] private InputController _inputController;

        private const int MIN_DESTROY_NUMBER = 2;
        private LinkedList<Ball> _conntectedList = new();

        private void Awake() =>
            Application.targetFrameRate = 30;

        private void Start()
        {
            FillBoardWithGuaranteedDistribution();
            _inputController.OnDrag += InputController_OnDrag;
            _inputController.OnDragStart += InputController_OnDragStart;
            _inputController.OnDragEnd += InputController_OnDragEnd;
        }

        private void InputController_OnDragStart(object sender, Vector3 worldPos)
        {
            var gridPosition = _boardGrid.GetGridPosition(worldPos);
            if (_boardGrid.TryGetBall(gridPosition, out var first))
            {
                _conntectedList.AddFirst(first);
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

        private GridPosition _lastConnectedPosition;

        private void InputController_OnDrag(object sender, Vector3 worldPos)
        {
            //Connect only from last one neighbors
            //Remove connection, if going back
            var dragGridPosition = _boardGrid.GetGridPosition(worldPos);

            if (_boardGrid.TryGetBall(dragGridPosition, out var selectedBall))
            {
                //Same ball -> ignore
                if (_conntectedList.Last.Value == selectedBall)
                    return;

                //Different type -> ignore
                if (_conntectedList.Last.Value.GetBallColorSO() != selectedBall.GetBallColorSO())
                    return;


                var validNeighbors = _boardGrid.GetValidNeighbors(_lastConnectedPosition);
                
                //Not neighbor and not in connections -> ignore
                if (!validNeighbors.Contains(dragGridPosition) && !_conntectedList.Contains(selectedBall))
                    return;

                //If not neighbor but in connections already -> remove till that one
                if (!validNeighbors.Contains(dragGridPosition) && _conntectedList.Contains(selectedBall))
                {
                    while (_conntectedList.Last.Value != selectedBall)
                    {
                        var toRemove = _conntectedList.Last.Value;
                        toRemove.Deselect();
                        _conntectedList.RemoveLast();
                    }

                    _lastConnectedPosition = dragGridPosition;
                    return;
                }


                selectedBall.SetSelected();
                _conntectedList.AddLast(selectedBall);
                _lastConnectedPosition = dragGridPosition;
            }
        }

        private void FillBoardWithRandomBalls()
        {
            var allGridPosition = _boardGrid.GetAllGridPositions();

            foreach (var gridPosition in allGridPosition)
            {
                if (_boardGrid.IsSlotEmpty(gridPosition))
                {
                    _ballSpawner.SpawnRandomBallAtGridPosition(gridPosition);
                }
            }
        }

        private void FillBoardWithGuaranteedDistribution()
        {
            var allGridPositions = _boardGrid.GetAllGridPositions();
            var totalSlots = allGridPositions.Count;

            var ballIndices = new List<int>();

            var numberOfBallTypes = 4;
            var ballsPerType = totalSlots / numberOfBallTypes;
            var remainder = totalSlots % numberOfBallTypes;

            for (var ballType = 0; ballType < numberOfBallTypes; ballType++)
            {
                var count = ballsPerType;

                if (ballType < remainder)
                    count++;

                for (var i = 0; i < count; i++)
                {
                    ballIndices.Add(ballType);
                }
            }

            ballIndices.ShuffleList();

            for (var i = 0; i < allGridPositions.Count; i++)
            {
                if (_boardGrid.IsSlotEmpty(allGridPositions[i]))
                {
                    _ballSpawner.SpawnBallAtGridPosition(ballIndices[i], allGridPositions[i]);
                }
            }
        }
    }
}