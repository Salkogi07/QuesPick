using UnityEngine;

public class Player_JumpState : Player_AiredState
{
    public Player_JumpState(Player player, Player_StateMachine playerStateMachine, string animBoolName) : base(player, playerStateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Condition.UseStaminaToJump();
        
        float horizontalVelocity = player.MoveInput * player.CurrentSpeed;

        player.SetVelocity(horizontalVelocity, player.Stats.JumpForce);
    }

    public override void Update()
    {
        base.Update();

        if (rigidbody.linearVelocity.y < 0)
            playerStateMachine.ChangeState(player.FallState);
    }
}