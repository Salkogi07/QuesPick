using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTemperatureUIView : MonoBehaviour
{
    [Header("Temperature GUI")]
    [SerializeField] Image temperatureImage;
    
    [Header("Freeze Edges GUI")]
    [SerializeField] private TemperatureEdgeDate[] freezeEdges;
    [SerializeField] private float fadeSpeed = 2f;

    private TemperatureState currentState;

    private void Start()
    {
        foreach (var edge in freezeEdges)
        {
            SetImageAlpha(edge.image, 0);
        }
    }
    
    private void Update()
    {
        foreach (var edge in freezeEdges)
        {
            // 현재 상태가 이 효과가 나타나야 할 상태(또는 그보다 더 추운 상태)인지 확인
            bool shouldBeVisible = currentState >= edge.state;
            float targetAlpha = shouldBeVisible ? edge.alpha : 0f;

            // 현재 알파값을 목표 알파값으로 부드럽게 변경
            float newAlpha = Mathf.MoveTowards(edge.image.color.a, targetAlpha, fadeSpeed * Time.deltaTime);
            SetImageAlpha(edge.image, newAlpha);
        }
    }

    public void UpdateTemperature(float value,  float maxValue)
    {
        float temperatureValue = value / maxValue;
        temperatureImage.fillAmount = temperatureValue;
    }

    public void UpdateEdges(float value,  float maxValue)
    {
        float tempRatio = value / maxValue;
        currentState = GetTemperatureState(tempRatio);
        
        temperatureImage.sprite = freezeEdges[(int)currentState].sprite;
    }

    /// <summary>
    /// 이미지 알파값 변경하는 함수
    /// </summary>
    /// <param name="image"></param>
    /// <param name="alpha"></param>
    private void SetImageAlpha(Image image, float alpha)
    {
        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
    
    private TemperatureState GetTemperatureState(float tempRatio)
    {
        if (tempRatio >= 0.75f)
        {
            return TemperatureState.Normal;
        }
        else if (tempRatio >= 0.5f)
        {
            return TemperatureState.Chilly;
        }
        else if (tempRatio >= 0.25f)
        {
            return TemperatureState.Cold;
        }
        else
        {
            return TemperatureState.Freezing;
        }
    }
}