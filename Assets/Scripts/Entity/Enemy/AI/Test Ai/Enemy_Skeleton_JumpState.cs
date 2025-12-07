using Unity.Netcode;
using UnityEngine;

public class Enemy_Skeleton_JumpState : Enemy_Skeleton_State
{
    public EnemyJumpData _jumpData;
    public string StateName;
    private Vector2 originalGroundCheckLocalPos;
    public Enemy_Skeleton_JumpState(Enemy_Skeleton enemySkeleton,Enemy_Skeleton_StateMachine enemyStateMachine, string animBoolName,EnemyJumpData JumpData) : base(enemySkeleton, enemyStateMachine, animBoolName)
    {
        _jumpData =  JumpData;
    }

    public override void Enter()
    {   
        base.Enter();
        originalGroundCheckLocalPos = enemySkeleton.groundCheck.localPosition;
        enemySkeleton.groundCheck.localPosition = new Vector2(0f, originalGroundCheckLocalPos.y);

        // 수평 속도와 수직 속도를 함께 적용하여 '도약' 구현
        float horizontalVelocity = enemySkeleton.battleMoveSpeed * enemySkeleton.FacingDirection;
        enemySkeleton.SetVelocity(horizontalVelocity, _jumpData.jumpForce);
        _jumpData.isJumping = true;
    }
    
    public override void Update()
    {
        base.Update();
        
        if (_jumpData.isJumping && enemySkeleton.IsGroundDetected)
        {
            _jumpData.isJumping = false;
            enemySkeleton.groundCheck.localPosition = originalGroundCheckLocalPos;
            ChangeStates(StateName);
        }
    }

    private void ChangeStates(string stateName)
    {
        if (stateName == "Chase_director")
        {
            enemyStateMachine.ChangeState(enemySkeleton.BattleState);
        }
        if (stateName == "Move_director")
        {
            enemyStateMachine.ChangeState(enemySkeleton.MoveState);
        }
    }
}