using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainCameraScript : MonoBehaviour
{
    public GameObject playerObject;
    public float initialCameraX;
    public Camera mainCamera;
    public bool cameraMoving;
    public bool restrictMoving;

    void Start()
    {
        initialCameraX = transform.position.x;
        cameraMoving = false;
    }

    void Update()
    {
        if (!restrictMoving)
        {
            cameraMoving = playerObject.transform.position.x > initialCameraX;
            if (cameraMoving && playerObject.GetComponent<marioScript>() != null)
            {
                MoveCamera();
            }
        }
    }
    void MoveCamera()
    {
        if (!playerObject.GetComponent<marioScript>().completedLevel)
        {
            // Get the current position of the camera
            Vector3 currentPosition = transform.position;

            // Change the camera position
            Vector3 newPosition = new(playerObject.transform.position.x, currentPosition.y, currentPosition.z);

            // Set the camera's position to the new position
            transform.position = newPosition;

            initialCameraX = playerObject.transform.position.x;
        }
        else
        {
            if (playerObject.GetComponent<marioScript>().winTime >= 2.7f)
            {
                transform.position = new(playerObject.transform.position.x - 2.0142f, transform.position.y, transform.position.z);
                initialCameraX = playerObject.transform.position.x;

            }
        }
    }

    public float GetSize()
    {
        return mainCamera.orthographicSize;
    }
}
