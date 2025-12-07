
using UnityEngine;

public class Enemy_Skeleton_BattleState : Enemy_Skeleton_State
{
    
    private Transform player;
    private float lastTimeWasInBattle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public Enemy_Skeleton_BattleState(Enemy_Skeleton enemySkeleton,Enemy_Skeleton_StateMachine enemyStateMachine, string animBoolName) : base(enemySkeleton, enemyStateMachine, animBoolName)
    {
    }
    
    public override void Enter()
    {
        
        base.Enter();
        UpdateBattleTimer();
        
        if (player == null)
        {
            player = enemySkeleton.GetPlayerReference();
        }
        // player ??= enemy.GetPlayerReference();
        if (shouldRetreat())
        {
            rb.linearVelocity = new Vector2(enemySkeleton.retreatVelocity.x*-DirectionToPlayer(),enemySkeleton.retreatVelocity.y);
            enemySkeleton.HandleFlip(DirectionToPlayer());
        }
        
    }

    public override void Update()
    {
        base.Update();
        
        if (enemySkeleton.PlayerDetection() == true)
        {
            UpdateBattleTimer();
        }

        if (BattleTimeIsOver())
        {
            enemyStateMachine.ChangeState(enemySkeleton.IdleState);
        }
        if (WithinAttackRange() && enemySkeleton.PlayerDetection())
        {
            enemyStateMachine.ChangeState(enemySkeleton.AttackState);
        }

        if( enemySkeleton.JumpState._jumpData.IsJumpDetected||player.transform.position.y > enemySkeleton.transform.position.y+1f)
        {
            
            enemySkeleton.JumpState.StateName = "Chase_director";
            enemyStateMachine.ChangeState(enemySkeleton.JumpState);
            
        }
        
    }

    public override void FiexedUpdate()
    {
        base.FiexedUpdate();
        enemySkeleton.SetVelocity(enemySkeleton.battleMoveSpeed * DirectionToPlayer(), rb.linearVelocity.y);
    }

    private void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;
    private bool BattleTimeIsOver() =>Time.time > lastTimeWasInBattle + enemySkeleton.battleTimeDuration;
    private bool WithinAttackRange() => DistanceToPlayer() < enemySkeleton.attackDistance;
    private bool shouldRetreat() => DistanceToPlayer() < enemySkeleton.minRetreatDistance;
        
    
    
    private float DistanceToPlayer()
    {
        if (player == null)
        {
            return float.MaxValue;
        }
        return Mathf.Abs(player.position.x - enemySkeleton.transform.position.x);
    }

    private int DirectionToPlayer()
    {
        if (player == null)
        {
            return 0;
        }
        return player.position.x > enemySkeleton.transform.position.x ? 1 : -1;
    }
}