using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HomeworkEnemyScript : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public BoxCollider2D myBoxCollider;
    public SpriteRenderer mySpriteRenderer;
    public marioScript playerScript;
    public ScoreManagerScript scoreScript;
    public ScoreSpawnerScript scoreSpawnerScript;
    public List<Sprite> animationSprites;
    public Sprite deadSprite;
    public AudioSource audioSource;
    public AudioClip stompedClip;
    public float moveSpeed;
    public bool isDead;
    public float animationRate;
    public float headBounciness;
    private float timer;
    private int scoreIncrease;
    public float spawnPoint;
    private bool startMoving;
    public float killPoint;


    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        scoreIncrease = 100;
        startMoving = false;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < killPoint)
        {
            Destroy(gameObject);
        }
        if (startMoving)
        {
            myRigidBody.velocity = new Vector3(moveSpeed, 0, 0);

        }
        if (isDead)
        {
            PlayDeadAnimation();
        }
        else
        {
            PlayAliveAnimation();
        }
        timer += 1;
        if (playerScript != null)
        {
            if (playerScript.gameObject.transform.position.x > spawnPoint && !startMoving)
            {
                startMoving = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with the player and the enemy is not dead
        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            playerScript.HitEnemy();
        }
        // Check if the collision is not with the player and the enemy is not dead
        else if (!collision.gameObject.CompareTag("Player") && !isDead)
        {
            // Check if the collision is horizontal (indicating a side collision)
            if (collision.gameObject.GetComponent<MinionEnemyScript>() != null && collision.gameObject.GetComponent<MinionEnemyScript>().state == "run")
            {
                isDead = true;
                myBoxCollider.enabled = false;
                scoreScript.AddScore(scoreIncrease);
                scoreSpawnerScript.SpawnScore(transform.position, scoreIncrease.ToString());
                audioSource.clip = stompedClip;
                audioSource.Play();
            }
            else if (Mathf.Abs(collision.contacts[0].normal.y) < 0.1f) // Adjust the threshold as needed
            {
                // Flip the enemy's direction
                moveSpeed = -moveSpeed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player Bottom") && !playerScript.isDead && (GameObject.Find("Mario").transform.position.y - transform.position.y) >= 1)
        {
            isDead = true;
            myBoxCollider.enabled = false;
            playerScript.myRigidBody.velocity = new Vector3(playerScript.myRigidBody.velocity.x, headBounciness * 10, 0);
            scoreScript.AddScore(scoreIncrease);
            scoreSpawnerScript.SpawnScore(transform.position, scoreIncrease.ToString());
            audioSource.clip = stompedClip;
            audioSource.Play();
        }
    }

    void PlayAliveAnimation()
    {
        int frameIndex = (int)(timer / animationRate) % animationSprites.Count;
        SetSprite(animationSprites[frameIndex]);
    }

    void PlayDeadAnimation()
    {
        if (mySpriteRenderer.sprite != deadSprite)
        {
            SetSprite(deadSprite);
            Destroy(myBoxCollider);
        }
        myRigidBody.velocity = new Vector3(myRigidBody.velocity.x, -5, 0);
        RotateObject(1);
    }

    private void SetSprite(Sprite sprite)
    {
        if (mySpriteRenderer.sprite != sprite)
        {
            mySpriteRenderer.sprite = sprite;
        }
    }

    void RotateObject(float degrees)
    {
        // Get the current rotation
        float currentRotation = transform.rotation.eulerAngles.z;

        // Calculate the new rotation
        float newRotation = currentRotation - degrees;

        // Apply the new rotation
        transform.rotation = Quaternion.Euler(0f, 0f, newRotation);
    }

}

