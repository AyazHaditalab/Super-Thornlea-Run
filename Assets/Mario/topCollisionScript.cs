using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class topCollisionScript : MonoBehaviour
{
    public LayerMask solidSurfaces;
    public BoxCollider2D boxCollider2D;
    public Transform mutableBricks;
    public bool headCollided;
    private bool letBrickJiggle;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        headCollided = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollisionWithLayer(solidSurfaces);
    }

    private void CheckCollisionWithLayer(LayerMask layer)
    {
        // Check if user is touching a layer
        ContactFilter2D contactFilter = new();
        contactFilter.SetLayerMask(layer);
        contactFilter.useTriggers = true;

        Collider2D[] results = new Collider2D[10];

        int count = boxCollider2D.OverlapCollider(contactFilter, results);

        headCollided = count > 0 && results[0].transform.position.y - 0.94f - (GameObject.Find("Mario").GetComponent<BoxCollider2D>().size.y/2) > transform.position.y && results[0].CompareTag("Mutable Brick");

        if (headCollided)
        {
            // Find the closest brick to the player's middle
            GameObject closestBrick = FindClosestMutableBrick();

            if (closestBrick != null)
            {
                BrickJiggleScript brickJiggleScript = closestBrick.GetComponent<BrickJiggleScript>();

                // Check if Mario collides with the bottom of the brick
                if (true)
                {
                    // Iterate through all bricks of the mutableBricks parent object
                    foreach (Transform brick in mutableBricks)
                    {
                        // Access all the child bricks' scripts
                        GameObject childObject = brick.gameObject;
                        BrickJiggleScript brickScript = childObject.GetComponent<BrickJiggleScript>();

                        if (brickScript != null && brickScript.brickJiggle)
                        {
                            letBrickJiggle = false;
                            break;
                        }
                        else
                        {
                            letBrickJiggle = true;
                        }
                    }

                    if (letBrickJiggle && brickJiggleScript != null && brickJiggleScript.allowBrickJiggle)
                    {
                        brickJiggleScript.brickJiggle = true;
                        if (brickJiggleScript.gameObject.GetComponent<ActionBrickScript>() != null)
                        {
                            brickJiggleScript.gameObject.GetComponent<ActionBrickScript>().SpawnObject();
                        }
                        marioScript player = GameObject.FindGameObjectWithTag("Player").GetComponent<marioScript>();
                        if (!(brickJiggleScript.breakable && (player.currentState == "big_mario" || player.currentState == "lightning_mario" || player.currentState == "big_thunder_mario")))
                        {
                            audioSource.Play();
                        }
                    }
                }
            }
        }
    }
    public GameObject FindClosestMutableBrick()
    {
        GameObject[] mutableBricks = GameObject.FindGameObjectsWithTag("Mutable Brick");

        if (mutableBricks.Length == 0)
        {
            // No Mutable Bricks found
            return null;
        }

        Transform currentTransform = transform;
        GameObject closestBrick = mutableBricks[0];
        float closestDistance = Vector2.Distance(currentTransform.position, closestBrick.transform.position);

        foreach (GameObject brick in mutableBricks)
        {
            float distance = Vector2.Distance(currentTransform.position, brick.transform.position);

            if (distance < closestDistance)
            {
                closestBrick = brick;
                closestDistance = distance;
            }
        }

        return closestBrick;
    }
    public bool HeadCollided()
    {
        return headCollided;
    }
}
