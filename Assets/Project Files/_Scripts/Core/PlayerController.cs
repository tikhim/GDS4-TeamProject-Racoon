using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
public class PlayerController: EntityBase
{
    [Header("References")]
    private PlayerInput _input;
    private Movement _move;

    [Header("Gameplay")]
    [HideInInspector] public Vector3 moveInput;
 
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _input = new PlayerInput();
        _move = GetComponent<Movement>();
    }

    private void FixedUpdate()
    {
        _move.MovementUpdate(GetMoveInput());
    }

    private Vector3 GetMoveInput() {
        // Normalise Input and Camera
        moveInput = moveInput.normalized;

        Vector3 camForward = Camera.main.transform.forward.normalized;
        Vector3 camRight = Camera.main.transform.right.normalized;

        // Nullify y vector to disable camera influence to movement in the verticle axis
        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRelativeInput = moveInput.y * camForward;
        Vector3 rightRelativeInput = moveInput.x * camRight;

        return (rightRelativeInput + forwardRelativeInput).normalized;
    }

    #region Input

    private void OnEnable() {
        _input.Enable();

        //Subscribe Player Input to Events
        _input.Player.Movement.performed += OnMovement;
        _input.Player.Movement.canceled += OnMovement;

        _input.Player.Jump.started += OnJumpStarted;
        _input.Player.Jump.canceled += OnJumpCancelled;
    }
    private void OnDisable() {
        _input.Disable();

        //Unsubscribe Player Input to Events
        _input.Player.Movement.performed -= OnMovement;
        _input.Player.Movement.canceled -= OnMovement;

        _input.Player.Jump.started -= OnJumpStarted;
        _input.Player.Jump.canceled -= OnJumpCancelled;
    }

    private void OnMovement(InputAction.CallbackContext value) {
        moveInput = value.ReadValue<Vector2>();
    }

    private void OnJumpStarted(InputAction.CallbackContext value) {
        _move.InitialiseJump(value.ReadValueAsButton(), true);
    }

    private void OnJumpCancelled(InputAction.CallbackContext value) {
        _move.GetComponent<Movement>().InitialiseJump(value.ReadValueAsButton(), false);
    }

    #endregion

}