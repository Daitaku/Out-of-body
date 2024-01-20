using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

public class PlayerController : SingletonMonoBehaviour<PlayerController>
{
    [SerializeField] private PlayerActionManager playerActionManager;
    [SerializeField] private int dashSpeed;
    [SerializeField] private int speed;
    private bool _canDash;
    private bool _wasSpaceKeyPressingLastFrame;
    
    public void Init()
    {
        _canDash = true;
    }

    public async UniTask Behaviour(CancellationToken ct)
    {
        var mergedCt =
            CancellationTokenSource.CreateLinkedTokenSource(ct, gameObject.GetCancellationTokenOnDestroy()).Token;

        await MoveControlAsync(mergedCt);
    }

    private async UniTask InvokeAstralProjectionAsync(CancellationToken ct)
    {
        InputProvider.Instance.EKeyOnClickAsyncEnumerable(ct).ForEachAsync(_ =>
        {
            //実装もろもろ
        }, cancellationToken: ct);
    }

    private async UniTask MoveControlAsync(CancellationToken ct)
    {
        await UniTaskAsyncEnumerable.EveryUpdate().ForEachAsync(_ =>
        {
            var axis = InputProvider.Instance.axis;
            if (axis.magnitude == 0) return;
            var vector = axis / axis.magnitude;
            
            Vector2 moveVector;
            if (InputProvider.Instance.isSpaceKeyPressing && _canDash)
            {
                moveVector = vector * dashSpeed / 100f;
                _wasSpaceKeyPressingLastFrame = true;
            }
            else
            {
                if (_wasSpaceKeyPressingLastFrame)
                {
                    BanDashAsync(ct).Forget();
                }
                
                moveVector = vector * speed / 100f;
            }

            var go = gameObject;
            var moveVector3d = new Vector3(moveVector.x, 0, moveVector.y);
            var currentPosition = go.transform.localPosition;

            if (Physics.Raycast(currentPosition, moveVector3d, out var _, moveVector.magnitude))
            {
                return;
            }
            var position = currentPosition;
            position += moveVector3d;
            go.transform.localPosition = position;
            
        },ct);
    }

    private async UniTask BanDashAsync(CancellationToken ct)
    {
        _canDash = false;
        _wasSpaceKeyPressingLastFrame = false;
        await UniTask.Delay(TimeSpan.FromSeconds(1),cancellationToken:ct);
        _canDash = true;
    }
}
