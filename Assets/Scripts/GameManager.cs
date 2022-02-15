using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameObject))]
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Time remaining to load scene
    /// </summary>
    private float timeRemaining = 0f;
    /// <summary>
    /// Async operation to load scene in the background
    /// </summary>
    private AsyncOperation asyncLoad;
    /// <summary>
    /// Used to decide whether to load scene or not
    /// </summary>
    [Header("Load New Scene?")]
    [SerializeField] private bool loadScene;
    /// <summary>
    /// Current Level number - 1
    /// </summary>
    private int currentLevelIndex;
    /// <summary>
    /// Player Game Object
    /// </summary>
    [SerializeField] private GameObject playerGO;
    /// <summary>
    /// Static GameManager instance
    /// </summary>
    private static GameManager gameManagerInstance;
    public static GameManager Instance
    {
        get
        {
            if (gameManagerInstance == null)
            {
                Debug.LogError("Game Manager Instance Not Found. Creating one...");
                GameObject go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
            }

            return gameManagerInstance;
        }
    }
    // Game Progress Event
    /// <summary>
    /// This event is fired when game state changes.
    /// </summary>
    private UnityEvent onGameStateChange;
    public UnityEvent OnGameStateChange { get => onGameStateChange; set => onGameStateChange = value; }
    /// <summary>
    /// CurrentGameState
    /// </summary>
    private GameState currentGameState;
    public GameState CurrentGameState
    {
        get => currentGameState;
    }

    /// <summary>
    /// Sets current game state; then fires onGameStateChange event.
    /// </summary>
    private void SetCurrentGameState(GameState state)
    {
        currentGameState = state;
        Debug.LogWarning(currentGameState + ": Fired");
        onGameStateChange.Invoke();
    }

    /// <summary>
    /// Listener Function called when OnGameStateChange event is fired
    /// </summary>
    private void OnStateChange()
    {
        // On Game state change
        if (CurrentGameState == GameState.s1_Loading)
        {
            LoadGame();
        }
        else if (CurrentGameState == GameState.s6_CalculatePlayerStats)
        {
            // Make some calculations (ie. 3 stars on perfect complete) if needed
            // We don't need it in our case
            SetCurrentGameState(GameState.s7_SaveGame);
        }
        else if (CurrentGameState == GameState.s7_SaveGame)
        {
            SaveGame();
        }
    }

    /// <summary>
    /// Called after object is enabled || created
    /// </summary>
    private void OnEnable()
    {
        // Initialize instance
        gameManagerInstance = this;
        
        // Initialization of Game Progress Event
        if (onGameStateChange == null)
        {
            onGameStateChange = new UnityEvent();
        }
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        onGameStateChange.AddListener(OnStateChange);
        StartCoroutine(LateStart());
    }

    /// <summary>
    /// Start Game -> Manage Game States
    /// </summary>
    private void Update()
    {
        // Start Game
        if (currentGameState == GameState.s2_NotStarted)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                SetCurrentGameState(GameState.s3_PlayingGrowing);
            }
        }

        // Check Win && lose State
        if (CheckWinState() && currentGameState == GameState.s5_PlayingMaze)
        {
            SetCurrentGameState(GameState.s6_CalculatePlayerStats);
        }
        else if (CheckLoseState() && currentGameState != GameState.s_Failed)
        {
            SetCurrentGameState(GameState.s_Failed);
        }
    }

    /// <summary>
    /// Waits one frame, then initializes s1_Loading Game State
    /// </summary>
    /// <returns>yield return new WaitForEndOfFrame();</returns>
    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        SetCurrentGameState(GameState.s1_Loading);
    }

    /// <summary>
    /// Save Game to PlayerPrefs
    /// </summary>
    private void SaveGame()
    {
        // General save
        PlayerPrefs.SetInt("CurrentLevelIndex", currentLevelIndex + 1);

        PlayerPrefs.Save();
        SetCurrentGameState(GameState.s8_Finished);
    }

    /// <summary>
    /// Load from PlayerPrefs -> Populate Canvas Data -> ConstructLevel
    /// </summary>
    private void LoadGame()
    {
        //Load level index
        if (PlayerPrefs.HasKey("CurrentLevelIndex"))
        {
            currentLevelIndex = PlayerPrefs.GetInt("CurrentLevelIndex");
        }
        else
        {
            currentLevelIndex = 0;
        }
        // Populate Canvas Data
        PopulateCanvasData();

        ConstructLevel();

        SetCurrentGameState(GameState.s2_NotStarted);
    }

    /// <summary>
    /// Build level either from blocks, or loading new scene
    /// </summary>
    private void ConstructLevel()
    {
        // Load Scene
        if (loadScene)
        {
            var _currInd = SceneManager.GetActiveScene().buildIndex;

            //timeRemaining = 3f;
            if (_currInd != currentLevelIndex && currentLevelIndex < SceneManager.sceneCountInBuildSettings)
            {
                //Load scene
                StartCoroutine(LoadScene(currentLevelIndex));
            }
            else if (_currInd != SceneManager.sceneCountInBuildSettings - 1)
            {
                //Load last scene available
                StartCoroutine(LoadScene(SceneManager.sceneCountInBuildSettings - 1));
            }
        }
        // Build on same scene
        else
        {
            //ProceduralLevelGenerator.Instance.ConstructScene(currentLevelIndex);
        }
    }

    /// <summary>
    /// Scene loader called in Coroutine
    /// </summary>
    /// <param name="sceneIndToLoad"> scene index to be loaded</param>
    /// <returns></returns>
    private IEnumerator LoadScene(int sceneIndToLoad)
    {
        if (asyncLoad == null)
        {
            asyncLoad = SceneManager.LoadSceneAsync(sceneIndToLoad);
        }
        while (true)
        {
            if (asyncLoad.progress >= 0.9f && timeRemaining <= 0f)
            {
                asyncLoad.allowSceneActivation = true;
                Debug.Log("true");
            }
            else
            {
                asyncLoad.allowSceneActivation = false;
                timeRemaining -= 1 * Time.deltaTime;
                Debug.Log("Load Progress: " + asyncLoad.progress);
                Debug.Log("timeRemaining: " + timeRemaining);
            }
            yield return null;
        }
    }

    /// <summary>
    /// Setter only
    /// </summary>
    public void IncreasePhase()
    {
        if (currentGameState == GameState.s3_PlayingGrowing)
        {
            SetCurrentGameState(GameState.s4_PlayingResizing);
        }
        else if (currentGameState == GameState.s4_PlayingResizing)
        {
            SetCurrentGameState(GameState.s5_PlayingMaze);
        }
    }

    /// <summary>
    /// Gives canvas necessary data
    /// </summary>
    private void PopulateCanvasData()
    {
        CanvasManager.Instance.CurrentLevel = currentLevelIndex + 1;
    }
    /// <summary>
    /// Win state: All maze cells are painted
    /// </summary>
    /// <returns>Returns true if game is won</returns>
    private bool CheckWinState()
    {
        if (LevelManager.Instance.AreAllCellsPainted())
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Lose state: Player lost all balls
    /// </summary>
    /// <returns>Returns true if game is lost</returns>
    private bool CheckLoseState()
    {
        if (playerGO.GetComponent<Player>().PlayerData.TotalBalls <= 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Called when this level is needed to be loaded again.
    /// </summary>
    public void LoadLevel()
    {
        if (currentLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            //Load scene
            StartCoroutine(LoadScene(currentLevelIndex));
        }
        else
        {
            //Load last scene available
            StartCoroutine(LoadScene(SceneManager.sceneCountInBuildSettings - 1));
        }
    }
}

