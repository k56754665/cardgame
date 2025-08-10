using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class ExpressionButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ITooltip
{
    string _expressionString;
    public OperatorCard OperatorCardData => _operatorCardData;
    OperatorCard _operatorCardData;

    void Start()
    {
        _expressionString = GetComponentInChildren<TMP_Text>().text;
        OperatorType type = _expressionString switch
        {
            "+" => OperatorType.Add,
            "-" => OperatorType.Subtract,
            "×" => OperatorType.Multiply,
            "÷" => OperatorType.Divide,
            "(" => OperatorType.LeftParenthesis,
            ")" => OperatorType.RightParenthesis,
            _ => OperatorType.Add,
        };

        int score = (type == OperatorType.LeftParenthesis || type == OperatorType.RightParenthesis)
            ? 1
            : Managers.RoundManager.GetOperatorScore(type);

        _operatorCardData = new OperatorCard
        {
            type = type,
            symbol = _expressionString,
            score = score,
        };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Managers.RoundManager.TotalText == Managers.RoundManager.Expression.Length)
            return;

        _operatorCardData.score = Managers.RoundManager.GetOperatorScore(_operatorCardData.type);
        Managers.RoundManager.AddOperatorCard(_operatorCardData);
    }

    public void ShowTooltip()
    {
        int score = Managers.RoundManager.GetOperatorScore(_operatorCardData.type);
        Managers.RoundManager.ShowTooltipEvent((RectTransform)transform, $"점수\n×{score}");
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
