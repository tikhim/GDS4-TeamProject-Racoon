using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EntityAttributes : ScriptableObject
{

        [Header("MOVEMENT")]
        public float WalkSpeed = 14;
        public float SprintSpeed = 20;
        public float GroundAcceleration = 120;
        public float GroundDeceleration = 60;
        public float AirDeceleration = 30;
        public float AirAcceleration = 30;

        [Range(0f, -10f)]
        public float GroundingForce = -1.5f;
        public float MaxSlopeAngle = 45f;
        public float rotSpeed = 5f;

        [Header("JUMP")] 
        public bool canJump = true;
        public float JumpPower = 36;
        public float MaxFallSpeed = 40;
        public float FallAcceleration = 110;
        public float CoyoteTime = .15f;
        public float JumpBuffer = .2f;
}
