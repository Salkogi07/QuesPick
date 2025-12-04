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
        if(player.IsOwner)
            anim.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {
        if(player.IsOwner)
            anim.SetFloat("yVelocity", rigidbody.linearVelocity.y);
    }

    public virtual void FixedUpdate()
    {
        if(player.IsOwner)
            anim.SetFloat("yVelocity", rigidbody.linearVelocity.y);
    }

    public virtual void Exit()
    {
        if(player.IsOwner)
            anim.SetBool(animBoolName, false);
    }
}