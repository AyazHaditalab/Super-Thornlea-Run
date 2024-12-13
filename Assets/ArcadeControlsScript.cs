using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeControlsScript : MonoBehaviour
{
    void Update()
    {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                print("Key pressed: " + keyCode);
            }
        }
    }
}
