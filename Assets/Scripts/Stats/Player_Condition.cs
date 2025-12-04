using UnityEngine;
using System.Collections.Generic;
using R3;
using Unity.Netcode;

public class Player_Condition : Entity_Health
{
    private Player_Stats _stats;

    [Header("Player info")]
    private bool _isDead = false;

    [SerializeField] private float temperatureDropRate = 1f;
    [SerializeField] private int frozenHpDropRate = 2;
    private float _frozenDamageTimer = 0f;
    
    [Header("Stamina Settings")]
    [SerializeField] private float staminaRegenDelay = 1.5f; // 달리기를 멈춘 후 스테미나 회복이 시작되기까지의 지연 시간
    private float _staminaRegenTimer; // 지연 시간을 측정하기 위한 타이머
    private bool _isCurrentlySprinting; // 현재 달리기 상태인지 추적하는 변수

    #region current Stats
    private ReactiveProperty<float> _hpPropertiy;
    private ReactiveProperty<float> _staminaPropertiy;
    private ReactiveProperty<float> _weightPropertiy;
    private ReactiveProperty<float> _temperaturePropertiy;

    public float Hp
    {
        get => _hpPropertiy.Value;
        set => _hpPropertiy.Value = value;
    }
    public float Stamina
    {
        get => _staminaPropertiy.Value;
        set => _staminaPropertiy.Value = value;
    }
    public float Weight
    {
        get => _weightPropertiy.Value;
        set => _weightPropertiy.Value = value;
    }
    public float Temperature
    {
        get => _temperaturePropertiy.Value;
        set => _temperaturePropertiy.Value = value;
    }


    private Observable<float> _hpObservable;
    public Observable<float> HpObservable => _hpObservable ??= _hpPropertiy.AsObservable();


    private Observable<float> _staminaObservable;
    public Observable<float> StaminaObservable => _staminaObservable ??= _staminaPropertiy.AsObservable();


    private Observable<float> _weightObservable;
    public Observable<float> WeightObservable => _weightObservable ??= _weightPropertiy.AsObservable();


    private Observable<float> _temperatureObservable;
    public Observable<float> TemperatureObservable => _temperatureObservable ??= _temperaturePropertiy.AsObservable();
    #endregion

    private void Awake()
    {
        _stats = GetComponent<Player_Stats>();
        // _vfx = GetComponent<Entity_VFX>();

        _hpPropertiy = new ReactiveProperty<float>(_stats.MaxHp.Value);
        _staminaPropertiy = new ReactiveProperty<float>(_stats.MaxStamina.Value);
        _weightPropertiy = new ReactiveProperty<float>(0);
        _temperaturePropertiy = new ReactiveProperty<float>(_stats.MaxTemperature.Value);
    }
    private void Update()
    {
        HandleTemperature();
        HandleFrozenHp();

        //달리고 있지 않을 때만 스테미나 회복 타이머를 증가시킵니다
        if (!_isCurrentlySprinting)
        {
            _staminaRegenTimer += Time.deltaTime;
        }
    }

    #region Get Stat Function
    public float GetMaxHp() => _stats.MaxHp.Value;
    public float GeMaxStamina() => _stats.MaxStamina.Value;
    public float GetMaxWeight() => _stats.MaxWeight.Value;
    public float GetMaxTemperature() => _stats.MaxTemperature.Value;
    public bool CheckIsDead() => _isDead;
    #endregion

    #region Stamina Function

    public void AddStamina(float value)
    {
        Stamina += value;
        Stamina = Mathf.Clamp(Stamina, 0f, _stats.MaxStamina.Value);
    }
    
    public void SetSprintingStatus(bool isSprinting)
    {
        _isCurrentlySprinting = isSprinting;

        // 달리기가 시작되면 타이머를 리셋합니다.
        if (isSprinting)
        {
            _staminaRegenTimer = 0f;
        }
    }
    
