using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUISetup : NetworkBehaviour
{
    private GameObject _precenters;

    //싱글 테스트용 (나중에 지우기)
    public void Start()
    {
        _precenters = GameObject.FindGameObjectWithTag("PlayerPrecenters");
        if (_precenters != null)
        {
            SettingPlayerUI(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _precenters = GameObject.FindGameObjectWithTag("PlayerPrecenters");
            if (_precenters != null)
            {
                SettingPlayerUI(gameObject);
            }
        }
    }
    
    private void SettingPlayerUI(GameObject player)
    {
        Player_Condition playerCondition = player.GetComponent<Player_Condition>();
        _precenters.GetComponent<PlayerHpUIPrecenter>().SetPlayerModel(playerCondition);
        _precenters.GetComponent<PlayerStaminaUIPresenter>().SetPlayerModel(playerCondition);
        _precenters.GetComponent<PlayerTemperatureUIPresenter>().SetPlayerModel(playerCondition);
        _precenters.GetComponent<PlayerWeightUIPresenter>().SetPlayerModel(playerCondition);
    }
}