using UnityEngine;
using UnityEngine.EventSystems;

public class SubmitButton : MonoBehaviour, IPointerClickHandler
{
    void Start()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Managers.RoundManager.CalculateExpression();
    }
}
