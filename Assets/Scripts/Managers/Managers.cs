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

    public static DeckManager DeckManager => _deckManager;
    static DeckManager _deckManager = new DeckManager();

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

    private void Start()
    {
        TurnManager.Init();
    }

    void InitManagers()
    {
        DeckManager.Init();
    }

    void Update()
    {
        TurnManager.UpdateCurrentTurn();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DeckManager.PrintAllCards();
        }
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
