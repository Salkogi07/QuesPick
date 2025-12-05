using UnityEngine;

public class Player_CrouchIdleState : Player_GroundedState
{
    public Player_CrouchIdleState(Player player, Player_StateMachine playerStateMachine, string animBoolName) : base(player, playerStateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, rigidbody.linearVelocity.y);
        
        // 콜라이더 크기 축소
        player.Collider.size = player.CrouchColliderSize;
        player.Collider.offset = player.CrouchColliderOffset;
    }

    public override void Exit()
    {
        base.Exit();
        
        // 콜라이더 크기 원상복구
        player.Collider.size = player.OriginalColliderSize;
        player.Collider.offset = player.OriginalColliderOffset;
    }

    public override void Update()
    {
        player.Condition.StaminaRecovery();

        // 공중으로 떨어지는 경우
        if (rigidbody.linearVelocity.y < -0.1f && !player.IsGroundDetected)
        {
            playerStateMachine.ChangeState(player.FallState);
            return;
        }
        
        // 천장이 없을 때 점프나 달리기 입력 처리
        if (!player.IsCeilingDetected)
        {
            // 점프 입력 시 -> 즉시 점프
            if (player.IsJumpPressed)
            {
                playerStateMachine.ChangeState(player.JumpState);
                return;
            }

            // 달리기 입력 + 이동 입력 시 -> 즉시 달리기
            // (제자리에서 달리기 키만 누르면 아무 일도 안 일어나는 것이 자연스러우므로 MoveInput 체크 포함)
            if (player.Condition.CanSprint() && player.IsSprintHeld && player.MoveInput != 0)
            {
                playerStateMachine.ChangeState(player.RunState);
                return;
            }
        }
        
        // 웅크리기 키를 뗐을 때 (천장이 없어야 일어날 수 있음)
        if (!player.IsCrouchHeld && !player.IsCeilingDetected)
        {
            playerStateMachine.ChangeState(player.IdleState);
            return;
        }

        // 이동 입력이 들어오면 웅크려 걷기로 전환
        if (player.MoveInput != 0)
        {
            playerStateMachine.ChangeState(player.CrouchMoveState);
            return;
        }
    }
}