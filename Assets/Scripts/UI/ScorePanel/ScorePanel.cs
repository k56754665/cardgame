using UnityEngine;
using TMPro;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] TMP_Text _currentScoreText;
    [SerializeField] TMP_Text _scoreText;

    private void Start()
    {

    }

    private void HandleCurrentScoreText(int score)
    {
        _currentScoreText.text = $"���� ����\n{score}";
    }

    private void HandleScoreText(int score)
    {
        _scoreText.text = $"{score}";
    }

    private void OnDestroy()
    {

    }
}
