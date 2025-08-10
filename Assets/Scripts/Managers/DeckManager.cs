using UnityEngine;
using System.Collections.Generic;
using System;

public class DeckManager
{
    public event Action OnHandChangeAction;
    public event Action<Card, int> OnCardDrawn;
    public event Action<Card, int> OnCardDiscarded;

    List<Card> _hand = new();
    List<Card> _deck = new ();
    List<Card> _discard = new();
    System.Random _rng = new();

    public IReadOnlyList<Card> Hand => _hand;
    public int DeckCount => _deck.Count;
    public int DiscardCount => _discard.Count;
    public int HandCount => _hand.Count;

    public void Init()
    {
        MakeDeck();
    }

    private void MakeDeck()
    {
        _hand.Clear();
        _deck.Clear();
        _discard.Clear();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j < 10; j++)
            {
                _deck.Add(MakeCard(j));
            }
        }

        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = _deck.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(i + 1);
            (_deck[i], _deck[j]) = (_deck[j], _deck[i]);
        }
    }

    // 드로우(1장) -> 핸드로
    public Card DrawOneToHand()
    {
        if (_deck.Count == 0) return null; // 그래도 없으면 종료
        Card c = _deck[^1];
        _deck.RemoveAt(_deck.Count - 1);
        _hand.Add(c);
        OnCardDrawn?.Invoke(c, _hand.Count - 1);
        return c;
    }

    // 드로우(N장) -> 핸드로
    public void DrawToHand(int count)
    {
        int newCount = count;
        if (DeckCount < count)
        {
            newCount = DeckCount;
        }

        List<Card> drawn = new List<Card>(count);
        for (int i = 0; i < count; i++)
        {
            Card c = DrawOneToHand();
            if (c == null) break;
            drawn.Add(c);
        }

        if (DeckCount == 0)
        {
            RecycleIfNeeded(); // 덱 없으면 섞기
        }

        OnHandChangeAction?.Invoke();
    }

    // 버리기 — 인덱스로 지정해서 버리기
    public List<Card> DiscardFromHandByIndices(IEnumerable<int> indices)
    {
        List<int> idx = new List<int>(indices);
        idx.Sort((a, b) => b.CompareTo(a)); // 뒤에서부터 제거
        List<Card> removed = new List<Card>();
        foreach (int i in idx)
        {
            if (i >= _hand.Count) continue;
            Card c = _hand[i];
            _hand.RemoveAt(i);
            _discard.Add(c);
            removed.Add(c);
            OnCardDiscarded?.Invoke(c, i);
        }
        OnHandChangeAction?.Invoke();
        return removed;
    }

    public void DrawToHandRightToLeft(int count)
    {
        int newCount = count;
        if (DeckCount < count)
        {
            newCount = DeckCount;
        }

        List<Card> drawn = new List<Card>(newCount);
        for (int i = 0; i < newCount; i++)
        {
            if (_deck.Count == 0) break;
            Card c = _deck[^1];
            _deck.RemoveAt(_deck.Count - 1);
            drawn.Add(c);
        }

        for (int i = drawn.Count - 1; i >= 0; i--)
        {
            _hand.Add(drawn[i]);
            OnCardDrawn?.Invoke(drawn[i], _hand.Count - 1);
        }

        if (DeckCount == 0)
        {
            RecycleIfNeeded(); // 덱 없으면 섞기
        }

        OnHandChangeAction?.Invoke();
    }


    // 더 이상 드로우할 게 없을 때: 버림을 덱으로 옮기고 셔플
    public void RecycleIfNeeded()
    {
        if (_deck.Count > 0) return;
        if (_discard.Count == 0) return;
        _deck.AddRange(_discard);
        _discard.Clear();
        Shuffle();
    }

    private Card MakeCard(int cardNum, int cardScore = -1)
    {
        int newScore = cardScore;

        if (newScore == -1) // 점수를 입력하지 않으면 카드와 동일한 점수
        {
            newScore = cardNum;
        }
        else if (newScore == 0) // 0 카드는 10점입니다.
        {
            newScore = 10;
        }

        Card newCard = new Card
        {
            number = cardNum,
            score = newScore,
        };

        return newCard;
    }

    public void PrintAllCards()
    {
        string handStr = string.Join(", ", _hand.ConvertAll(c => c.number.ToString()));
        string deckStr = string.Join(", ", _deck.ConvertAll(c => c.number.ToString()));
        string discardStr = string.Join(", ", _discard.ConvertAll(c => c.number.ToString()));

        Debug.Log($"[Hand] ({_hand.Count}) : {handStr}");
        Debug.Log($"[Deck] ({_deck.Count}) : {deckStr}");
        Debug.Log($"[Discard] ({_discard.Count}) : {discardStr}");
    }

}
