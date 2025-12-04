using UnityEngine;

public class Player_AiredState : PlayerState
{
    public Player_AiredState(Player player, Player_StateMachine playerStateMachine, string animBoolName) : base(player,
        playerStateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float xVelocity = player.MoveInput * (player.CurrentSpeed * player.inAirMoveMultiplier);

        if (player.MoveInput != 0)
            player.SetVelocity(xVelocity, rigidbody.linearVelocity.y);
    }
}