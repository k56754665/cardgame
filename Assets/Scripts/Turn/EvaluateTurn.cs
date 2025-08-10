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
            // ������ ����ٸ� ���� ���

            // ������ ��ǥ �������� ������
            // ������ ���� ������ ���� ��ο�
            // �÷��� ������ ����

            // ������ ��ǥ �������� ũ��
            // ���� ������ ����
        }
        else
        {
            // Ʋ������ ���� ��ȸ -1
            Managers.RoundManager.SubmitChance--;

            // ������ ���� ������ ���� ��ο�

            if (Managers.RoundManager.SubmitChance > 0)
            {
                // �÷��� ������ ����
                Managers.TurnManager.ChangeTurn(TurnStateFactory.GetState(Define.TurnStateType.PlayRoundTurn));
            }
            else
            {
                // ���ӿ���

                // ���� Ȱ��ȭ�� ���� �̸��� ������ �ٽ� �ε�
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
