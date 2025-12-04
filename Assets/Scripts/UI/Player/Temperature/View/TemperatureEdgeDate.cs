using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TemperatureEdgeDate
{
    public TemperatureState state;
    public Image image;
    public Sprite sprite;
    [Range(0, 1)]
    public float alpha;
}