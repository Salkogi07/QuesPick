using UnityEngine;

public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(Player player, Player_StateMachine playerStateMachine, string stateName) : base(player, playerStateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(0, rigidbody.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();
            
        player.Condition.StaminaRecovery();
            
        if (player.MoveInput != 0)
            playerStateMachine.ChangeState(player.WalkState);
    }
}
