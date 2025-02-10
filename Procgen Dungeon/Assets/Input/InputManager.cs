using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;
    // TODO: Create Enum for algorithms expose publicly for use in Tilemap Generator

    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _algorithmSelect;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions["Move"];
        _algorithmSelect = _playerInput.actions["AlgorithmSelect"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

    }
}
