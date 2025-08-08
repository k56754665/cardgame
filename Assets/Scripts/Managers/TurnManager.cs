using UnityEngine;

public class TurnManager
{
    public ITurnState CurrentTurn => _currentTurn;
    ITurnState _currentTurn;

    public void Init()
    {
        ChangeTurn(TurnStateFactory.GetState(Define.TurnStateType.EnterRoundTurn));
    }

    public void UpdateCurrentTurn()
    {
        CurrentTurn?.UpdateState();
    }

    public void ChangeTurn(ITurnState nextTurn)
    {
        _currentTurn?.ExitState();
        _currentTurn = nextTurn;
        _currentTurn?.EnterState();
    }
}
