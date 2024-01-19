using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class GameManager
{
    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        InputProvider.Instance.Init();
        PlayerController.Instance.Init();
        Progress().Forget();
    }

    private static async UniTask Progress()
    {
        var gameCts = new CancellationTokenSource();
        await PlayerController.Instance.Behaviour(gameCts.Token);
    }
}
