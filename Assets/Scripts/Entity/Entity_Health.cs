using Unity.Netcode;
using UnityEngine;

public class Entity_Health : NetworkBehaviour
{
    private Entity_VFX _entityVFX;
    private Entity _entity;
    
    protected float EntityMaxHealth;
    
    [Header("On Damage Knockback")]
    [SerializeField]  private float knockbackDuration = .2f;
    [SerializeField]  private Vector2 KnockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField]  private Vector2 heavyKnockbackpower = new Vector2(7f, 7f);
    [SerializeField]  private float heavyKnockbackDuration = .5f;
    [Header("On Heavy Damage")]
    [SerializeField]  private float heavyDamageThreshold = .3f;
    
    protected virtual void Awake()
    {
        _entity = GetComponent<Entity>();
        _entityVFX = GetComponent<Entity_VFX>();
    }
    
    public virtual void TakeDamage(int damage, Transform damageDealer)
    {
        Vector2 knockback = CalculateKnockback(damage,damageDealer);
        float duration = CalculateDuration(damage);
        
        // 대미지 비례 넉백량 증가
        _entity?.Reciveknockback(knockback, duration);
        
        _entityVFX?.PlayOnDamageVfx();
    }
    
    // 대미지 비례 넉백량 증가
    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        
        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackpower : KnockbackPower;
        
        knockback.x *= direction;
        
        return knockback;
    }
    // 대미지 비례 넉백량 증가
    private bool IsHeavyDamage(float damage) => damage / EntityMaxHealth > heavyDamageThreshold;
}
