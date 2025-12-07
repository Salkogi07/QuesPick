using System;
using UnityEngine;

public class Enemy_Skeleton_Health : MonoBehaviour
{
    
    // private Enemy_Skeleton enemy => GetComponent<Enemy_Skeleton>();
    private Enemy_Skeleton enemy;
    private Entity_VFX _entityVFX;
    private Entity entity;
    

    [SerializeField] protected int health;
    [SerializeField] protected Stat maxHealth; 
    [SerializeField] protected bool isDead;
    [Header("On Damage Knockback")]
    [SerializeField]  private float knockbackDuration = .2f;
    [SerializeField]  private Vector2 KnockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField]  private Vector2 heavyKnockbackpower = new Vector2(7f, 7f);
    [SerializeField]  private float heavyKnockbackDuration = .5f;
    [Header("On Heavy Damage")]
    [SerializeField]  private float heavyDamageThreshold = .3f;
    
    protected virtual void Awake()
    {
        enemy = GetComponent<Enemy_Skeleton>();
        entity = GetComponent<Entity>();
        _entityVFX = GetComponent<Entity_VFX>();
        
        
        health = maxHealth.GetValue();
    }

    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(5, transform);
        }
    }


    public void TakeDamage(int damage , Transform damageDealer)
    {
        // if (isDead)
        // {
        //     return;
        // }
        // if (damageDeaaler.GetComponent<Player>() != null)
        // {
        //     
        //     enemy.TryEnterBattleState(damageDeaaler);
        // }
        
        Vector2 knockback = CalculateKnockback(damage,damageDealer);
        float duration = CalculateDuration(damage);
        
        
        // enemy?.Reciveknockback(knockback, knockbackDuration);
        
        // 대미지 비례 넉백량 증가
        entity?.Reciveknockback(knockback, duration);
        
        _entityVFX?.PlayOnDamageVfx();
        ReduceHp(damage);
        
    }

    private void ReduceHp(int damage)
    {
        health -= damage;
        // Debug.Log(health);
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        enemy.Deading();
    }

    
    // 대미지 비례 넉백량 증가
    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        
        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackpower : KnockbackPower;
        
        knockback.x = knockback.x * direction;
        
        return knockback;
    }
    // 대미지 비례 넉백량 증가
    private bool IsHeavyDamage(float damage) => damage / maxHealth.GetValue() > heavyDamageThreshold;
}
    
    
  