using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IGrowingPlayer, IResizingPlayer, IMazePlayer
{
    /// <summary>
    /// Player's properties
    /// </summary>
    [SerializeField]
    private PlayerData playerData;
    /// <summary>
    /// Dynamic floating joystick from Joystick Pack asset 
    /// </summary>
    [SerializeField]
    private DynamicJoystick dynamicJoystick;
    /// <summary>
    /// Player's Rigidbody to MOVE
    /// </summary>
    [SerializeField]
    private Rigidbody rigidbody;
    /// <summary>
    /// Visual Representation
    /// </summary>
    [SerializeField]
    private GameObject meshObject;
    /// <summary>
    /// Canvas Cache
    /// </summary>
    [SerializeField]
    private GameObject canvas;
    /// <summary>
    /// Error blocker
    /// </summary>
    private float increasePhaseTimer = 0f;

    [Header("Maze Specific Variables")]
    [Min(0)]
    [SerializeField] private float mazeMoveTimer;
    [Min(0)]
    [SerializeField] private float mazeMoveAmount;
    [SerializeField] private GameObject mazePlayerStartPoint;
    private float mazeCurrTime = 0f;
    private float mazeLerpTime = 0f;

    public PlayerData PlayerData { get => playerData; set => playerData = value; }

    /// <summary>
    /// Listener Function called when OnGameStateChange event is fired
    /// </summary>
    private void OnStateChange()
    {
        // Resizing Phase: Change size and position of player
        if (GameManager.Instance.CurrentGameState == GameState.s4_PlayingResizing)
        {
            transform.localScale = Vector3.one; // Reset scale
            meshObject.transform.localScale = Vector3.one * 2;
            gameObject.transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
        // Maze Phase: Revert size of player
        else if (GameManager.Instance.CurrentGameState == GameState.s5_PlayingMaze)
        {
            meshObject.transform.localScale = Vector3.one;
        }
    }
    /// <summary>
    /// Add Listener to OnGameStateChange event
    /// </summary>
    private void Start()
    {
        GameManager.Instance.OnGameStateChange.AddListener(OnStateChange);
        playerData.TotalBalls = 1;
    }
    /// <summary>
    /// Populate Canvas Data -> MoveBall()
    /// </summary>
    private void Update()
    {
        // Count time since Phase Enter is fired
        increasePhaseTimer += Time.deltaTime;

        // Populate Canvas Data
        canvas.GetComponent<CanvasManager>().CurrentBalls = playerData.TotalBalls;

        // Resize Phase
        if (GameManager.Instance.CurrentGameState == GameState.s4_PlayingResizing)
        {
            GetComponent<CapsuleCollider>().radius = 0.5f;
            Resize(3);
        }
        // Maze Phase
        if (GameManager.Instance.CurrentGameState == GameState.s5_PlayingMaze)
        {
            mazeLerpTime += Time.deltaTime;
            
            if (mazeLerpTime >= 1)
            {
                mazeCurrTime += Time.deltaTime;
                if (mazeCurrTime >= mazeMoveTimer)
                {
                    MoveBall(ref mazeCurrTime, mazeMoveAmount);
                }
            }
            else
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,
                                                               new Vector3(mazePlayerStartPoint.transform.position.x,
                                                               gameObject.transform.position.y,
                                                               mazePlayerStartPoint.transform.position.z),
                                                         mazeLerpTime);
            }
        }
    }
    /// <summary>
    /// MovePlayer()
    /// </summary>
    private void FixedUpdate()
    {
        if (rigidbody == null)
            return;
        
        /// Growing Phase
        if (GameManager.Instance.CurrentGameState == GameState.s3_PlayingGrowing)
        {
            MovePlayer(true);
        }
        /// Growing Phase
        else if (GameManager.Instance.CurrentGameState == GameState.s4_PlayingResizing)
        {
            MovePlayer(false);
        }
    }
    /// <summary>
    /// MoveGamePhase(), ConsumeBall()
    /// </summary>
    /// <param name="other">Collider of other game object</param>
    private void OnTriggerEnter(Collider other)
    {
        if (rigidbody == null)
        {
            Debug.LogError("No rigidbody found!");
            return;
        }

        // Change Phase && wait 1 sec to check again
        if (other.CompareTag("Phase Enter") && increasePhaseTimer > 1)
        {
            increasePhaseTimer = 0;
            GameManager.Instance.IncreasePhase();
        }
        // Consume Ball
        else if (other.CompareTag("MazeCell"))
        {
            ConsumeBall(1);
            if (other.GetComponent<MazeCellData>().IsPainted == false)
            {
                other.GetComponent<MazeCellData>().IsPainted = true;
            }
            
        }
        // Growing Phase
        else if (other.CompareTag("GP_DifferentBall"))
        {
            Destroy(other.gameObject);
            LoseBall(5);
            canvas.GetComponent<CanvasManager>().PlayDamage();
        }
        else if (other.CompareTag("GP_SameBall"))
        {
            Destroy(other.gameObject);
            GetBall(5);
        }
        // Resizing Phase
        else if (other.CompareTag("RP_Tall Obstacle")
                || other.CompareTag("RP_Mid Obstacle")
                || other.CompareTag("RP_Small Obstacle"))
        {
            HitObstacle(other.tag, 5);
            Destroy(other.gameObject);
        }
    }

    /// <summary>
    /// Moves Player Vertical and Horizontal
    /// </summary>
    private void MovePlayer(bool control)
    {
        Vector3 forward = transform.forward * playerData.ZSpeed;
        Vector3 direction = new Vector3(dynamicJoystick.Horizontal * playerData.XSpeed, 0, forward.z);
        if (control)
        {
            rigidbody.velocity = direction * Time.deltaTime;
        }
        else
        {
            rigidbody.velocity = forward * Time.deltaTime;
        }
    }

    public void GetBall(int b)
    {
        playerData.TotalBalls += b;
        transform.localScale += Vector3.one * 0.075f;
    }

    public void LoseBall(int b)
    {
        playerData.TotalBalls -= b;     
        transform.localScale -= Vector3.one * 0.075f;
    }

    public void HitObstacle(string tag, int loseAmount)
    {
        if (tag == "RP_Tall Obstacle")
        {
            if (meshObject.transform.localScale.y < 1.75f || meshObject.transform.localScale.y > 2f)
            {
                playerData.TotalBalls -= loseAmount;
                canvas.GetComponent<CanvasManager>().PlayDamage();
            }
        }
        else if (tag == "RP_Mid Obstacle")
        {
            if (meshObject.transform.localScale.y > 1.75f)
            {
                playerData.TotalBalls -= loseAmount;
                canvas.GetComponent<CanvasManager>().PlayDamage();
            }
        }
        else if (tag == "RP_Small Obstacle")
        {
            if (meshObject.transform.localScale.y > 0.75f)
            {
                playerData.TotalBalls -= loseAmount;
                canvas.GetComponent<CanvasManager>().PlayDamage();
            }
        }
    }

    public void Resize(float force)
    {
        if (dynamicJoystick.Vertical > 0 && meshObject.transform.localScale.y < 3)
        {
            meshObject.transform.localScale = new Vector3(meshObject.transform.localScale.x - Time.deltaTime * force * dynamicJoystick.Vertical,
                                                          meshObject.transform.localScale.y + Time.deltaTime * force * dynamicJoystick.Vertical,
                                                          meshObject.transform.localScale.z);
        }
        else if (dynamicJoystick.Vertical < 0 && meshObject.transform.localScale.y > 0.5)
        {
            meshObject.transform.localScale = new Vector3(meshObject.transform.localScale.x - Time.deltaTime * force * dynamicJoystick.Vertical,
                                                          meshObject.transform.localScale.y + Time.deltaTime * force * dynamicJoystick.Vertical,
                                                          meshObject.transform.localScale.z);
        }
    }

    public void MoveBall(ref float timer, float moveAmount)
    {
        float errorMargin = 0.1f;
        float minWidth = LevelManager.Instance.LeftmostCell.transform.position.x  - errorMargin;
        float maxDepth = LevelManager.Instance.DepthmostCell.transform.position.z + errorMargin;
        float minDepth = LevelManager.Instance.NearestCell.transform.position.z   - errorMargin;
        float maxWidth = LevelManager.Instance.RightmostCell.transform.position.x + errorMargin;

        if (dynamicJoystick.Horizontal > 0.75f && gameObject.transform.position.x + moveAmount <= maxWidth)
        {
            gameObject.transform.position += Vector3.right * moveAmount;
        }
        else if (dynamicJoystick.Horizontal < -0.75f && gameObject.transform.position.x - moveAmount >= minWidth)
        {
            gameObject.transform.position += Vector3.left * moveAmount;
        }
        else if (dynamicJoystick.Vertical > 0.75f && gameObject.transform.position.z + moveAmount <= maxDepth)
        {
            gameObject.transform.position += Vector3.forward * moveAmount;
        }
        else if (dynamicJoystick.Vertical < -0.75f && gameObject.transform.position.z - moveAmount >= minDepth)
        {
            gameObject.transform.position += Vector3.back * moveAmount;
        }
        timer = 0;
    }

    public void ConsumeBall(int b)
    {
        playerData.TotalBalls -= b;
    }
}