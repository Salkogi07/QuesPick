using R3;
using UnityEngine;

public class PlayerWeightUIPresenter : MonoBehaviour
{
    [SerializeField] private PlayerWeightUIView view;
    [SerializeField] private Player_Condition model;
    
    public void SetPlayerModel(Player_Condition model)
    {
        this.model = model;
        
        this.model.WeightObservable.Subscribe(temp =>
        {
            view.UpdateWeight(temp, this.model.GetMaxWeight());
        });
    }
}