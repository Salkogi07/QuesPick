using UnityEngine;

public class Enemy_Skeleton_AnimationTrigger : MonoBehaviour
{
    private Enemy_Skeleton enemy;
    private Enemy_Combat enemyCombat;
    
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Skeleton>();
        enemyCombat = GetComponentInParent<Enemy_Combat>();
    }

    public void CurrentStateTrigger()
    {
        enemy.CallAnimationTrigger();
    }

    public void AttackTrigger()
    {
        enemyCombat.PerformAttack();
    }
    
}