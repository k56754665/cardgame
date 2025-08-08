using UnityEngine;
using UnityEngine.EventSystems;

public class EraseButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Managers.RoundManager.ClearExpression();
    }
}
