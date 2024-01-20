using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : SingletonMonoBehaviour<InputProvider>
{
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction dashAction;
    [SerializeField] private InputAction eKeyAction;
    [HideInInspector] public Vector2 axis;
    [HideInInspector] public bool isSpaceKeyPressing;

    public void Init()
    {
        axis = Vector2.zero;

        moveAction.performed += context => axis = context.ReadValue<Vector2>();
        moveAction.canceled += context => axis = context.ReadValue<Vector2>();

        dashAction.performed += _ => isSpaceKeyPressing = true;
        dashAction.canceled += _ => isSpaceKeyPressing = false;
        
        moveAction.Enable();
        dashAction.Enable();
    }

    public IUniTaskAsyncEnumerable<AsyncUnit> EKeyOnClickAsyncEnumerable(CancellationToken gameCt)
    {
        return UniTaskAsyncEnumerable.Create<AsyncUnit>(async (writer,ct) =>
        {
            var mergedCt = CancellationTokenSource.CreateLinkedTokenSource(gameCt, ct).Token;
            while (true)
            {
                await writer.YieldAsync(new AsyncUnit());
                await UniTask.WaitUntil(() => eKeyAction.WasPerformedThisFrame(), cancellationToken: mergedCt);
                if (mergedCt.IsCancellationRequested)
                {
                    return;
                }
            }
        });
    }
}