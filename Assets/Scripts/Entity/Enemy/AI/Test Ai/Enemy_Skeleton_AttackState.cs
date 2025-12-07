using UnityEngine;

public class Enemy_Skeleton_AttackState : Enemy_Skeleton_State
{
    public Enemy_Skeleton_AttackState(Enemy_Skeleton enemySkeleton, Enemy_Skeleton_StateMachine stateMachine, string animBoolName) : base(enemySkeleton,
        stateMachine, animBoolName)
    {
        
    }

    public override void Update()
    {
        base.Update();
        
        if(triggerCalled)
        {
            enemyStateMachine.ChangeState(enemySkeleton.BattleState);
            // 끝없는 추격
        }
        
        
            
    }
}