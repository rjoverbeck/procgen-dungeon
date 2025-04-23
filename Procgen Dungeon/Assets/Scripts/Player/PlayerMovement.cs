using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour, Controls.IGameplayActions
{
    [SerializeField] private TilemapGenerator tilemapGenerator;
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 _movement;
    private Rigidbody2D _rb;
    private Controls _controls;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rb.linearVelocity = _movement * moveSpeed;
    }

    public void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Gameplay.SetCallbacks(this);
        }
        _controls.Gameplay.Enable();
    }

    public void OnDisable()
    {
        _controls.Gameplay.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    public void OnCreateBSPTilemap(InputAction.CallbackContext context)
    {
        tilemapGenerator.CreateBSPTilemap();
    }

    public void OnCreateRWTilemap(InputAction.CallbackContext context)
    {
        tilemapGenerator.CreateRWTilemap();
    }

    public void OnCreateWFCTilemap(InputAction.CallbackContext context)
    {
        tilemapGenerator.CreateWFCTilemap();
    }
}
