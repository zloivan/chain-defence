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
            FillTheBoard();

        private void FillTheBoard()
        {
            var allGridPosition =  _boardGrid.GetAllGridPositions();

            foreach (var gridPosition in allGridPosition)
            {
                if (_boardGrid.IsSlotEmpty(gridPosition))
                {
                    _ballSpawner.SpawnRandomBallAtGridPosition(gridPosition);
                }
            }
        }
    }
}