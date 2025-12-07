using UnityEngine;

public class Enemy_Skeleton_GroggyState : Enemy_Skeleton_State
{
    private float Groggying;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Enemy_Skeleton_GroggyState(Enemy_Skeleton enemySkeleton, Enemy_Skeleton_StateMachine enemyStateMachine, string animBoolName) : base(enemySkeleton, enemyStateMachine, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        Groggying = enemySkeleton.stats.Groggy;
    }
    

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if(Groggying <= 0)
        {
            enemyStateMachine.ChangeState(enemySkeleton.IdleState);
        }
        else
        {
            GrooggingTime();
        }
    }
    private void GrooggingTime() =>  Groggying  -= Time.deltaTime;
}