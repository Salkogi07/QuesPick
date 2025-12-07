
    using UnityEngine;

    public class Enemy_Skeleton_MoveState : Enemy_Skeleton_State
    {
        
        public Enemy_Skeleton_MoveState(Enemy_Skeleton enemySkeleton, Enemy_Skeleton_StateMachine enemyStateMachine, string animBoolName) : base(
            enemySkeleton,enemyStateMachine, animBoolName)
        {
            
        }

        public override void Enter()
        {
            base.Enter();
            // if (enemySkeleton.IsGroundDetected == false || enemySkeleton.IsWallDetected)
            // {
            //     enemySkeleton.Flip();
            // }
        }

        public override void Update()
        {
        base.Update();
        
        if (enemySkeleton.JumpState._jumpData.IsJumpDetected )
        {
            if (enemySkeleton.CanPerformLeap())
            {
                
                enemySkeleton.JumpState.StateName = "Move_director";
                            enemyStateMachine.ChangeState(enemySkeleton.JumpState);
            }
            
        }
        if (!enemySkeleton.JumpState._jumpData.isJumping && (enemySkeleton.IsWallDetected || !enemySkeleton.IsGroundDetected))
        {
            enemyStateMachine.ChangeState(enemySkeleton.IdleState);
        }
        if (enemySkeleton.PlayerDetection() == true)
        {
            enemyStateMachine.ChangeState(enemySkeleton.BattleState);
        }
        }

        public override void FiexedUpdate()
        {
            base.FiexedUpdate();

            enemySkeleton.SetVelocity(enemySkeleton.MoveSpeed * enemySkeleton.FacingDirection, rb.linearVelocity.y);
        }


    // public bool MeasureWallHeight()
    // {
    //     
    //     Vector2 startpoint = new Vector2(enemy.transform.position.x + (enemy.wallCheckDistance * enemy.FacingDirection), enemy.transform.position.y);
    //        
    //     while (enemy.GetState<Enemy_JumpState>()._jumpData.jumpForce <= startpoint.y)
    //     {
    //         startpoint = new Vector2(startpoint.x, startpoint.y + 0.01f);
    //         RaycastHit2D hit = Physics2D.Raycast(startpoint, Vector2.up, enemy.GetState<Enemy_JumpState>()._jumpData.jumpForce, enemy.whatIsWall);
    //         return true;
    //     }
    //     return false;
    // }
}