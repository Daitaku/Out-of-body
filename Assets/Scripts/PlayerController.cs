using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

public class PlayerController : SingletonMonoBehaviour<PlayerController>
{
    [SerializeField] private int dashSpeed;
    [SerializeField] private int speed;
    private bool _canDash;
    private bool _wasSpaceKeyPressingLastFrame;
    
    public void Init()
    {
        _canDash = true;
    }

    public async UniTask Behaviour(CancellationToken gameCt)
    {
        var mergedCt =
            CancellationTokenSource.CreateLinkedTokenSource(gameCt, gameObject.GetCancellationTokenOnDestroy()).Token;
        await UniTaskAsyncEnumerable.EveryUpdate().ForEachAwaitAsync(async _ =>
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
                    _canDash = false;
                    BanDashAsync(mergedCt).Forget();
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
            
        },mergedCt);
        
    }

    private async UniTask BanDashAsync(CancellationToken gameCt)
    {
        _wasSpaceKeyPressingLastFrame = false;
        await UniTask.Delay(TimeSpan.FromSeconds(1),cancellationToken:gameCt);
        _canDash = true;
    }
}
