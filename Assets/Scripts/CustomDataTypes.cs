using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Phases of game
/// </summary>
public enum GameState
{
    s1_Loading,
    s2_NotStarted,
    s3_PlayingGrowing,
    s4_PlayingResizing,
    s5_PlayingMaze,
    s6_CalculatePlayerStats,
    s7_SaveGame,
    s8_Finished,
    s_Failed
}
/// <summary>
/// UI Panels
/// </summary>
[System.Serializable]
public enum UIPanel
{
    MainMenuUI, InGameUI, FailedUI, WinUI
};
/// <summary>
/// Environmental objects' spawn points
/// </summary>
[System.Serializable]
public struct SpawnPoints
{
    public Vector3 point;
    public bool isAvailable;
}