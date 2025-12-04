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
        player.Condition.SetSprintingStatus(true); // 달리기 시작을 알립니다.
    }

    public override void Exit()
    {
        base.Exit();
        player.Condition.SetSprintingStatus(false); // 달리기가 끝났음을 알립니다.
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
            
        player.SetMoveSpeed(player.Stats.RunSpeed);
        player.SetVelocity(player.MoveInput * player.CurrentSpeed, rigidbody.linearVelocity.y);
    }
}