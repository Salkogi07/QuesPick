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

        // 이동 멈춤 -> 웅크리기 대기
        if (player.MoveInput == 0)
        {
            playerStateMachine.ChangeState(player.CrouchIdleState);
            return;
        }
        
        if (!player.IsCeilingDetected)
        {
            // 점프 입력 시 -> 즉시 점프
            if (player.IsJumpPressed)
            {
                playerStateMachine.ChangeState(player.JumpState);
                return;
            }

            // 달리기 입력 시 -> 즉시 달리기 (이미 이동 중이므로 MoveInput 체크 불필요)
            if (player.Condition.CanSprint() && player.IsSprintHeld)
            {
                playerStateMachine.ChangeState(player.RunState);
                return;
            }
        }

        // 웅크리기 키 뗌 -> 걷기 (천장 체크)
        if (!player.IsCrouchHeld && !player.IsCeilingDetected)
        {
            playerStateMachine.ChangeState(player.WalkState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        // 웅크리기 속도로 이동
        player.SetMoveSpeed(player.CrouchMoveSpeed);
        player.SetVelocity(player.MoveInput * player.CurrentSpeed, rigidbody.linearVelocity.y);
    }
}