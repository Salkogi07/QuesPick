using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaUIView : MonoBehaviour
{
    [Header("Stamina GUI")]
    [SerializeField] Image staminaImage;
    
    public void UpdateHp(float value,  float maxValue)
    {
        float staminaValue = value / maxValue;
        staminaImage.fillAmount = staminaValue;
    }
}