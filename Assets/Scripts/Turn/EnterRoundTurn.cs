using System.Linq;
using UnityEngine;

public class EnterRoundTurn : ITurnState
{
    public void EnterState() 
    {
        Debug.Log("[EnterRoundTurn] EnterState");

        Managers.RoundManager.ClearExpression();
        Managers.RoundManager.SetRandomGoalNum();
        Managers.RoundManager.SetGoalPoint();

        // 기존 핸드를 비우고 필요한 만큼만 드로우하여 핸드 크기가 HandSize를 넘지 않도록 한다.
        GameObject.FindAnyObjectByType<UI_HandCard>().DestroyImmediately();
        Managers.DeckManager.DrawToHand(Managers.RoundManager.HandSize);
    }

    public void UpdateState()
    { 

    }

    public void ExitState()
    { 

    }
}
