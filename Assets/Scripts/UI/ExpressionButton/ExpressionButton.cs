using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExpressionButton : MonoBehaviour, IPointerClickHandler
{
    string _expressionString;

    void Start()
    {
        _expressionString = GetComponentInChildren<TMP_Text>().text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Managers.RoundManager.AddExpression(_expressionString);
    }
}
