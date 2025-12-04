
using System;
using Stats;
using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{

    public int damage = 10;
    // public Collider2D[] targetColliders;


    [Header("Target detection")]
    [SerializeField] private Transform targetChack;
    [SerializeField] private float targetcheckRadius;
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private Enemy_Stats stats;

    private void Start()
    {
        stats = GetComponent<Enemy_Stats>();
    }


    public void PerformAttack()
    {
        GetDetectedColliders();
        // entityHealth?.TakeDamage(damage,transform);
        foreach (var target in GetDetectedColliders())
        {
            // Player_Condition playerCondition = target.GetComponent<Player_Condition>();
            //
            // playerCondition?.TakeDamage(damage);
            damage = stats.damage.GetValue();
            target.GetComponent<Entity_Health>().TakeDamage(damage, transform);

        }
    }

    private Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetChack.position, targetcheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetChack.position, targetcheckRadius);
    }
}
