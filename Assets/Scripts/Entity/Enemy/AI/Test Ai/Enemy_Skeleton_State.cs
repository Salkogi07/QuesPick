using UnityEngine;



public abstract class Enemy_Skeleton_State
{
    protected Enemy_Skeleton enemySkeleton;
    protected Enemy_Skeleton_StateMachine enemyStateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody2D rb;

    protected float stateTimer;
    protected bool triggerCalled;

    public Enemy_Skeleton_State(Enemy_Skeleton enemySkeleton, Enemy_Skeleton_StateMachine enemyStateMachine, string animBoolName)
    {
        this.enemySkeleton = enemySkeleton;
        this.enemyStateMachine = enemyStateMachine;
        this.animBoolName = animBoolName;

        anim = enemySkeleton.Anim;
        rb = enemySkeleton.rb;
    }

    public virtual void Enter()
    {
        anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        float battleAnimSpeedMultiplier = enemySkeleton.battleMoveSpeed / enemySkeleton.MoveSpeed;

        anim.SetFloat("BattleAnimSpeedMultiplier", battleAnimSpeedMultiplier);
        anim.SetFloat("moveAnimSpeedMultiplier", enemySkeleton.moveAnimSpeedMultiplier);
        // anim.SetFloat("xVelocity", rigidbody.linearVelocity.x);
        anim.SetFloat("xVelocity", Mathf.Clamp(rb.linearVelocityX, -1f, 1f));
    }

    public virtual void FiexedUpdate()
    {
        anim.SetFloat("moveAnimSpeedMultiplier", enemySkeleton.moveAnimSpeedMultiplier);
        // anim.SetFloat("yVelocity", rigidbody.linearVelocity.y);
    }

    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
    }

    public void CallAnimationTrigger()
    {
        triggerCalled = true;
    }
}