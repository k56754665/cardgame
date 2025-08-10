using System;
using UnityEngine;

public class GameManager
{
    public event Action OnGameOverEvent;

    public void Init()
    {

    }

    public void GameOver()
    {
        OnGameOverEvent?.Invoke();
    }
}
