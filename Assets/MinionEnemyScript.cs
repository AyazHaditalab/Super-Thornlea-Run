using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinionEnemyScript : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public BoxCollider2D myBoxCollider;
    public SpriteRenderer mySpriteRenderer;
    public marioScript playerScript;
    public ScoreManagerScript scoreScript;
    public ScoreSpawnerScript scoreSpawnerScript;
    public List<Sprite> animationSprites;
    public Sprite deadSprite;
    public Sprite shellSprite;
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
    public string state;
    private float interationTime = 0f;


    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        scoreIncrease = 200;
        startMoving = false;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        interationTime += Time.deltaTime;
        if (transform.position.y < killPoint)
        {
            Destroy(gameObject);
        }
        if (startMoving)
        {
            if (state != "still")
            {
                myRigidBody.velocity = new Vector3(moveSpeed, myRigidBody.velocity.y, 0);
            }
        }
        if (isDead)
        {
            PlayDeadAnimation();
            myRigidBody.velocity = new Vector3(0, myRigidBody.velocity.y, 0);
        }
        else
        {
            if (state == "walking")
            {
                PlayAliveAnimation();
            }
            else if (state == "still")
            {
                myRigidBody.velocity = new Vector3(0, 0, 0);
            }
        }
        timer += 1;
        if (playerScript != null)
        {
            if (playerScript.gameObject.transform.position.x > spawnPoint && !startMoving)
            {
                startMoving = true;
            }
        }
        mySpriteRenderer.flipX = moveSpeed > 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with the player and the enemy is not dead
        if (collision.gameObject.CompareTag("Player") && !isDead && state != "still")
        {
            playerScript.HitEnemy();
        }
        else if (collision.gameObject.CompareTag("Player") && !isDead && state == "still" && interationTime > 0.1f)
        {
            interationTime = 0;
            scoreScript.AddScore(scoreIncrease);
            audioSource.clip = stompedClip;
            audioSource.Play();
            state = "run";
            if (playerScript.gameObject.transform.position.x < transform.position.x)
            {
                moveSpeed = 20;
            }
            else
            {
                moveSpeed = -20;
            }
        }
        // Check if the collision is not with the player and the enemy is not dead
        else if (!collision.gameObject.CompareTag("Player") && !isDead && !(collision.gameObject.GetComponent<HomeworkEnemyScript>() != null && state == "run"))
        {
            // Check if the collision is horizontal (indicating a side collision)
            if (Mathf.Abs(collision.contacts[0].normal.y) < 0.1f) // Adjust the threshold
            {
                // Flip the enemy's direction
                moveSpeed = -moveSpeed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player Bottom") && !playerScript.isDead && (GameObject.Find("Mario").transform.position.y - transform.position.y) >= 0.5 && (state == "walking" || state == "run") && interationTime > 0.1f)
        {
            interationTime = 0;
            playerScript.myRigidBody.velocity = new Vector3(playerScript.myRigidBody.velocity.x, headBounciness * 10, 0);
            scoreScript.AddScore(scoreIncrease);
            audioSource.clip = stompedClip;
            audioSource.Play();
            state = "still";
            SetSprite(shellSprite);
            myRigidBody.velocity = new Vector3(0, 0, 0);
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
            myRigidBody.velocity = new Vector3(0, 20, 0);
            mySpriteRenderer.flipY = true;
        }
    }

    private void SetSprite(Sprite sprite)
    {
        if (mySpriteRenderer.sprite != sprite)
        {
            mySpriteRenderer.sprite = sprite;
        }
        if (mySpriteRenderer.sprite == shellSprite)
        {
            myBoxCollider.offset = new Vector2(0, -0.355866f);
            myBoxCollider.size = new Vector2(1.74f, 1.208268f);
        }
        else
        {
            myBoxCollider.offset = new Vector2(0, 0);
            myBoxCollider.size = new Vector2(1.46f, 1.92f);
        }
    }

}
