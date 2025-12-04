using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUIView : MonoBehaviour
{
    [SerializeField] private Image hpImage;

    public void UpdateHp(float value,  float maxValue)
    {
        float hpValue = value / maxValue;
        hpImage.fillAmount = hpValue;
    }
}