using UnityEngine;

public class EnterRoundTurn : ITurnState
{
    public void EnterState() 
    {
        Managers.RoundManager.SetRandomGoalNum();
        //Managers.TurnManager.ChangeTurn();
    }

    public void UpdateState()
    { 

    }

    public void ExitState()
    { 

    }
}
