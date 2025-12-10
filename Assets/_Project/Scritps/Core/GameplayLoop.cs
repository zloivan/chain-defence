using System.Threading;
using Cysharp.Threading.Tasks;
using IKhom.StateMachineSystem.Runtime.abstractions;
using UnityEngine;

namespace ChainDefense.Core
{
   public enum GamePlayLoopState
    {
        Gameplay,
        Refill
    }
    
    public class GameplayLoop : IState<GamePlayLoopState>
    {
       

        public UniTask EnterAsync(CancellationToken cancellationToken = new())
        {
            Debug.Log("Entered Gameplay State");
            return UniTask.CompletedTask;
        }

        public UniTask UpdateAsync(CancellationToken cancellationToken = new()) =>
            UniTask.CompletedTask;

        public UniTask ExitAsync(CancellationToken cancellationToken = new())
        {
            Debug.Log("Exited Gameplay State");
            return UniTask.CompletedTask;
        }
    }
    
    public class RefillState : IState<GamePlayLoopState>
    {
        private readonly IStateContext<GamePlayLoopState> _context;

        public RefillState(IStateContext<GamePlayLoopState> context) =>
            _context = context;

        public async UniTask EnterAsync(CancellationToken cancellationToken = new())
        {
            Debug.Log("Entered Gameplay State");
            
            await UniTask.Delay(2000, cancellationToken: cancellationToken);
            _context.ChangeState(GamePlayLoopState.Gameplay);
        }

        public UniTask UpdateAsync(CancellationToken cancellationToken = new()) =>
            UniTask.CompletedTask;

        public UniTask ExitAsync(CancellationToken cancellationToken = new())
        {
            Debug.Log("Exited Gameplay State");
            return UniTask.CompletedTask;
        }
    }
}