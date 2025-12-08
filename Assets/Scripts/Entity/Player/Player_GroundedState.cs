using UnityEngine;

public class Player_GroundedState : PlayerState
{
    public Player_GroundedState(Player player, Player_StateMachine playerStateMachine, string animBoolName) : base(
        player, playerStateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        // [수정됨] 경사면에 있지 않을 때만 Y속도를 0으로 초기화
        // 경사면에서는 Y축 이동(오르막/내리막)이 필요하므로 0으로 만들지 않음
        if (player.IsGroundDetected && !player.IsOnSlope)
        {
            var v = rigidbody.linearVelocity;
            v.y = 0f;
            rigidbody.linearVelocity = v;
        }

        if (rigidbody.linearVelocity.y < 0 && !player.IsGroundDetected)
        {
            playerStateMachine.ChangeState(player.FallState);
            return;
        }
        
        if (player.IsCrouchHeld && !player.IsSprintHeld)
        {
            if (player.MoveInput != 0)
                playerStateMachine.ChangeState(player.CrouchMoveState);
            else
                playerStateMachine.ChangeState(player.CrouchIdleState);
            
            return;
        }

        if (player.Condition.CanSprint())
            if (Input.GetKey(KeyManager.instance.GetKeyCodeByName("Sprint")) && player.MoveInput != 0)
                playerStateMachine.ChangeState(player.RunState);

        if (player.IsGroundDetected && Input.GetKeyDown(KeyManager.instance.GetKeyCodeByName("Jump")))
            playerStateMachine.ChangeState(player.JumpState);
    }
}