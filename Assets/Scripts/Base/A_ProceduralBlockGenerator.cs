using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class A_ProceduralBlockGenerator : MonoBehaviour
{
    /// <summary>
    /// Mini blocks building the main area
    /// </summary>
    [SerializeField]
    [ReadOnly] protected GameObject[] childGrounds;
    /// <summary>
    /// Available spawn points for environmental objects
    /// </summary>
    [SerializeField]
    [ReadOnly] protected SpawnPoints[] availablePoints;
    /// <summary>
    /// GetChildGrounds() -> GetGenerationPoints() -> Random.InitState(...)
    /// </summary>
    protected virtual void Start()
    {
        GetChildGrounds();
        GetGenerationPoints();
        Random.InitState(System.DateTime.Now.Second * System.DateTime.Now.Millisecond);
    }
    /// <summary>
    /// Initializes && Populates childGrounds array
    /// </summary>
    private void GetChildGrounds()
    {
        childGrounds = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            childGrounds[i] = transform.GetChild(i).gameObject;
            
        }
    }
    /// <summary>
    /// Initializes && Populates availablePoints array
    /// </summary>
    private void GetGenerationPoints()
    {
        availablePoints = new SpawnPoints[childGrounds.Length * childGrounds[0].transform.childCount];
        for (int k = 0; k < availablePoints.Length;)
        {
            for (int i = 0; i < childGrounds.Length; i++)
            {
                for (int j = 0; j < childGrounds[i].transform.childCount; j++)
                {
                    availablePoints[k].isAvailable = true;
                    availablePoints[k].point = childGrounds[i].transform.GetChild(j).position;
                    k++;
                }
            }
        }

    }
}
