using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("References")]
    private Rigidbody _rb;
    private EntityBase _base;
    private EntityAttributes _stats;
    private float _time;

    [Header("Stats")]
    public float currentSpeed = 6f;

    [Header("Collisions")]
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private Transform groundCheck;
    public bool _grounded = true;
    private float _frameLeftGrounded = float.MinValue;
    private RaycastHit slopeHit;
    
    [Header("Movement")]
    private Vector3 moveVelocity = Vector3.zero;

    [Header("Jump")]
    private bool jumpInput;
    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;
    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void Update() {
        _time += Time.deltaTime;
    }

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _base = GetComponent<EntityBase>();
        _stats = _base._stats;
    }


    private void FixedUpdate() {
        CheckCollisions();
        HandleGravity();
    }

    public void MovementUpdate(Vector3 moveInput){
        HandleJump();
        HandleDirection(moveInput);

        HandleRotation();
        ApplyMovement();
    }

    private void HandleRotation()
    {
        //Rotation based on velocity
        Vector3 rot = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        if (rot.sqrMagnitude > 1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rot), _stats.rotSpeed * Time.deltaTime);
        }
    }


    private void CheckCollisions()
    {
        //Raycasts
        bool groundHit = Physics.CheckSphere(groundCheck.position, 0.3f, GroundLayer);
        
        // Landed on the ground
        if (!_grounded && groundHit) {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
        } else if (_grounded && !groundHit){
            _grounded = false;
            _frameLeftGrounded = _time;
        }
    }

    private bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 0.3f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < _stats.MaxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 moveInput) {
        return Vector3.ProjectOnPlane(moveInput, slopeHit.normal).normalized;
    }


    private void HandleDirection(Vector3 moveInput)
    {
        Vector3 moveDir = !OnSlope() ? moveInput : GetSlopeMoveDirection(moveInput); 

        float deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
        float acceleration = _grounded ? _stats.GroundAcceleration : _stats.AirAcceleration;

        if (OnSlope()) {
            moveVelocity.y = Mathf.MoveTowards(moveVelocity.y, moveDir.y * currentSpeed, acceleration * Time.fixedDeltaTime);
        }

        // Apply Acceleration and Decceleration
        if (moveInput == Vector3.zero || moveVelocity.magnitude > 15) {
            moveVelocity.x = Mathf.MoveTowards(moveVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            moveVelocity.z = Mathf.MoveTowards(moveVelocity.z, 0, deceleration * Time.fixedDeltaTime);
        } else {
            moveVelocity.x = Mathf.MoveTowards(moveVelocity.x, moveDir.x * currentSpeed, acceleration * Time.fixedDeltaTime);
            moveVelocity.z = Mathf.MoveTowards(moveVelocity.z, moveDir.z * currentSpeed, acceleration * Time.fixedDeltaTime);
        }
    }


    public void InitialiseJump(bool value, bool started) {
        jumpInput = value;

        if (!started) return;
        _jumpToConsume = true;
        _timeJumpWasPressed = _time;
    }

    private void HandleJump() {
        if (!_stats.canJump) return;
        if (!_endedJumpEarly && !_grounded && !jumpInput && _rb.velocity.y > 0) _endedJumpEarly = true;
        if (!_jumpToConsume && !HasBufferedJump) return;
        if (_grounded || CanUseCoyote) ExecuteJump();
        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        
        moveVelocity.y = _stats.JumpPower;
    }

    private void HandleGravity()
    {
        if (OnSlope()) return;

        if (_grounded && moveVelocity.y <= 0f) {
            moveVelocity.y = _stats.GroundingForce;
        } else {
            var inAirGravity = _stats.FallAcceleration;
            moveVelocity.y = Mathf.MoveTowards(moveVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    private void ApplyMovement()
    {
        _rb.velocity = moveVelocity;
    }

    
}
