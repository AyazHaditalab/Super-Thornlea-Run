using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FlagScript : MonoBehaviour
{
    public float flagMinimumY;
    public Rigidbody2D myRigidBody;
    public float moveSpeed;
    public GameObject mario;
    public GameObject flagBrick;
    public ScoreManagerScript scoreManager;
    public ScoreSpawnerScript scoreSpawner;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody.velocity = new Vector3(0, -moveSpeed, 0);
        float marioHeight = (mario.transform.position.y - (mario.GetComponent<BoxCollider2D>().size.y / 2)) - (flagBrick.transform.position.y + (flagBrick.GetComponent<BoxCollider2D>().size.y / 2));

        int score = 0;
        if (marioHeight <= 1)
        {
            score = 100;
        }
        else if (marioHeight <= 4)
        {
            score = 400;
        }
        else if (marioHeight <= 10)
        {
            score = 1000;
        }
        else if (marioHeight <= 14)
        {
            score = 2000;
        }
        else
        {
            score = 5000;
        }
        scoreManager.AddScore(score);
        GameObject flagPole = GameObject.Find("Flag Pole");
        scoreSpawner.SpawnScore(new Vector3(flagPole.transform.position.x, flagPole.transform.position.y + 9, 0), $"{score}");

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= flagMinimumY)
        {
            transform.position = new Vector3(transform.position.x, flagMinimumY, 0);
            myRigidBody.velocity = new Vector3(0, 0, 0);
        }
        else
        {
            myRigidBody.velocity = new Vector3(0, -moveSpeed, 0);
        }
    }
}
