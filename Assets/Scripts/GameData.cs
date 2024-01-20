using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "GameData",menuName = "GameData")]
public class GameData : ScriptableObject
{
    [SerializeField] public InputAction moveAction;
    [SerializeField] public InputAction dashAction;
    [SerializeField] public InputAction skillAction;
}