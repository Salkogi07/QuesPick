using UnityEngine;
using Netcode;

public abstract class PlayerState
{
    protected Player player;
    protected Player_StateMachine playerStateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody2D rigidbody;

    public PlayerState(Player player, Player_StateMachine playerStateMachine, string animBoolName)
    {
        this.player = player;
        this.playerStateMachine = playerStateMachine;
        this.animBoolName = animBoolName;

        anim = player.Anim;
        rigidbody = player.rb;
    }

    public virtual void Enter()
    {
        anim.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {
        anim.SetFloat("yVelocity", rigidbody.linearVelocity.y);
    }

    public virtual void FixedUpdate()
    {
        anim.SetFloat("yVelocity", rigidbody.linearVelocity.y);
    }

    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
    }
}