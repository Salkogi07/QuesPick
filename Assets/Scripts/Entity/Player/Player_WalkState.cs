using FMOD.Studio;
using UnityEngine;

public class Player_WalkState : Player_GroundedState
{
    public Player_WalkState(Player player, Player_StateMachine playerStateMachine, string stateName) : base(player, playerStateMachine, stateName)
    {
    }

    public override void Update()
    {
        base.Update();
        
        player.Condition.StaminaRecovery();
        
        if (player.MoveInput == 0)
            playerStateMachine.ChangeState(player.IdleState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        player.SetGroundVelocity(player.MoveInput, player.Stats.WalkSpeed);
    }
}