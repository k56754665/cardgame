using UnityEngine;
using TMPro;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] TMP_Text _currentScoreText;
    [SerializeField] TMP_Text _scoreText;

    private void Start()
    {
        HandleCurrentScoreText(Managers.RoundManager.Score);
        HandleScoreText(0);
        Managers.RoundManager.OnExpressionChangeEvent += HandleExpressionChange;
        Managers.RoundManager.OnExpressionClearEvent += HandleExpressionClear;
        Managers.RoundManager.OnChangeScoreEvent += HandleCurrentScoreText;
    }

    void HandleExpressionChange()
    {
        if (Managers.RoundManager.TryGetExpressionScore(out int score))
            HandleScoreText(score);
        else
            HandleScoreText(0);
    }

    void HandleExpressionClear()
    {
        HandleScoreText(0);
    }

    private void HandleCurrentScoreText(int score)
    {
        _currentScoreText.text = $"현재 점수\n{score}";
    }

    private void HandleScoreText(int score)
    {
        _scoreText.text = $"{score}";
    }

    private void OnDestroy()
    {
        Managers.RoundManager.OnExpressionChangeEvent -= HandleExpressionChange;
        Managers.RoundManager.OnExpressionClearEvent -= HandleExpressionClear;
        Managers.RoundManager.OnChangeScoreEvent -= HandleCurrentScoreText;
    }
}
