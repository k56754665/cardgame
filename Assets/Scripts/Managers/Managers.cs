using UnityEngine;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{
    public static Managers Instance => _instance;
    static Managers _instance;

    public static GameManager GameManager => _gameManager;
    static GameManager _gameManager = new GameManager();

    public static TurnManager TurnManager => _turnManager;
    static TurnManager _turnManager = new TurnManager();

    public static RoundManager RoundManager => _roundManager;
    static RoundManager _roundManager = new RoundManager();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitManagers()
    {
        TurnManager.Init();
    }

    void Update()
    {
        TurnManager.UpdateCurrentTurn();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitManagers();
    }


    void OnDestroy()
    {
        if (_instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
