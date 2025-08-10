using UnityEngine;

public class ShopTurn : ITurnState
{
    public void EnterState()
    {
        Managers.ShopManager.OpenShop();
    }

    public void ExitState()
    {

    }

    public void UpdateState()
    {
        Managers.ShopManager.CloseShop();
    }
}
