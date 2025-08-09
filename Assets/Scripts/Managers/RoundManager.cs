using System;
using System.Data;
using System.Globalization;
using System.Numerics;
using UnityEngine;

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

    public void SetRandomGoalNum()
    {
        // TODO : ºÐ¼öµµ ¸¸µé¾î¾ßÇØ¿ä
        _goalNum = UnityEngine.Random.Range(0, 100);
        OnGoalNumIntegerEvent?.Invoke(_goalNum);
    }

    public void SetGoalPoint()
    {
        _goalPoint = (int)(_goalPoint * 1.2f);
        OnChangeGoalPointEvent?.Invoke(_goalPoint);
    }

    public void AddExpression(string s)
    {
        _expression += s;
        OnExpressionChangeEvent?.Invoke();
    }

    public void ClearExpression()
    {
        _expression = "";
        OnExpressionClearEvent?.Invoke();
    }

    public void CalculateExpression()
    {
        Debug.Log($"_expression: {_expression}");
        try
        {
            string newExpression = _expression
                .Replace("¡¿", "*")  // °ö¼À ±âÈ£
                .Replace("¡À", "/"); // ³ª´°¼À ±âÈ£

            DataTable table = new DataTable();
            object raw = table.Compute(newExpression, "");

            // DataTable.Compute´Â º¸Åë double·Î ¹ÝÈ¯µÊ
            double value = Convert.ToDouble(raw, CultureInfo.InvariantCulture);

            if (IsInteger(value))
            {
                long intValue = (long)Math.Round(value);
                OnExpressionIntegerEvent?.Invoke(intValue);
            }
            else
            {
                Fraction frac = Fraction.FromDouble(value, maxDenominator: 100000, epsilon: 1e-12);

                OnExpressionFractionEvent?.Invoke(frac.Num, frac.Den);
            }
        }
        catch (Exception e)
        {
            ClearExpression();
            Debug.LogError($"Expression calculation failed: {e.Message}");
        }
    }

    public void ShowTooltipEvent(RectTransform rect, string description)
    {
        OnShowTooltipEvent?.Invoke(rect, description);
    }

    public void HideTooltipEvent()
    {
        OnHideTooltipEvent?.Invoke();
    }

    private bool IsInteger(double x, double epsilon = 1e-12)
    {
        return Math.Abs(x - Math.Round(x)) < epsilon;
    }
}