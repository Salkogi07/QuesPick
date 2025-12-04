using R3;
using UnityEngine;

public class PlayerStaminaUIPresenter : MonoBehaviour
{
    [SerializeField] private PlayerStaminaUIView view;
    [SerializeField] private Player_Condition model;
    
    public void SetPlayerModel(Player_Condition model)
    {
        this.model = model;
        
        this.model.StaminaObservable.Subscribe(stamina =>
        {
            view.UpdateHp(stamina, this.model.GeMaxStamina());
        });
    }
}