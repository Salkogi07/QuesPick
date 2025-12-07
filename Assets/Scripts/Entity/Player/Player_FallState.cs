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
        
        // 공중(Fall)이지만 코요테 시간이 남아있고, 점프 입력이 있으면 점프 허용
        if (player.CoyoteTimeCounter > 0 && player.JumpBufferCounter > 0)
        {
            player.JumpBufferCounter = 0; // 점프 입력 소모
            playerStateMachine.ChangeState(player.JumpState);
            return;
        }

        // 땅에 닿으면 Idle로 전환
        if (player.IsGroundDetected)
            playerStateMachine.ChangeState(player.IdleState);
    }
}