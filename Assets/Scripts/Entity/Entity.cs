using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    public Animator Anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    private Coroutine KnockbackCo;
    protected bool isknocked = false;

    protected virtual void Awake()
    {
        Anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Reciveknockback(Vector2 knockback, float duration)
    {
        if (KnockbackCo != null)
        {
            StopCoroutine(KnockbackCo);
        }

        KnockbackCo = StartCoroutine(konckbackCo(knockback, duration));
    }


    private IEnumerator konckbackCo(Vector2 knockback, float duration)
    {
        isknocked = true;
        rb.linearVelocity = knockback;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isknocked = false;
    }
}