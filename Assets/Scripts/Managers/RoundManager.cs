using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Numerics;
using UnityEngine;
using static Define;

public class RoundManager
{
    public long GoalNum => _goalNum;
    private long _goalNum;

    public int GoalPoint => _goalPoint;
    private int _goalPoint = 50;

    public int HandSize => _handSize;
    private int _handSize = 8;

    public string Expression => _expression;
    private string _expression = "";

    public int TotalText => _totalText;
    private int _totalText = 10;

    public int SubmitChance
    {
        get => _submitChance;
        set
        {
            _submitChance = value;
            OnChangeSubmitChanceEvent?.Invoke(_submitChance);
        }
    }
    private int _submitChance = 5;

    public int Score => _score;
    private int _score;

    private enum ExpressionCardType { Number, Operator }

    private struct ExpressionCard
    {
        public ExpressionCardType Type;
        public int Score;
    }

    List<ExpressionCard> _expressionCards = new();
    List<int> _selectedHandIndices = new();

    Dictionary<OperatorType, int> _operatorScores = new()
    {
        { OperatorType.Add, 2 },
        { OperatorType.Subtract, 2 },
        { OperatorType.Multiply, 2 },
        { OperatorType.Divide, 2 },
    };

    public event Action OnExpressionChangeEvent;
    public event Action OnExpressionClearEvent;
    public event Action<long> OnExpressionIntegerEvent;
    public event Action<BigInteger, BigInteger> OnExpressionFractionEvent;

    public event Action<int> OnChangeSubmitChanceEvent;

    public event Action<RectTransform, string> OnShowTooltipEvent;
    public event Action OnHideTooltipEvent;

    public event Action<long> OnGoalNumIntegerEvent;
    public event Action<BigInteger, BigInteger> OnGoalNumFractionEvent;

    public event Action<int> OnChangeGoalPointEvent;
    public event Action<int> OnChangeScoreEvent;

    public void SetRandomGoalNum()
    {
        // TODO : 분수도 만들어야해요
        _goalNum = UnityEngine.Random.Range(0, 100);
        OnGoalNumIntegerEvent?.Invoke(_goalNum);
    }

    public void SetGoalPoint()
    {
        _goalPoint = (int)(_goalPoint * 1.2f);
        OnChangeGoalPointEvent?.Invoke(_goalPoint);
    }

    public void AddNumberCard(Card card, int handIndex)
    {
        _expressionCards.Add(new ExpressionCard { Type = ExpressionCardType.Number, Score = card.score });
        _expression += card.number.ToString();
        _selectedHandIndices.Add(handIndex);
        OnExpressionChangeEvent?.Invoke();
    }


    public void AddOperatorCard(OperatorCard card)
    {
        _expressionCards.Add(new ExpressionCard { Type = ExpressionCardType.Operator, Score = card.score });
        _expression += card.symbol;
        OnExpressionChangeEvent?.Invoke();
    }

    public void DiscardAndDrawSelectedCards()
    {
        if (_selectedHandIndices.Count > 0)
        {
            Managers.DeckManager.DiscardFromHandByIndices(_selectedHandIndices);
            Managers.DeckManager.DrawToHandRightToLeft(_selectedHandIndices.Count);
        }
        ClearExpression();
    }


    public void ClearExpression()
    {
        _expression = "";
        _expressionCards.Clear();
        _selectedHandIndices.Clear();
        OnExpressionClearEvent?.Invoke();
    }

    public bool CalculateExpression()
    {
        Debug.Log($"_expression: {_expression}");
        try
        {
            string newExpression = _expression
                .Replace("×", "*")  // 곱셈 기호
                .Replace("÷", "/"); // 나눗셈 기호

            DataTable table = new DataTable();
            object raw = table.Compute(newExpression, "");

            // DataTable.Compute는 보통 double로 반환됨
            double value = Convert.ToDouble(raw, CultureInfo.InvariantCulture);

            if (IsInteger(value))
            {
                long intValue = (long)Math.Round(value);
                OnExpressionIntegerEvent?.Invoke(intValue);

                if (intValue == _goalNum)
                {
                    CalculateScore();
                    return true;
                }
            }
            else
            {
                Fraction frac = Fraction.FromDouble(value, maxDenominator: 100000, epsilon: 1e-12);

                OnExpressionFractionEvent?.Invoke(frac.Num, frac.Den);

                // TODO : 분수일때 정답과 같은지 검사해요
            }
        }
        catch (Exception e)
        {
            ClearExpression();
            Debug.LogError($"Expression calculation failed: {e.Message}");
            return false;
        }
        return false;
    }

    public int CalculateScore()
    {
        int total = 0;
        foreach (var card in _expressionCards)
        {
            if (card.Type == ExpressionCardType.Number)
                total += card.Score;
            else
                total *= card.Score;
        }
        _score += total;
        OnChangeScoreEvent?.Invoke(_score);
        return total;
    }


    public void ShowTooltipEvent(RectTransform rect, string description)
    {
        OnShowTooltipEvent?.Invoke(rect, description);
    }

    public void HideTooltipEvent()
    {
        OnHideTooltipEvent?.Invoke();
    }

    public bool TryGetExpressionScore(out int score)
    {
        score = 0;
        if (_expressionCards.Count == 0)
            return false;

        if (_expressionCards[_expressionCards.Count - 1].Type == ExpressionCardType.Operator)
            return false;

        int total = 0;
        foreach (var card in _expressionCards)
        {
            if (card.Type == ExpressionCardType.Number)
                total += card.Score;
            else
                total *= card.Score;
        }

        score = total;
        return true;
    }

    private bool IsInteger(double x, double epsilon = 1e-12)
    {
        return Math.Abs(x - Math.Round(x)) < epsilon;
    }

    public int GetOperatorScore(OperatorType type)
    {
        return _operatorScores[type];
    }
}