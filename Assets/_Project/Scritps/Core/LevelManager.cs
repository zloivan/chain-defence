using System.Collections.Generic;
using ChainDefense.GameGrid;
using ChainDefense.Stacking;
using ChainDefense.Utilities;
using UnityEngine;

namespace ChainDefense.Core
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private BallSpawner _ballSpawner;
        [SerializeField] private BoardGrid _boardGrid;

        private void Awake() =>
            Application.targetFrameRate = 30;

        private void Start() =>
            FillBoardWithGuaranteedDistribution();

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