using UnityEngine;
using FMOD.Studio;

public class Player_RunState : Player_GroundedState
{
    public Player_RunState(Player player, Player_StateMachine playerStateMachine, string animBoolName) : base(player, playerStateMachine, animBoolName)
    {
    }
        
    public override void Enter()
    {
        base.Enter();
        player.Condition.SetSprintingStatus(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.Condition.SetSprintingStatus(false);
    }
    
    public override void Update()
    {
        player.Condition.UseStaminaToSprint();

        if (player.MoveInput == 0)
        {
            playerStateMachine.ChangeState(player.IdleState);
            return;
        }

        if (Input.GetKeyUp(KeyManager.instance.GetKeyCodeByName("Sprint")))
        {
            playerStateMachine.ChangeState(player.WalkState);
            return;
        }

        if (player.Condition.Stamina <= 0)
        {
            playerStateMachine.ChangeState(player.WalkState);
            return;
        }
        
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        player.SetGroundVelocity(player.MoveInput, player.Stats.RunSpeed);
    }
}