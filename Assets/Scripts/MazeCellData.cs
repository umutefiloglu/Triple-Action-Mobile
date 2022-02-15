using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellData : MonoBehaviour
{
    /// <summary>
    /// Used to simulate painting action
    /// </summary>
    [SerializeField] private Material paintedMaterial;
    /// <summary>
    /// Holds maze cell status
    /// </summary>
    private bool isPainted = false;
    public bool IsPainted { get { return isPainted; }
        set
        {
            GetComponent<MeshRenderer>().material = paintedMaterial;
            isPainted = value; 
        } 
    }
}
