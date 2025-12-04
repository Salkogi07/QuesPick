using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeightUIView : MonoBehaviour
{
    [SerializeField] private Image weightImage;
    [SerializeField] private Sprite[] weightSprites;
    [SerializeField] private TMP_Text weightText;
    
    public void UpdateWeight(float value,  float maxValue)
    {
        float weightValue = value / maxValue;
        weightText.text = value + "KG";
        weightImage.sprite = weightSprites[(int)GetWeightState(weightValue)];
    }
    
    private WeightState GetWeightState(float tempRatio)
    {
        if (tempRatio >= 0.75f)
        {
            return WeightState.Heavy;
        }
        else if (tempRatio >= 0.4f)
        {
            return WeightState.Medium;
        }
        else
        {
            return WeightState.Light;
        }
    }
}