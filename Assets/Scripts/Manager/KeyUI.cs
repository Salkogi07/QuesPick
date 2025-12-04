using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyUI : MonoBehaviour
{
    public string keyName;
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI keyExplanation;
    public Button keyButton;
    public Button resetButton;

    private void OnValidate()
    {
        gameObject.name = keyName;
    }

    private void Start()
    {
        keyExplanation.text = keyName;
        keyButton.onClick.AddListener(() => KeyManager.instance.StartRebinding(keyName));
        resetButton.onClick.AddListener(() => KeyManager.instance.ResetKey(keyName));
    }
}
