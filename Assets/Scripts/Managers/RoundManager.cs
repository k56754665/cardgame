using System;
using System.Data;
using System.Globalization;
using System.Numerics;
using UnityEngine;

public class RoundManager
{
    public int GoalNum => _goalNum;
    private int _goalNum;

    public int HandSize => _handSize;
    private int _handSize = 8;

    public string Expression => _expression;
    private string _expression = "";

    public int TotalText => _totalText;
    private int _totalText = 10;

    public event Action<int> OnGoalNumChangeEvent;
    public event Action OnExpressionChangeEvent;
    public event Action OnExpressionClearEvent;
    public event Action<long> OnExpressionIntegerEvent;
    public event Action<BigInteger, BigInteger> OnExpressionFractionEvent;

    public void SetRandomGoalNum()
    {
        _goalNum = UnityEngine.Random.Range(0, 100);
        OnGoalNumChangeEvent?.Invoke(_goalNum);
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

    private bool IsInteger(double x, double epsilon = 1e-12)
    {
        return Math.Abs(x - Math.Round(x)) < epsilon;
    }
}