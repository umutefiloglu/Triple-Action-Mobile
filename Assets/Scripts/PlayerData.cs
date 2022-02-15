using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerData", menuName = "Player Data", order = 51)]
public class PlayerData : ScriptableObject
{
    /// <summary>
    /// Player's Horizontal Speed
    /// </summary>
    [SerializeField]
    private float xSpeed = 1f;
    /// <summary>
    /// Player's Forward Speed
    /// </summary>
    [SerializeField]
    private float zSpeed = 1f;
    /// <summary>
    /// Player's Rotation Speed
    /// </summary>
    [SerializeField][Range(0f, 1f)]
    private float rotationSpeed = 1f;
    /// <summary>
    /// Total number of collected balls
    /// </summary>
    [SerializeField] private int totalBalls = 0;

    public float XSpeed { get => xSpeed; set => xSpeed = value; }
    public float ZSpeed { get => zSpeed; set => zSpeed = value; }
    public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
    public int TotalBalls { get => totalBalls; set => totalBalls = value; }
}