    public void StaminaRecovery()
    {
        if (_staminaRegenTimer < staminaRegenDelay)
            return;

        float tempValue = Temperature / _stats.MaxTemperature.Value;
        if (GetTemperatureState(tempValue) == TemperatureState.Cold)
        {
            float targetStamina = _stats.MaxStamina.Value * 0.5f;

            if (Stamina > targetStamina)
                Stamina -= _stats.staminaDecreaseRate * Time.deltaTime;
            else
            {
                Stamina += _stats.StaminaRecoverRate * Time.deltaTime;
                Stamina = Mathf.Clamp(Stamina, 0f, targetStamina);
            }
        }
        else if (GetTemperatureState(tempValue) == TemperatureState.Freezing)
        {
            if (Stamina >= 0)
                Stamina -= _stats.staminaDecreaseRate * Time.deltaTime;
            else
                Stamina = Mathf.Clamp(Stamina, 0f, _stats.MaxStamina.Value);
        }
        else
        {
            Stamina += _stats.StaminaRecoverRate * Time.deltaTime;
            Stamina = Mathf.Clamp(Stamina, 0f, _stats.MaxStamina.Value);
        }
    }

    public void UseStaminaToSprint()
    {
        Stamina -= _stats.SprintCost * Time.deltaTime;
        Stamina = Mathf.Clamp(Stamina, 0f, _stats.MaxStamina.Value);
    }

    public void UseStaminaToJump()
    {
        Stamina -= _stats.JumpCost;
        Stamina = Mathf.Clamp(Stamina, 0f, _stats.MaxStamina.Value);
    }

    public bool CanSprint() => Stamina > 0;

    #endregion

    #region PlayerDamage Function
    public void EffectTakeDamage(int _damage)
    {
        // 오너만 대미지를 적용할 수 있도록 합니다.
        if (!IsOwner) return;

        if (_isDead)
            return;

        Hp -= _damage;
        if (Hp <= 0)
        {
            Hp = 0;
            Die();
        }
    }

    public override void TakeDamage(int _damage, Transform damageDealer)
    {
        // 오너만 대미지를 적용할 수 있도록 합니다.
        if (!IsOwner) return;
        
        if (_isDead)
            return;
        
        EntityMaxHealth = _stats.MaxHp.Value;
        
        Debug.Log("Player Damage: " + _damage);
        
        Hp -= _damage;
        if (Hp <= 0)
        {
            Hp = 0;
            Die();
        }
        
        base.TakeDamage(_damage, damageDealer);
    }

    void Die()
    {
        if (_isDead) return; // 중복 실행 방지
        _isDead = true;
        
        // 내가 이 캐릭터의 주인(Owner)일 경우에만 서버에 사망 사실을 보고합니다.
        if (IsOwner)
        {
            //NetworkTransmission.instance.ReportMyDeathServerRpc();
            Debug.Log("Reporting my death to the server.");
        }
    }
    #endregion

    public void ChangeTemperature(float value)
    {
        Temperature += value;
        Temperature = Mathf.Clamp(Temperature, 0f, _stats.MaxTemperature.Value);
    }

    private void HandleTemperature()
    {
        float dropRate = temperatureDropRate;

        /*if (playerMove.moveInput != 0)
            dropRate *= 0.5f;

        if (playerMove.isSprinting)
            dropRate *= 0.25f;

        if (playerMove.isAttack)
            dropRate *= 0.5f;*/

        Temperature -= dropRate * Time.deltaTime;
        Temperature = Mathf.Max(Temperature, 0f);
    }

    private void HandleFrozenHp()
    {
        float tempRatio = Temperature / _stats.MaxTemperature.Value;

        if (tempRatio <= 0)
        {
            _frozenDamageTimer += Time.deltaTime;

            if (_frozenDamageTimer >= 1f)
            {
                EffectTakeDamage(frozenHpDropRate);
                _frozenDamageTimer = 0f;
            }
        }
        else
        {
            _frozenDamageTimer = 0f;
        }
    }

    private TemperatureState GetTemperatureState(float tempRatio)
    {
        TemperatureState tempState;
        if (tempRatio >= 0.75f)
        {
            tempState = TemperatureState.Normal;
        }
        else if (tempRatio >= 0.5f)
        {
            tempState = TemperatureState.Chilly;
        }
        else if (tempRatio >= 0.25f)
        {
            tempState = TemperatureState.Cold;
        }
        else
        {
            tempState = TemperatureState.Freezing;
        }

        return tempState;
    }
}