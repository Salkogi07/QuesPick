using System;
using UnityEngine;

[Serializable]
public class EnemyJumpData
{
    [Header("Jump Data")]
    public float jumpForce;
    public float jumpCoolTime;
    public bool isJumping = false;
    public float jumpVelocity;

    [Header("Collision detection [Wall]")]
    public LayerMask whatIsJump;
    public Transform primaryJumpCheck;
    public float jumpCheckDistance;
    public bool IsJumpDetected { get; set; }

    // [HideInInspector]

    // public EnemyJumpData Clone()
    // {
    //     return new EnemyJumpData
    //     {
    //         jumpForce = this.jumpForce,
    //         jumpCoolTime = this.jumpCoolTime,
    //         isJumping = this.isJumping
    //     };
    // }
}