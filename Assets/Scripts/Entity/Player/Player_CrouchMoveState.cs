using UnityEngine;

public class Player_CrouchMoveState : Player_GroundedState
{
    public Player_CrouchMoveState(Player player, Player_StateMachine playerStateMachine, string animBoolName) : base(player, playerStateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Collider.size = player.CrouchColliderSize;
        player.Collider.offset = player.CrouchColliderOffset;
    }

    public override void Exit()
    {
        base.Exit();
        player.Collider.size = player.OriginalColliderSize;
        player.Collider.offset = player.OriginalColliderOffset;
    }

    public override void Update()
    {
        player.Condition.StaminaRecovery();

        if (rigidbody.linearVelocity.y < -0.1f && !player.IsGroundDetected)
        {
            playerStateMachine.ChangeState(player.FallState);
            return;
        }

        if (player.MoveInput == 0)
        {
            playerStateMachine.ChangeState(player.CrouchIdleState);
            return;
        }
        
        if (!player.IsCeilingDetected)
        {
            if (player.IsJumpPressed)
            {
                playerStateMachine.ChangeState(player.JumpState);
                return;
            }

            if (player.Condition.CanSprint() && player.IsSprintHeld)
            {
                playerStateMachine.ChangeState(player.RunState);
                return;
            }
        }

        if (!player.IsCrouchHeld && !player.IsCeilingDetected)
        {
            playerStateMachine.ChangeState(player.WalkState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        player.SetGroundVelocity(player.MoveInput, player.Stats.CrouchSpeed);
    }
}