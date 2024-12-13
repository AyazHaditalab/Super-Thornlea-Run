using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class BrickJiggleScript : MonoBehaviour
{
    public bool allowBrickJiggle;
    public bool brickJiggle;
    private float deltaYBrick;
    private Vector3 originalPosition;
    private float jiggleIntensity;
    private float jiggleHeight;
    private bool jiggleCompleted;
    public bool jiggleTriggered;
    public bool breakable;
    private marioScript player;
    public GameObject brickBreakAnimation;

    // Start is called before the first frame update
    void Start()
    {
        deltaYBrick = 0;
        jiggleIntensity = 0.03f;
        jiggleHeight = 0.5f;
        originalPosition = transform.position;
        jiggleCompleted = false;
        player = GameObject.Find("Mario").GetComponent<marioScript>();
    }

    // Update is called once per frame
    void Update()
    {
        jiggleCompleted = false;

        if (brickJiggle && allowBrickJiggle)
        {
            if ((player.currentState == "big_mario" || player.currentState == "lightning_mario" || player.currentState == "big_thunder_mario") && breakable)
            {
                Instantiate(brickBreakAnimation, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            BrickJiggle();
        }
    }

    void BrickJiggle()
    {
        // Get the current position of the camera
        Vector3 currentPosition = transform.position;

        // Set the borders position to the new position
        transform.position = new Vector3(currentPosition.x, currentPosition.y + jiggleIntensity, currentPosition.z);

        // Update change in y position
        deltaYBrick += jiggleIntensity;

        // Reverse jiggle movement when at peak
        if (deltaYBrick >= jiggleHeight)
        {
            jiggleIntensity = -jiggleIntensity;
        }

        // End brick jiggle
        if (deltaYBrick <= 0)
        {
            brickJiggle = false;
            jiggleIntensity = -jiggleIntensity;
            transform.position = originalPosition;
            jiggleCompleted = true;
        }
    }

    public bool BrickHit()
    {
        return jiggleCompleted;
    }
}
