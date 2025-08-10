using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_HandCard : MonoBehaviour
{
    Transform _handCardRoot;
    GameObject _handCardPrefab;
    readonly List<GameObject> _handCards = new();
    readonly Queue<(Card card, int index)> _pendingDraws = new();
    int _discardAnimations;
    bool _isShifting;
    bool _isProcessingDraws;

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
        _pendingDraws.Enqueue((card, index));
        if (_discardAnimations == 0 && !_isShifting)
        {
            ProcessDrawQueue();
        }
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
        _discardAnimations++;
        rt.DOAnchorPos(target, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(go);
            _discardAnimations--;
            if (_discardAnimations == 0)
            {
                UpdateIndices();
                ShiftCardsLeft();
            }
        });
    }

    void ShiftCardsLeft()
    {
        if (_handCards.Count == 0)
        {
            _isShifting = false;
            ProcessDrawQueue();
            return;
        }

        _isShifting = true;
        RectTransform handRect = _handCardRoot as RectTransform;

        List<RectTransform> rects = new();
        List<Vector2> startPos = new();
        foreach (var cardGO in _handCards)
        {
            RectTransform rt = cardGO.GetComponent<RectTransform>();
            rects.Add(rt);
            startPos.Add(rt.anchoredPosition);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(handRect);
        List<Vector2> targetPos = new();
        foreach (var rt in rects)
        {
            targetPos.Add(rt.anchoredPosition);
        }

        for (int i = 0; i < rects.Count; i++)
        {
            LayoutElement le = rects[i].GetComponent<LayoutElement>();
            if (le == null) le = rects[i].gameObject.AddComponent<LayoutElement>();
            le.ignoreLayout = true;
            rects[i].anchoredPosition = startPos[i];
        }

        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < rects.Count; i++)
        {
            seq.Append(rects[i].DOAnchorPos(targetPos[i], 0.2f).SetEase(Ease.OutQuad));
        }
        seq.OnComplete(() =>
        {
            for (int i = 0; i < rects.Count; i++)
            {
                LayoutElement le = rects[i].GetComponent<LayoutElement>();
                if (le != null) le.ignoreLayout = false;
                rects[i].anchoredPosition = targetPos[i];
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(handRect);
            _isShifting = false;
            ProcessDrawQueue();
        });
    }

    void ProcessDrawQueue()
    {
        if (_pendingDraws.Count == 0 || _isProcessingDraws)
            return;

        _isProcessingDraws = true;
        Sequence seq = DOTween.Sequence();

        while (_pendingDraws.Count > 0)
        {
            var (card, index) = _pendingDraws.Dequeue();
            GameObject go = Instantiate(_handCardPrefab, _handCardRoot);
            go.transform.SetSiblingIndex(index);
            HandCard handCard = go.GetComponentInChildren<HandCard>();
            handCard.SetCard(card, index);
            _handCards.Insert(index, go);
            UpdateIndices();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_handCardRoot as RectTransform);
            RectTransform cardRect = go.transform.GetChild(0) as RectTransform;
            cardRect.anchoredPosition = new Vector2(200f, 0f);
            seq.Append(cardRect.DOAnchorPos(Vector2.zero, 0.3f).SetEase(Ease.OutBack));
        }

        seq.OnComplete(() =>
        {
            _isProcessingDraws = false;
        });
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