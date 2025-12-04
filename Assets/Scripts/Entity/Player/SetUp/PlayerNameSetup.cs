using Unity.Netcode;
using Unity.Collections;
using TMPro;
using UnityEngine;

public class PlayerNameSetup : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    private NetworkVariable<FixedString64Bytes> playerName = 
        new NetworkVariable<FixedString64Bytes>(
            default, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += OnNameChanged;
        
        // nameText가 null이 아닌지 확인 후 UI 업데이트
        if (nameText != null && !string.IsNullOrEmpty(playerName.Value.ToString()))
        {
            OnNameChanged(default, playerName.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        playerName.OnValueChanged -= OnNameChanged;
    }

    private void OnNameChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        // UI 업데이트 전에 nameText가 null이 아닌지 다시 확인
        if (nameText != null)
        {
            nameText.text = newValue.ToString();
        }
        else
        {
            // 할당되지 않았다면 개발자가 알 수 있도록 경고를 출력합니다.
            Debug.LogWarning($"NameText is not assigned on {gameObject.name}. Cannot display name.");
        }
    }

    public void SetPlayerName(string name)
    {
        playerName.Value = name;
    }
}