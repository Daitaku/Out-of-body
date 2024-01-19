using UnityEngine;

public class CameraManager : SingletonMonoBehaviour<CameraManager>
{
    [SerializeField] private int y;
    
    public void Init()
    {
        
    }

    public void Move(Vector3 playerPosition)
    {
        transform.localPosition = new Vector3(playerPosition.x, playerPosition.y + 15f, playerPosition.z);
    }
}
