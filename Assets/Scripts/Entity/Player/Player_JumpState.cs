using UnityEngine;

public class Player_JumpState : Player_AiredState
{
    private bool hasLeftGround = false; 
    public Player_JumpState(Player player, Player_StateMachine playerStateMachine, string animBoolName) : base(player, playerStateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Condition.UseStaminaToJump();
        
        player.CoyoteTimeCounter = 0;
        
        hasLeftGround = false; 
        
        float horizontalVelocity = player.MoveInput * player.CurrentSpeed;
        player.SetVelocity(horizontalVelocity, player.Stats.JumpForce);
    }

    public override void Update()
    {
        base.Update();
        
        if (!player.IsGroundDetected)
        {
            hasLeftGround = true;
        }
        
        if (hasLeftGround && player.IsGroundDetected)
        {
            playerStateMachine.ChangeState(player.IdleState);
            return;
        }

        if (rigidbody.linearVelocity.y < 0)
            playerStateMachine.ChangeState(player.FallState);
    }
}