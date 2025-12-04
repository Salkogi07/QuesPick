
using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(Enemy))]
public class Enemy_Health : Entity_Health
{
    protected Enemy enemy;
    // 참고: Enemy_Stats는 Enemy_Bomb_Monkey에 있으므로, 구체적인 Health 스크립트에서 참조합니다.

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(int damage, Transform damageDealer)
    {
        if (!IsServer) return; // 서버에서만 피해 처리

        base.TakeDamage(damage, damageDealer);
    }
}