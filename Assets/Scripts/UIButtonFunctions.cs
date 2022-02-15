using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonFunctions : MonoBehaviour
{
    /// <summary>
    /// It is not a good practice to use same function for all button clicks.
    /// However, in our case, we don't need to create another UI button click function.
    /// </summary>
    public void UIButtonOnClick()
    {
        GameManager.Instance.LoadLevel();
    }
}
