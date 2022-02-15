using UnityEngine;

public interface IMazeManager
{
    /// <summary>
    /// Calculates and returns minimum number of balls required to complete maze
    /// </summary>
    /// <param name="mazeParent">Parent object holding maze cells</param>
    /// <returns></returns>
    int CalcMinReqBalls(Transform mazeParent);
}