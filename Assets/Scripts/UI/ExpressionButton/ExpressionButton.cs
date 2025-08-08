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
        if (Managers.RoundManager.TotalText == Managers.RoundManager.Expression.Length)
            return;

        Managers.RoundManager.AddExpression(_expressionString);
    }
}
