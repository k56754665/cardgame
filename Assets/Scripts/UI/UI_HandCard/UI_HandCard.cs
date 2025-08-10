using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_HandCard : MonoBehaviour
{
    Transform _handCardRoot;
    GameObject _handCardPrefab;
    readonly List<GameObject> _handCards = new();

    void Start()
    {
        _handCardRoot = transform.GetChild(0);
        _handCardPrefab = Resources.Load<GameObject>("HandCardSlot/HandCardSlot");

        MakeHandCards();
        Managers.DeckManager.OnCardDrawn += HandleCardDrawn;
        Managers.DeckManager.OnCardDiscarded += HandleCardDiscarded;
    }

    void OnDestroy()
    {
        Managers.DeckManager.OnCardDrawn -= HandleCardDrawn;
        Managers.DeckManager.OnCardDiscarded -= HandleCardDiscarded;
    }

    void MakeHandCards()
    {
        foreach (var go in _handCards)
        {
            Destroy(go);
        }
        _handCards.Clear();

        for (int i = 0; i < Managers.DeckManager.Hand.Count; i++)
        {
            Card card = Managers.DeckManager.Hand[i];
            GameObject go = Instantiate(_handCardPrefab, _handCardRoot);
            go.GetComponentInChildren<HandCard>().SetCard(card, i);
            _handCards.Add(go);
        }
    }

    void HandleCardDrawn(Card card, int index)
    {
        GameObject go = Instantiate(_handCardPrefab, _handCardRoot);
        go.transform.SetSiblingIndex(index);
        HandCard handCard = go.GetComponentInChildren<HandCard>();
        handCard.SetCard(card, index);
        _handCards.Insert(index, go);
        UpdateIndices();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_handCardRoot as RectTransform);
        RectTransform cardRect = go.transform.GetChild(0) as RectTransform;
        cardRect.anchoredPosition = new Vector2(200f, 0f);
        cardRect.DOAnchorPos(Vector2.zero, 0.3f).SetEase(Ease.OutBack);
    }

    void HandleCardDiscarded(Card card, int index)
    {
        if (index < 0 || index >= _handCards.Count) return;
        GameObject go = _handCards[index];
        _handCards.RemoveAt(index);
        LayoutElement le = go.GetComponent<LayoutElement>();
        if (le == null) le = go.AddComponent<LayoutElement>();
        le.ignoreLayout = true;
        RectTransform rt = go.GetComponent<RectTransform>();
        Vector2 target = rt.anchoredPosition + new Vector2(0f, -200f);
        rt.DOAnchorPos(target, 0.3f).SetEase(Ease.InBack).OnComplete(() => Destroy(go));
        UpdateIndices();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_handCardRoot as RectTransform);
    }

    void UpdateIndices()
    {
        for (int i = 0; i < _handCards.Count; i++)
        {
            _handCards[i].transform.SetSiblingIndex(i);
            _handCards[i].GetComponentInChildren<HandCard>().SetHandIndex(i);
        }
    }
}