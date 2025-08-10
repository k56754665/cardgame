using UnityEngine;

public class UI_GameOver : MonoBehaviour
{
    Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        Managers.GameManager.OnGameOverEvent += ShowGameOver;
    }
    
    private void ShowGameOver()
    {
        _canvas.enabled = true;
    }

    private void OnDestroy()
    {
        Managers.GameManager.OnGameOverEvent -= ShowGameOver;
    }
}
