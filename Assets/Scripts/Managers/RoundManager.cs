using System;
using System.Data;
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
            string newExpression = _expression;

            newExpression = newExpression
                .Replace("¡¿", "*")  // '¡¿'¸¦ ³ª´°¼À ±âÈ£·Î º¯È¯
                .Replace("¡À", "/"); // '¡À'¸¦ ³ª´°¼À ±âÈ£·Î º¯È¯

            DataTable table = new DataTable();
            object value = table.Compute(newExpression, "");
            _expression = Convert.ToString(value);

            OnExpressionChangeEvent?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Expression calculation failed: {e.Message}");
        }
    }
}