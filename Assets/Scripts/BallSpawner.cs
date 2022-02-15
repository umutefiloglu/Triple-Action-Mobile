using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : A_ProceduralBlockGenerator
{
    /// <summary>
    /// Friend balls * this value
    /// </summary>
    [Tooltip("  0   -> Use 0 friend balls" +
           "\n  1   -> Use all points for friend balls" +
           "\n(0,1) -> Use ratio (this value * max points)")]
    [Range(0f, 1f)]
    [SerializeField] private float friendBallsPercentage = 1;
    /// <summary>
    /// Number of spawn points * this ratio
    /// </summary>
    [Tooltip("  0   -> Use 0 spawn points" +
           "\n  1   -> Use all spawn points" +
           "\n(0,1) -> Use ratio (this value * max points)")]
    [Range(0, 1)]
    [SerializeField] private float usedPointsPercentage = 1;
    [SerializeField] [ReadOnly] private int totalSpawnPoints = 0;
    [SerializeField] [ReadOnly] private int totalFriendBalls = 0;
    [SerializeField] [ReadOnly] private int totalEnemyBalls = 0;

    /// <summary>
    /// base.Start() -> SpawnFriendBalls() -> SpawnEnemyBalls()
    /// </summary>
    protected override void Start()
    {
        base.Start();
        SpawnFriendBalls();
        SpawnEnemyBalls();
    }
    /// <summary>
    /// SetActive this number of friend balls: 
    /// Mathf.Floor(availablePoints.Length * usedPointsPercentage
    ///                 * friendBallsPercentage)
    /// </summary>
    private void SpawnFriendBalls()
    {
        totalSpawnPoints = (int)Mathf.Floor(availablePoints.Length * usedPointsPercentage);
        totalFriendBalls = (int)Mathf.Floor(totalSpawnPoints * friendBallsPercentage);

        for (int i = 0; i < totalFriendBalls; i++)
        {
            var _x = Random.Range(0, availablePoints.Length - 1);
            while (availablePoints[_x].isAvailable == false)
            {
                _x = Random.Range(0, availablePoints.Length - 1);
            }

            availablePoints[_x].isAvailable = false;
            childGrounds[_x].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// SetActive this number of enemy balls: 
    /// Mathf.Floor((availablePoints.Length * usedPointsPercentage) * (1 - friendBallsPercentage))
    /// </summary>
    private void SpawnEnemyBalls()
    {
        int totalEnemyBalls = totalSpawnPoints - totalFriendBalls;
        for (int i = 0; i < totalEnemyBalls; i++)
        {
            var _x = Random.Range(0, availablePoints.Length - 1);
            while (availablePoints[_x].isAvailable == false)
            {
                _x = Random.Range(0, availablePoints.Length - 1);
            }

            availablePoints[_x].isAvailable = false;
            childGrounds[_x].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
    }
}
