using UnityEngine;
using UnityEngine.EventSystems;

public class SubmitButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Managers.TurnManager.ChangeTurn(TurnStateFactory.GetState(Define.TurnStateType.EvaluateTurn));
    }
}
