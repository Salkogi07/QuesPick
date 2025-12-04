using UnityEngine;

public class Player_FallState : Player_AiredState
{
    public Player_FallState(Player player, Player_StateMachine playerStateMachine, string animBoolName) : base(player,
        playerStateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        player.Condition.StaminaRecovery();

        if (player.IsGroundDetected)
            playerStateMachine.ChangeState(player.IdleState);
    }
}