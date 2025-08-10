using UnityEngine;
using UnityEngine.SceneManagement;

public class EvaluateTurn : ITurnState
{
    public void EnterState()
    {
        bool isAnswer = false;
        isAnswer = Managers.RoundManager.CalculateExpression();
        Managers.RoundManager.DiscardAndDrawSelectedCards();
        if (isAnswer)
        {
            if (Managers.RoundManager.Score >= Managers.RoundManager.GoalPoint)
            {
                Managers.DeckManager.DiscardAllFromHand();

                // 계산기 비우기
                Managers.RoundManager.ClearExpression();

                // 점수가 목표 점수보다 크면
                // 상점 턴으로 가기
                Managers.TurnManager.ChangeTurn(TurnStateFactory.GetState(Define.TurnStateType.ShopTurn));
            }
            else
            {
                // 점수가 목표 점수보다 작으면
                Managers.RoundManager.SetRandomGoalNum();
                // 계산기 비우기
                Managers.RoundManager.ClearExpression();
                // 플레이 턴으로 가기
                Managers.TurnManager.ChangeTurn(TurnStateFactory.GetState(Define.TurnStateType.PlayRoundTurn));
            }   
        }
        else
        {
            // 틀렸으면 제출 기회 -1
            Managers.RoundManager.SubmitChance--;

            // 계산기 비우기
            Managers.RoundManager.ClearExpression();

            // 제출한 손패 버리고 새로 드로우
            if (Managers.RoundManager.SubmitChance > 0)
            {
                // 플레이 턴으로 가기
                Managers.TurnManager.ChangeTurn(TurnStateFactory.GetState(Define.TurnStateType.PlayRoundTurn));
            }
            else
            {
                // 게임오버
                Managers.DeckManager.DiscardAllFromHand();

                Managers.GameManager.GameOver();
            }
        }
    }

    public void ExitState()
    {

    }

    public void UpdateState()
    {

    }
}
