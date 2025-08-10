using System.Linq;
using UnityEngine;

public class EnterRoundTurn : ITurnState
{
    public void EnterState() 
    {
        Managers.RoundManager.ClearExpression();
        Managers.RoundManager.SetRandomGoalNum();
        Managers.RoundManager.SetGoalPoint();
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
