using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpScript : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public BoxCollider2D myBoxCollider;
    public ScoreManagerScript scoreScript;
    public ScoreSpawnerScript scoreSpawnerScript;
    public marioScript playerScript;
    public SpriteRenderer spriteRenderer;
    public float moveSpeed;
    private int scoreIncrease;
    public bool movable;
    private bool spawning;
    public float spawnSpeed;
    private float ogYPos;
    public Sprite[] powerup_Animation;
    public bool hasPowerupAnimation;
    private int currentSpriteIndex = 0;
    public float animationRate;

    // Start is called before the first frame update
    void Start()
    {
        scoreIncrease = 1000;
        spawning = true;
        myRigidBody.gravityScale = 0;
        scoreScript = GameObject.Find("Stats Manager").GetComponent<ScoreManagerScript>();
        scoreSpawnerScript = GameObject.Find("Score Spawner").GetComponent<ScoreSpawnerScript>();
        playerScript = GameObject.Find("Mario").GetComponent<marioScript>();
        ogYPos = transform.position.y;
        if (hasPowerupAnimation)
        {
            StartCoroutine(AnimatePowerUp());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawning)
        {
            myRigidBody.velocity = new Vector3(0, spawnSpeed, 0);
            if (transform.position.y - ogYPos >= 1.95f)
            {
                spawning = false;
                myRigidBody.gravityScale = 13;
                myBoxCollider.enabled = true;
            }
        }
        else
        {
            if (movable)
            {
                myRigidBody.velocity = new Vector3(moveSpeed, myRigidBody.velocity.y, 0);
            }
        }
    }

    IEnumerator AnimatePowerUp()
    {
        while (true)
        {
            spriteRenderer.sprite = powerup_Animation[currentSpriteIndex];
            yield return new WaitForSeconds(animationRate);

            // Increment the sprite index and reset to 0 if it reaches the last sprite
            currentSpriteIndex = (currentSpriteIndex + 1) % powerup_Animation.Length;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            scoreScript.AddScore(scoreIncrease);
            scoreSpawnerScript.SpawnScore(transform.position, scoreIncrease.ToString());
            playerScript.PowerUpCollected();
            Destroy(gameObject);
        }
        else
        {
            // Check if the collision is horizontal (indicating a side collision)
            if (Mathf.Abs(collision.contacts[0].normal.y) < 0.1f) // Adjust the threshold as needed
            {
                // Flip the enemy's direction
                moveSpeed = -moveSpeed;
            }

        }
    }
}

