using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IPointerClickHandler
{
    TMP_Text _text;
    ShopItem _shopItem;

    void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void SetItem(ShopItem shopItem)
    {
        _shopItem = shopItem;
        _text.text = _shopItem.Description;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Managers.ShopManager.ApplyItem(_shopItem);
        Managers.TurnManager.ChangeTurn(TurnStateFactory.GetState(Define.TurnStateType.EnterRoundTurn));
    }
}
