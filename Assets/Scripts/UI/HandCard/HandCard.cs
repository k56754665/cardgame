using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandCard : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Card CardData => _cardData;
    Card _cardData;
    TMP_Text _text;
    Toggle _toggle;

    [SerializeField] DOTweenAnimation _cardUpTween;
    [SerializeField] DOTweenAnimation _cardColorTween;
    [SerializeField] DOTweenAnimation _cardShakeTween;

    void Start()
    {
        _toggle = GetComponent<Toggle>();
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void SetCard(Card card)
    {
        _cardData = card;
        _text.text = $"{card.number}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_toggle.isOn)
        {
            _cardColorTween?.DOPlayForward();
        }
        else
        {
            _cardColorTween?.DOPlayBackwards();
            _cardUpTween?.DOPlayBackwards();
        }
        _cardShakeTween?.DORestart();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _cardUpTween?.DOPlayForward();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_toggle != null && _toggle.isOn) return;

        _cardUpTween?.DOPlayForward();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_toggle != null && _toggle.isOn)
        {
            _cardUpTween?.DOPlayForward();
            return;
        }

        _cardUpTween?.DOPlayBackwards();
    }
}
