using UnityEngine;

public class ShopTurn : ITurnState
{
    public void EnterState()
    {
        Managers.ShopManager.OpenShop();
    }

    public void ExitState()
    {
        Managers.ShopManager.CloseShop();
    }

    public void UpdateState()
    {

    }
}
