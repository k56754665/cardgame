using UnityEngine;

public class EnterRoundTurn : ITurnState
{
    public void EnterState() 
    {
        Managers.RoundManager.SetRandomGoalNum();
        Managers.DeckManager.DrawToHand(Managers.RoundManager.HandSize);
        //Managers.TurnManager.ChangeTurn();
    }

    public void UpdateState()
    { 

    }

    public void ExitState()
    { 

    }
}
