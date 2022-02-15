using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour, IMazeManager
{
    /// <summary>
    /// Maze Phase start point
    /// </summary>
    [Tooltip("Maze Phase Enter Point")]
    [SerializeField] private GameObject mazePhaseEnter;
    /// <summary>
    /// Growing Phase start point
    /// </summary>
    [Tooltip("Growing Phase Start Point")]
    [SerializeField] private GameObject growingPhaseStart;
    /// <summary>
    /// Player on the scene
    /// </summary>
    [Tooltip("Main Player Object")]
    [SerializeField] private GameObject playerGO;
    /// <summary>
    /// Level manager static instance
    /// </summary>
    private static LevelManager instance;
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("LevelManager");
                go.AddComponent<LevelManager>();
            }

            return instance;
        }
    }

    
    [Header("Project Specific Variables")]
    /// <summary>
    /// Maze Object
    /// </summary>
    [SerializeField] private GameObject maze;
    /// <summary>
    /// Max X value
    /// </summary>
    [SerializeField] private GameObject rightmostCell;
    /// <summary>
    /// Min X value
    /// </summary>
    [SerializeField] private GameObject leftmostCell;
    /// <summary>
    /// Max Z value
    /// </summary>
    [SerializeField] private GameObject depthmostCell;
    /// <summary>
    /// Min Z value
    /// </summary>
    [SerializeField] private GameObject nearestCell;
    public GameObject RightmostCell { get => rightmostCell; set => rightmostCell = value; }
    public GameObject LeftmostCell { get => leftmostCell; set => leftmostCell = value; }
    public GameObject DepthmostCell { get => depthmostCell; set => depthmostCell = value; }
    public GameObject NearestCell { get => nearestCell; set => nearestCell = value; }

    /// <summary>
    /// Sets instance
    /// </summary>
    private void OnEnable()
    {
        instance = this;
    }
    /// <summary>
    /// Calculate number of empty squares
    /// </summary>
    private void Start()
    {
        CanvasManager.Instance.MinReqBalls = CalcMinReqBalls(maze.transform);
    }
    /// <summary>
    /// CalcRemainingRoad()
    /// </summary>
    private void Update()
    {
        CalcRemainingRoad();
    }
    /// <summary>
    /// Calculate remaining road
    /// </summary>
    private void CalcRemainingRoad()
    {
        //   0      -> Started from Growing Phase
        // (0,1)    -> Somewhere between
        //   1      -> Arrived Maze Phase
        CanvasManager.Instance.LevelSliderProgress =
                       1 - (
                       Vector3.Distance(mazePhaseEnter.transform.position, playerGO.transform.position)
                     / Vector3.Distance(mazePhaseEnter.transform.position, growingPhaseStart.transform.position)
                    );
    }
    /// <summary>
    /// Calculate number of empty squares of Maze
    /// </summary>
    /// <param name="mazeParent">Maze objects holding cells as children</param>
    /// <returns></returns>
    public int CalcMinReqBalls(Transform mazeParent)
    {
        int _numOfCells = 0;

        for (int i = 0; i < mazeParent.childCount; i++)
        {
            if (mazeParent.GetChild(i).GetChild(0).gameObject.activeInHierarchy == false)
            {
                _numOfCells++;
            }
        }

        return _numOfCells;
    }
    /// <summary>
    /// Checks if maze is completed
    /// </summary>
    /// <returns>Returns true if all cells are painted. Otherwise, false.</returns>
    public bool AreAllCellsPainted()
    {
        for (int i = 0; i < maze.transform.childCount; i++)
        {
            if (maze.transform.GetChild(i).gameObject.GetComponent<MazeCellData>().IsPainted == false)
            {
                return false;
            }
        }

        return true;
    }
}
