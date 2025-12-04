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
            playerStateMachine.ChangeState(player.FallState);
        
        if (player.CanMine && Input.GetKey(KeyManager.instance.GetKeyCodeByName("Mining")))
        {
            // 마우스 위치에 채굴 가능한 타일이 있는지 확인하는 추가 조건이 필요할 수 있습니다.
            //playerStateMachine.ChangeState(player.MiningState);
            return;
        }

        //if (player.Condition.CanSprint())
            if (Input.GetKey(KeyManager.instance.GetKeyCodeByName("Sprint")) && player.MoveInput != 0)
                playerStateMachine.ChangeState(player.RunState);

        if (Input.GetKeyDown(KeyManager.instance.GetKeyCodeByName("Jump")))
            playerStateMachine.ChangeState(player.JumpState);
    }
}