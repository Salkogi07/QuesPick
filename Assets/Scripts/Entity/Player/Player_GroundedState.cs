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

        /*float y = rigidbody.linearVelocity.y;
        if (Mathf.Abs(y) < 0.0001f)
            y = 0f;*/

        if (player.IsGroundDetected)
        {
            var v = rigidbody.linearVelocity;
            v.y = 0f;
            rigidbody.linearVelocity = v;
        }

        if (rigidbody.linearVelocity.y < 0)
        {
            playerStateMachine.ChangeState(player.FallState);
            return;
        }
        
        if (player.IsCrouchHeld)
        {
            // 이동 중이면 CrouchMove, 멈춰있으면 CrouchIdle
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