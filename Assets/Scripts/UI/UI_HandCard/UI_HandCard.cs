using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_HandCard : MonoBehaviour
{
    RectTransform _handCardRoot;                 // 슬롯들이 붙는 부모(레이아웃 대상)
    HorizontalLayoutGroup _layoutGroup;          // 실제 HorizontalLayoutGroup
    GameObject _handCardPrefab;

    readonly List<GameObject> _handCards = new();                      // 슬롯 GO 목록(각 슬롯의 자식[0]이 HandCard)
    readonly Queue<(Card card, int index)> _pendingDraws = new();      // 대기 드로우

    int _discardAnimations;
    bool _animating;

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
            GameObject slot = Instantiate(_handCardPrefab, _handCardRoot);
            slot.GetComponentInChildren<HandCard>().SetCard(card, i);
            _handCards.Add(slot);
        }

        ForceRebuild();
    }

    void HandleCardDrawn(Card card, int index)
    {
        _pendingDraws.Enqueue((card, index));
        TryProcessDraws();
    }

    // 슬롯은 레이아웃에 남겨둔 채, 자식 HandCard만 아래로 내린다 -> 간격 즉시 변화 없음
    void HandleCardDiscarded(Card card, int index)
    {
        if (index < 0 || index >= _handCards.Count) return;

        GameObject slotGO = _handCards[index];
        RectTransform slotRect = slotGO.GetComponent<RectTransform>();

        // 슬롯은 레이아웃 포함 상태 유지(간격 유지)
        var le = slotRect.GetComponent<LayoutElement>() ?? slotRect.gameObject.AddComponent<LayoutElement>();
        le.ignoreLayout = false;

        // 내부 비주얼(HandCard)만 내려보내기
        RectTransform inner = slotGO.transform.GetChild(0) as RectTransform;
        if (inner == null)
        {
            // 안전장치: 구조가 다르면 슬롯 자체를 fallback
            inner = slotRect;
        }

        // 남아있는 다른 슬롯들의 “수축 전 시작 좌표” 저장
        var startMap = new Dictionary<RectTransform, Vector2>();
        for (int i = 0; i < _handCards.Count; i++)
        {
            if (i == index) continue;
            var rt = _handCards[i].GetComponent<RectTransform>();
            startMap[rt] = rt.anchoredPosition;
        }

        _discardAnimations++;
        // 필요하면 페이드 추가: cg.DOFade(0f, 0.25f);
        inner.DOAnchorPos(inner.anchoredPosition + new Vector2(0f, -200f), 0.25f)
             .SetEase(Ease.InBack)
             .SetLink(slotGO, LinkBehaviour.KillOnDestroy)
             .OnComplete(() =>
             {
                 try
                 {
                     // 슬롯 제거(이 시점에 레이아웃이 한 칸 줄어듦)
                     _handCards.Remove(slotGO);
                     DOTween.Kill(slotGO, complete: false);
                     Destroy(slotGO);

                     // 제거 전 위치(startMap) -> 제거 후 레이아웃 타겟으로 부드럽게 수축
                     AnimateToCurrentLayoutUsingSavedStarts(0.22f, Ease.OutCubic, startMap);
                 }
                 finally
                 {
                     _discardAnimations--;
                 }
             });
    }

    void TryProcessDraws()
    {
        if (_animating || _discardAnimations > 0) return;
        if (_pendingDraws.Count == 0) return;

        // 몰아서 생성 -> 한 번에 재배치
        List<GameObject> newSlots = new();

        while (_pendingDraws.Count > 0)
        {
            var (card, index) = _pendingDraws.Dequeue();

            GameObject slot = Instantiate(_handCardPrefab, _handCardRoot);
            slot.transform.SetSiblingIndex(Mathf.Clamp(index, 0, _handCards.Count));

            HandCard handCard = slot.GetComponentInChildren<HandCard>();
            handCard.SetCard(card, index);

            _handCards.Insert(Mathf.Clamp(index, 0, _handCards.Count), slot);
            newSlots.Add(slot);
        }

        UpdateIndices();

        // 새 슬롯들은 "오른쪽 바깥"에서 들어오도록 시작점 조정
        AnimateToCurrentLayout(
            moveDuration: 0.24f,
            ease: Ease.OutCubic,
            newCardsSlideIn: newSlots
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
    /// (드로우용) 현재 레이아웃 기준 타겟으로 이동.
    /// newCardsSlideIn: 새 슬롯들은 타겟에서 오른쪽으로 오프셋한 위치를 시작점으로 사용.
    /// </summary>
    void AnimateToCurrentLayout(float moveDuration, Ease ease, List<GameObject> newCardsSlideIn = null)
    {
        _handCards.RemoveAll(go => go == null);
        if (_handCards.Count == 0)
        {
            _animating = false;
            TryProcessDraws();
            return;
        }

        _animating = true;

        // 1) 현재 시작 위치 스냅샷
        List<RectTransform> rects = new();
        List<Vector2> startPos = new();
        foreach (var go in _handCards)
        {
            if (go == null) continue;
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) continue;
            rects.Add(rt);
            startPos.Add(rt.anchoredPosition);
        }

        // 2) 레이아웃 켜고 타겟 위치 산출
        foreach (var rt in rects)
        {
            if (rt == null) continue;
            var le = rt.GetComponent<LayoutElement>() ?? rt.gameObject.AddComponent<LayoutElement>();
            le.ignoreLayout = false;
        }
        ForceRebuild();

        List<Vector2> targetPos = new();
        foreach (var rt in rects)
        {
            if (rt == null) continue;
            targetPos.Add(rt.anchoredPosition);
        }

        // 3) 새 슬롯들은 타겟에서 우측으로 민 위치를 시작점으로 덮어쓰기
        if (newCardsSlideIn != null && newCardsSlideIn.Count > 0)
        {
            var newSet = new HashSet<RectTransform>();
            foreach (var go in newCardsSlideIn)
            {
                var rt = go != null ? go.GetComponent<RectTransform>() : null;
                if (rt != null) newSet.Add(rt);
            }

            float off = Mathf.Max(200f, _handCardRoot.rect.width * 0.6f);
            for (int i = 0; i < rects.Count; i++)
            {
                if (newSet.Contains(rects[i]))
                {
                    startPos[i] = targetPos[i] + new Vector2(off, 0f);
                }
            }
        }

        // 4) 레이아웃 영향 차단 + 시작점 적용
        foreach (var rt in rects)
        {
            if (rt == null) continue;
            var le = rt.GetComponent<LayoutElement>();
            le.ignoreLayout = true;
        }
        for (int i = 0; i < rects.Count; i++)
        {
            if (rects[i] == null) continue;
            rects[i].anchoredPosition = startPos[i];
        }

        // 5) 타겟으로 보간
        Sequence seq = DOTween.Sequence().SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        for (int i = 0; i < rects.Count; i++)
            seq.Join(
                rects[i]
                    .DOAnchorPos(targetPos[i], moveDuration)
                    .SetEase(ease)
                    .SetLink(rects[i].gameObject, LinkBehaviour.KillOnDestroy)
            );

        seq.OnComplete(() =>
        {
            foreach (var rt in rects)
            {
                if (rt == null) continue;
                var le = rt.GetComponent<LayoutElement>();
                if (le != null) le.ignoreLayout = false;
            }
            ForceRebuild();

            _animating = false;
            TryProcessDraws();
        });
    }

    /// <summary>
    /// (버리기용) 제거 전 시작좌표(savedStarts) -> 제거 후 레이아웃 타겟으로 보간.
    /// </summary>
    void AnimateToCurrentLayoutUsingSavedStarts(float moveDuration, Ease ease, Dictionary<RectTransform, Vector2> savedStarts)
    {
        _handCards.RemoveAll(go => go == null);
        if (_handCards.Count == 0)
        {
            _animating = false;
            TryProcessDraws();
            return;
        }

        _animating = true;

        // 남아있는 슬롯들
        List<RectTransform> rects = new();
        foreach (var go in _handCards)
        {
            if (go == null) continue;
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) continue;
            rects.Add(rt);
        }

        // 1) 레이아웃 켜서 목표 위치 산출(슬롯 제거 이후의 타겟)
        foreach (var rt in rects)
        {
            if (rt == null) continue;
            var le = rt.GetComponent<LayoutElement>() ?? rt.gameObject.AddComponent<LayoutElement>();
            le.ignoreLayout = false;
        }
        ForceRebuild();

        List<Vector2> targetPos = new();
        foreach (var rt in rects)
        {
            if (rt == null) continue;
            targetPos.Add(rt.anchoredPosition);
        }

        // 2) 다시 레이아웃 영향 차단 + 제거 전 시작좌표로 복원
        foreach (var rt in rects)
        {
            if (rt == null) continue;
            var le = rt.GetComponent<LayoutElement>();
            le.ignoreLayout = true;
        }
        for (int i = 0; i < rects.Count; i++)
        {
            if (rects[i] == null) continue;
            Vector2 start;
            if (!savedStarts.TryGetValue(rects[i], out start))
                start = rects[i].anchoredPosition;
            rects[i].anchoredPosition = start;
        }

        // 3) 타겟으로 보간
        Sequence seq = DOTween.Sequence().SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        for (int i = 0; i < rects.Count; i++)
            seq.Join(
                rects[i]
                    .DOAnchorPos(targetPos[i], moveDuration)
                    .SetEase(ease)
                    .SetLink(rects[i].gameObject, LinkBehaviour.KillOnDestroy)
            );


        seq.OnComplete(() =>
        {
            foreach (var rt in rects)
            {
                if (rt == null) continue;
                var le = rt.GetComponent<LayoutElement>();
                if (le != null) le.ignoreLayout = false;
            }
            ForceRebuild();

            // 인덱스 갱신(실제 남은 슬롯 기준)
            UpdateIndices();

            _animating = false;
            TryProcessDraws();
        });
    }
}
