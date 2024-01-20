using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class InputProvider
{
    public static Vector2 Axis;
    public static bool IsSpaceKeyPressing;
    private static GameData _data;

    public static async UniTask Init(CancellationToken ct = default)
    {
        _data = await Addressables.LoadAssetAsync<GameData>("Assets/Settings/GameData.asset").WithCancellation(ct);
        
        Axis = Vector2.zero;
        
        _data.moveAction.performed += context => Axis = context.ReadValue<Vector2>();
        _data.moveAction.canceled += context => Axis = context.ReadValue<Vector2>();

        _data.dashAction.performed += _ => IsSpaceKeyPressing = true;
        _data.dashAction.canceled += _ => IsSpaceKeyPressing = false;
        
        _data.moveAction.Enable();
        _data.dashAction.Enable();
    }

    public static IUniTaskAsyncEnumerable<AsyncUnit> SkillKeyOnClickAsyncEnumerable(CancellationToken gameCt)
    {
        return UniTaskAsyncEnumerable.Create<AsyncUnit>(async (writer,ct) =>
        {
            var mergedCt = CancellationTokenSource.CreateLinkedTokenSource(gameCt, ct).Token;
            while (true)
            {
                await writer.YieldAsync(new AsyncUnit());
                await UniTask.WaitUntil(() => _data.skillAction.WasPerformedThisFrame(), cancellationToken: mergedCt);
                if (mergedCt.IsCancellationRequested)
                {
                    return;
                }
            }
        });
    }
}