using System;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public int GoalNum => _goalNum;
    private int _goalNum;

    public event Action<int> OnGoalNumChangeEvent;

    public void SetRandomGoalNum()
    {
        _goalNum = UnityEngine.Random.Range(0, 100);
        OnGoalNumChangeEvent?.Invoke(_goalNum);
    }
}