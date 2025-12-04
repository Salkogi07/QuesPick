using Steamworks.Data;
using System;
using UnityEngine;
namespace Stats
{
    public class Enemy_Stats : MonoBehaviour
    {
        [Header("Stat info")]
        [SerializeField] public Stat maxHealth;
        [SerializeField] public Stat damage;
        [SerializeField] public float armor;
        [SerializeField] public float Groggy;
        [SerializeField] public float speed;

        public int difficulty = 1;
    }
}