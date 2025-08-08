using System;
using UnityEngine;

public class RoundManager
{
    public int GoalNum => _goalNum;
    private int _goalNum;

    public int HandSize => _handSize;
    private int _handSize = 8;

    public event Action<int> OnGoalNumChangeEvent;

    public void SetRandomGoalNum()
    {
        _goalNum = UnityEngine.Random.Range(0, 100);
        OnGoalNumChangeEvent?.Invoke(_goalNum);
    }
}