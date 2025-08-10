using System.Linq;
using UnityEngine;

public class EnterRoundTurn : ITurnState
{
    public void EnterState() 
    {
        Managers.RoundManager.ClearExpression();
        Managers.RoundManager.SetRandomGoalNum();
        Managers.RoundManager.SetGoalPoint();

        // 기존 핸드를 비우고 필요한 만큼만 드로우하여 핸드 크기가 HandSize를 넘지 않도록 한다.
        Managers.DeckManager.DiscardAllFromHand();
        int need = Managers.RoundManager.HandSize - Managers.DeckManager.HandCount;
        if (need > 0)
        {
            Managers.DeckManager.DrawToHand(need);
        }
        Debug.Assert(Managers.DeckManager.HandCount <= Managers.RoundManager.HandSize);
    }

    public void UpdateState()
    { 

    }

    public void ExitState()
    { 

    }
}
