using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_HandCard : MonoBehaviour
{
    RectTransform _handCardRoot;                 // 루트는 반드시 RectTransform로 받기
    HorizontalLayoutGroup _layoutGroup;          // 실제 LayoutGroup이 붙은 곳
    GameObject _handCardPrefab;

    readonly List<GameObject> _handCards = new();
    readonly Queue<(Card card, int index)> _pendingDraws = new();

    int _discardAnimations;
    bool _animating; // 레이아웃 이동/드로우 애니메이션 중

    void Start()
    {
        _handCardRoot = transform.GetChild(0) as RectTransform;
        _layoutGroup = _handCardRoot.GetComponent<HorizontalLayoutGroup>();
        if (_layoutGroup == null)
            _layoutGroup = _handCardRoot.GetComponentInChildren<HorizontalLayoutGroup>(true);

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
        foreach (var go in _handCards) Destroy(go);
        _handCards.Clear();

        for (int i = 0; i < Managers.DeckManager.Hand.Count; i++)
        {
            Card card = Managers.DeckManager.Hand[i];
            GameObject go = Instantiate(_handCardPrefab, _handCardRoot);
            go.GetComponentInChildren<HandCard>().SetCard(card, i);
            _handCards.Add(go);
        }

        ForceRebuild();
    }

    void HandleCardDrawn(Card card, int index)
    {
        _pendingDraws.Enqueue((card, index));
        TryProcessDraws();
    }

    void HandleCardDiscarded(Card card, int index)
    {
        if (index < 0 || index >= _handCards.Count) return;

        GameObject go = _handCards[index];
        _handCards.RemoveAt(index);

        // 아래로 빠지는 애니메이션(슬롯 자체)
        var slotRect = go.GetComponent<RectTransform>();
        var le = slotRect.GetComponent<LayoutElement>() ?? slotRect.gameObject.AddComponent<LayoutElement>();
        le.ignoreLayout = true;

        _discardAnimations++;
        slotRect.DOAnchorPos(slotRect.anchoredPosition + new Vector2(0f, -200f), 0.25f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    Destroy(go);
                    _discardAnimations--;
                    UpdateIndices();
                    AnimateToCurrentLayout(moveDuration: 0.22f, ease: Ease.OutCubic);
                });
    }

    void TryProcessDraws()
    {
        if (_animating || _discardAnimations > 0) return;
        if (_pendingDraws.Count == 0) return;

        // 한 번에 쌓인 드로우 전부 적용하고, 한 방에 애니메이션 (더 부드러움)
        List<GameObject> newCards = new();

        while (_pendingDraws.Count > 0)
        {
            var (card, index) = _pendingDraws.Dequeue();

            GameObject go = Instantiate(_handCardPrefab, _handCardRoot);
            go.transform.SetSiblingIndex(Mathf.Clamp(index, 0, _handCards.Count));

            HandCard handCard = go.GetComponentInChildren<HandCard>();
            handCard.SetCard(card, index);

            _handCards.Insert(Mathf.Clamp(index, 0, _handCards.Count), go);
            newCards.Add(go);
        }

        UpdateIndices();

        // 새 카드들의 내부는 오른쪽에서 슬라이드 인
        AnimateToCurrentLayout(
            moveDuration: 0.24f,
            ease: Ease.OutCubic,
            newCardsSlideIn: newCards
        );
    }

    void UpdateIndices()
    {
        for (int i = 0; i < _handCards.Count; i++)
        {
            _handCards[i].transform.SetSiblingIndex(i);
            _handCards[i].GetComponentInChildren<HandCard>().SetHandIndex(i);
        }
    }

    void ForceRebuild()
    {
        if (_layoutGroup != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup.GetComponent<RectTransform>());
        else
            LayoutRebuilder.ForceRebuildLayoutImmediate(_handCardRoot);
    }

    /// <summary>
    /// 현재 레이아웃 기준의 "목표 위치"로 모든 슬롯을 동시에 보간한다.
    /// newCardsSlideIn이 있으면 그 카드의 내부(자식 0)는 오른쪽에서 슬라이드 인.
    /// </summary>
    void AnimateToCurrentLayout(float moveDuration, Ease ease, List<GameObject> newCardsSlideIn = null)
    {
        if (_handCards.Count == 0)
        {
            _animating = false;
            TryProcessDraws();
            return;
        }

        _animating = true;

        // 1) 현재(시작) 위치 스냅샷
        List<RectTransform> rects = new();
        List<Vector2> startPos = new();
        foreach (var go in _handCards)
        {
            var rt = go.GetComponent<RectTransform>();
            rects.Add(rt);
            startPos.Add(rt.anchoredPosition);
        }

        // 2) 레이아웃을 정상 모드(ignore=false)로 두고 "목표 위치" 계산
        foreach (var rt in rects)
        {
            var le = rt.GetComponent<LayoutElement>() ?? rt.gameObject.AddComponent<LayoutElement>();
            le.ignoreLayout = false;
        }
        ForceRebuild();

        List<Vector2> targetPos = new();
        foreach (var rt in rects)
            targetPos.Add(rt.anchoredPosition);

        // 3) 다시 레이아웃 영향 차단 + 화면상 시작점으로 되돌림(스냅 방지)
        foreach (var rt in rects)
        {
            var le = rt.GetComponent<LayoutElement>();
            le.ignoreLayout = true;
        }
        for (int i = 0; i < rects.Count; i++)
            rects[i].anchoredPosition = startPos[i];

        // 4) 시퀀스 구성: 모든 슬롯을 동시에 target으로 보간
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < rects.Count; i++)
            seq.Join(rects[i].DOAnchorPos(targetPos[i], moveDuration).SetEase(ease));

        // 5) 새 카드 내부 비주얼 슬라이드 인(있을 경우)
        if (newCardsSlideIn != null)
        {
            foreach (var go in newCardsSlideIn)
            {
                if (go == null) continue;
                // 내부 비주얼(자식 0) 기준으로 슬라이드 (프리팹 구조에 맞춰 조정)
                var inner = go.transform.GetChild(0) as RectTransform;
                if (inner != null)
                {
                    inner.anchoredPosition = new Vector2(500f, 0f);
                    seq.Join(inner.DOAnchorPos(Vector2.zero, moveDuration + 0.04f).SetEase(Ease.OutBack));
                }
            }
        }

        // 6) 완료 처리: 레이아웃 복구 + 후속 드로우 처리
        seq.OnComplete(() =>
        {
            foreach (var rt in rects)
            {
                var le = rt.GetComponent<LayoutElement>();
                if (le != null) le.ignoreLayout = false;
            }
            ForceRebuild();

            _animating = false;
            TryProcessDraws();
        });
    }
}
