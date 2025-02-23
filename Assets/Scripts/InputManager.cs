using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;
    private PlayerInput playerInput;
    private InputAction playerMove;
    private InputAction playerInteract;
    private InputAction playerHold;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMove = playerInput.actions["Move"];
        playerInteract = playerInput.actions["Interact"];
        playerHold = playerInput.actions["Hold"];
    }

    private void Update()
    {
        Movement = playerMove.ReadValue<Vector2>();
    }
}
