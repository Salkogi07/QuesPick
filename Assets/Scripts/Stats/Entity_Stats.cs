using System;
using UnityEngine;

namespace Stats
{
    public class Entity_Stats : MonoBehaviour
    {
        [SerializeField] protected float health;
        [SerializeField] protected bool isDead = false;
        public Stat MaxHealth;
        public Stat_OffenseGroup Offense;
    }
}