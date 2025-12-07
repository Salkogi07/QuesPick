using UnityEngine;


public class Enemy_Skeleton_DeadState : Enemy_Skeleton_State
{
    private Collider2D col;


    public Enemy_Skeleton_DeadState(Enemy_Skeleton enemySkeleton, Enemy_Skeleton_StateMachine enemyStateMachine, string animBoolName) : base(enemySkeleton, enemyStateMachine, animBoolName)
    {
        col = enemySkeleton.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        base.Enter();
        //애니메이션이 멈춤
        // anim.enabled = false;
        // col.enabled = false;
        //
        // rb.gravityScale = 12;
        // rb.linearVelocity = new Vector2(rb.linearVelocity.x, 15);
        Debug.Log("Entered");
        enemyStateMachine.SwitchOffStateMachine();
    }
}