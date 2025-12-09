using System.Collections.Generic;
using IKhom.GridSystems._Samples.helpers;
using IKhom.GridSystems._Samples.LevelGrid;
using UnityEngine;

namespace _Project.Scritps.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BallSpawner _ballSpawner;
        [SerializeField] private BoardGrid _boardGrid;

        private void Start() =>
            FillBoardWithGuaranteedDistribution();

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