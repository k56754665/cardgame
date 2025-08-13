using TMPro;
using UnityEngine;

public class GoalScorePanel : MonoBehaviour
{
    TMP_Text _text;

    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
        Managers.RoundManager.OnChangeGoalPointEvent += ChangeGoalPoint;
    }

    private void ChangeGoalPoint(int goalPoint)
    {
        _text.text = $"목표 IQ\n{goalPoint}";
    }

    private void OnDestroy()
    {
        Managers.RoundManager.OnChangeGoalPointEvent -= ChangeGoalPoint;
    }
}
