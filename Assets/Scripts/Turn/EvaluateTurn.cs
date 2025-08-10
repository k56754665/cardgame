using UnityEngine;
using UnityEngine.SceneManagement;

public class EvaluateTurn : ITurnState
{
    public void EnterState()
    {
        bool isAnswer = false;
        isAnswer = Managers.RoundManager.CalculateExpression();
        if (isAnswer)
        {
            // 정답을 맞췄다면 점수 계산

            // 점수가 목표 점수보다 작으면
            // 제출한 손패 버리고 새로 드로우
            // 플레이 턴으로 가기

            // 점수가 목표 점수보다 크면
            // 상점 턴으로 가기
        }
        else
        {
            // 틀렸으면 제출 기회 -1
            Managers.RoundManager.SubmitChance--;

            // 제출한 손패 버리고 새로 드로우

            if (Managers.RoundManager.SubmitChance > 0)
            {
                // 플레이 턴으로 가기
                Managers.TurnManager.ChangeTurn(TurnStateFactory.GetState(Define.TurnStateType.PlayRoundTurn));
            }
            else
            {
                // 게임오버

                // 현재 활성화된 씬의 이름을 가져와 다시 로드
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
