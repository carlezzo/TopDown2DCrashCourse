using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    public GameState currentGameState = GameState.Playing;
    
    [Header("Events")]
    public UnityEvent<GameState> OnGameStateChanged;
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;
    
    [Header("Settings")]
    public bool pauseOnFocusLoss = true;
    
    private bool isPaused = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        SetGameState(GameState.Playing);
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (currentGameState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentGameState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }
    
    public void SetGameState(GameState newState)
    {
        if (currentGameState != newState)
        {
            GameState previousState = currentGameState;
            currentGameState = newState;
            
            OnGameStateChanged?.Invoke(newState);
            
        }
    }
    
    public void PauseGame()
    {
        if (!isPaused)
        {
            // isPaused = true;
            // Time.timeScale = 0f;
            // SetGameState(GameState.Paused);
            // OnGamePaused?.Invoke();
            
        }
    }
    
    public void ResumeGame()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            SetGameState(GameState.Playing);
            OnGameResumed?.Invoke();
            
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ClearInventory();
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
    
    public void QuitGame()
    {
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (pauseOnFocusLoss && !hasFocus && currentGameState == GameState.Playing)
        {
            PauseGame();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseOnFocusLoss && pauseStatus && currentGameState == GameState.Playing)
        {
            PauseGame();
        }
    }
}

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Inventory,
    Settings
}