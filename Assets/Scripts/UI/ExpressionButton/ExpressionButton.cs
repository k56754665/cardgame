using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExpressionButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ITooltip
{
    string _expressionString;
    public OperatorCard OperatorCardData => _operatorCardData;
    OperatorCard _operatorCardData;

    void Start()
    {
        _expressionString = GetComponentInChildren<TMP_Text>().text;
        _operatorCardData = new OperatorCard
        {
            symbol = _expressionString,
            score = 2,
        };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Managers.RoundManager.TotalText == Managers.RoundManager.Expression.Length)
            return;

        Managers.RoundManager.AddOperatorCard(_operatorCardData);
    }

    public void ShowTooltip()
    {
        Managers.RoundManager.ShowTooltipEvent((RectTransform)transform, $"점수\n×{_operatorCardData.score}");
    }

    public void HideTooltip()
    {
        Managers.RoundManager.HideTooltipEvent();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }
}
