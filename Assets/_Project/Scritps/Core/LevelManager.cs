using System.Collections.Generic;
using ChainDefense.Balls;
using ChainDefense.GameGrid;
using ChainDefense.Utilities;
using UnityEngine;
using IKhom.StateMachineSystem.Runtime;

namespace ChainDefense.Core
{
    public class LevelManager : MonoBehaviour
    {
        private BallSpawner _ballSpawner;
        private BoardGrid _boardGrid;
        
        private StateMachine<GamePlayLoopState> _levelStateMachine;

        private void Awake()
        {
            Application.targetFrameRate = 30;

            _levelStateMachine = new StateMachine<GamePlayLoopState>();
            _levelStateMachine.AddState<GameplayLoop>(GamePlayLoopState.Gameplay);
            _levelStateMachine.AddStateWithContext(GamePlayLoopState.Refill, context => new RefillState(context));

            _levelStateMachine.StateChanged += (oldState, newState) =>
                Debug.Log($"State changed from {oldState} to {newState}");

            _levelStateMachine.SetInitialState(GamePlayLoopState.Refill);
        }

        private void Start()
        {
            _boardGrid = BoardGrid.Instance;
            _ballSpawner = BallSpawner.Instance;

            FillBoardWithGuaranteedDistribution();
        }

        private void Update()
        {
            _levelStateMachine.Update();
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
                _ballSpawner.SpawnBallAtGridPosition(ballIndices[i], allGridPositions[i]);
            }
        }
    }
}