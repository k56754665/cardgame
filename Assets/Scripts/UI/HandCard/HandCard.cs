using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ITooltip
{
    public Card CardData => _cardData;
    Card _cardData;
    [SerializeField] TMP_Text _text;

    [SerializeField] DOTweenAnimation _cardUpTween;
    [SerializeField] DOTweenAnimation _cardColorTween;
    [SerializeField] DOTweenAnimation _cardShakeTween;

    bool _isSelected;

    private void Start()
    {
        Managers.RoundManager.OnExpressionClearEvent += DeselectCard;
    }

    private void OnDestroy()
    {
        Managers.RoundManager.OnExpressionClearEvent -= DeselectCard;
    }

    public void SetCard(Card card)
    {
        _cardData = card;
        _text.text = $"{card.number}";
    }

    private void CardUp()
    {
        _cardColorTween?.DOPlayForward();
        _cardUpTween?.DOPlayForward();
    }

    private void CardDown()
    {
        _cardColorTween?.DOPlayBackwards();
        _cardUpTween?.DOPlayBackwards();
    }

    private void DeselectCard()
    {
        _isSelected = false;

        CardDown();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Managers.RoundManager.TotalText == Managers.RoundManager.Expression.Length)
            return;

        if (_isSelected)
            return;

        _isSelected = true;
        CardUp();
        _cardShakeTween?.DORestart();
        Managers.RoundManager.AddExpression(_cardData.number.ToString());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();

        if (Managers.RoundManager.TotalText == Managers.RoundManager.Expression.Length)
            return;

        if (_isSelected)
            return;

        _cardUpTween?.DOPlayForward();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();

        if (Managers.RoundManager.TotalText == Managers.RoundManager.Expression.Length)
            return;

        if (_isSelected)
            return;

        _cardUpTween?.DOPlayBackwards();
    }

    public void ShowTooltip()
    {
        RectTransform rect = GetComponent<RectTransform>();
        string description = $"Á¡¼ö\n{_cardData.score}";
        Managers.RoundManager.ShowTooltipEvent(rect, description);
    }

    public void HideTooltip()
    {
        Managers.RoundManager.HideTooltipEvent();
    }
}
