using R3;
using UnityEngine;

public class PlayerTemperatureUIPresenter : MonoBehaviour
{
    [SerializeField] private PlayerTemperatureUIView view;
    [SerializeField] private Player_Condition model;
    
    public void SetPlayerModel(Player_Condition model)
    {
        this.model = model;
        
        this.model.TemperatureObservable.Subscribe(temp =>
        {
            view.UpdateTemperature(temp, this.model.GetMaxTemperature());
            view.UpdateEdges(temp, this.model.GetMaxTemperature());
        });
    }
}