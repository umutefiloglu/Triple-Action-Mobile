using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [Header("Game Objects References")]
    /// <summary>
    /// Panels to control
    /// </summary>
    [SerializeField] private GameObject[] panels;
    /// <summary>
    /// Slider object
    /// </summary>
    [SerializeField] private GameObject levelSlider;
    /// <summary>
    /// Min required balls text object
    /// </summary>
    [SerializeField] private TextMeshProUGUI minBallText;
    /// <summary>
    /// Current balls text object
    /// </summary>
    [SerializeField] private TextMeshProUGUI currBallText;
    [Header("Variables")]
    /// <summary>
    /// Number of Minimum required balls to complete maze
    /// </summary>
    [SerializeField] private int minReqBalls;
    /// <summary>
    /// Number of collected && remaining balls (Growing && Maze Phase)
    /// </summary>
    [SerializeField] private int currentBalls;
    /// <summary>
    /// Current Level Number (Level index + 1)
    /// </summary>
    [SerializeField] private int currentLevel;
    /// <summary>
    /// [0 -> Started Growing Phase, 1 -> Finished Resizing Phase]
    /// </summary>
    [Range(0f, 1f)]
    [SerializeField] private float levelSliderProgress;
    /// <summary>
    /// Shows when ball || game is lost
    /// </summary>
    [SerializeField] private AnimationClip damageFX;

    public int MinReqBalls { get => minReqBalls; set => minReqBalls = value; }
    public int CurrentBalls { get => currentBalls; set => currentBalls = value; }
    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public float LevelSliderProgress { get => levelSliderProgress; set => levelSliderProgress = value; }

    private static CanvasManager canvasManagerInstance;
    public static CanvasManager Instance
    {
        get
        {
            if (canvasManagerInstance == null)
            {
                Debug.LogError("Canvas Manager Instance Not Found. Creating one...");
                GameObject go = new GameObject("CanvasManager");
                go.AddComponent<CanvasManager>();
            }

            return canvasManagerInstance;
        }
    }


    /// <summary>
    /// Set instance
    /// </summary>
    private void OnEnable()
    {
        // Initialize instance
        canvasManagerInstance = this;
    }

    /// <summary>
    /// Listener Function called when OnGameStateChange event is fired
    /// </summary>
    private void OnStateChange()
    {
        if (GameManager.Instance.CurrentGameState == GameState.s2_NotStarted)
        {
            ActivateUI(UIPanel.MainMenuUI);
        }
        else if (GameManager.Instance.CurrentGameState == GameState.s3_PlayingGrowing)
        {
            ActivateUI(UIPanel.InGameUI);
            minBallText.text = minReqBalls.ToString();
        }
        else if (GameManager.Instance.CurrentGameState == GameState.s5_PlayingMaze)
        {
            ToggleSlider(false);
        }
        else if (GameManager.Instance.CurrentGameState == GameState.s_Failed)
        {
            ActivateUI(UIPanel.FailedUI);
        }
        else if (GameManager.Instance.CurrentGameState == GameState.s8_Finished)
        {
            ActivateUI(UIPanel.WinUI);
        }
    }
    /// <summary>
    /// Add Listener to OnGameStateChange event
    /// </summary>
    void Start()
    {
        GameManager.Instance.OnGameStateChange.AddListener(OnStateChange);
    }
    /// <summary>
    /// Updates currBallText & level slider progress
    /// </summary>
    private void Update()
    {
        levelSlider.GetComponent<Slider>().value = levelSliderProgress;
        currBallText.text = currentBalls.ToString();
    }
    /// <summary>
    /// Activates given panel, but others
    /// </summary>
    /// <param name="p">Panel to activate</param>
    private void ActivateUI(UIPanel p)
    {
        foreach (var panel in panels)
        {
            if (panel.name == p.ToString())
                panel.SetActive(true);
            else
                panel.SetActive(false);
        }
    }
    /// <summary>
    /// Plays damage animation
    /// </summary>
    public void PlayDamage()
    {
        transform.GetChild(0).gameObject.GetComponent<Animator>().Play("DamageAnim", 0, 0);
    }
    /// <summary>
    /// Toggles slider on & off
    /// </summary>
    /// <param name="showSlider">if true, then slider shows up</param>
    public void ToggleSlider(bool showSlider)
    {
        levelSlider.SetActive(showSlider);
    }
}
