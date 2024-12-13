using System;
using System.Collections;
using UnityEngine;

public class LightningScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    public Rigidbody2D myRigidbody;
    private GameObject player;
    public AudioSource audioSource;
    public AudioClip bumpClip;
    public float moveSpeed;
    public float bounciness;
    public Sprite[] animationSprites;
    public float animationRate;
    public Sprite[] explosionSprites;
    private int currentSpriteIndex = 0;
    private bool explode;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Mario");
        StartCoroutine(AnimateLightning());
        if (player.GetComponent<SpriteRenderer>().flipX == true)
        {
            spriteRenderer.flipX = true;
            moveSpeed = -moveSpeed;
        }
    }

    IEnumerator AnimateLightning()
    {
        while (!explode)
        {
            spriteRenderer.sprite = animationSprites[currentSpriteIndex];
            yield return new WaitForSeconds(animationRate);

            // Increment the sprite index and reset to 0 if it reaches the last sprite
            currentSpriteIndex = (currentSpriteIndex + 1) % animationSprites.Length;
        }

        // If explode is true, switch to explosion animation
        StartCoroutine(AnimateExplosion());
    }

    IEnumerator AnimateExplosion()
    {
        // Reset the current sprite index
        currentSpriteIndex = 0;

        // Loop through explosion sprites
        while (currentSpriteIndex < explosionSprites.Length)
        {
            spriteRenderer.sprite = explosionSprites[currentSpriteIndex];
            yield return new WaitForSeconds(animationRate);

            currentSpriteIndex++;
        }

        // Destroy the object after the explosion animation
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!explode)
        {
            // Ensure the fireball moves at a constant velocity
            myRigidbody.velocity = new Vector2(moveSpeed, myRigidbody.velocity.y);
        }
        else
        {
            myRigidbody.velocity = Vector2.zero;
            myRigidbody.gravityScale = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is NOT coming from the bottom and if it is coming from the left or right
        bool collisionNotFromBottomAndFromSide = false;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // If the normal's y-component is less than 1, the collision is not from the bottom
            // If the normal's x-component is not close to 0, the collision is from the left or right
            if (contact.normal.y < 1 && Mathf.Abs(contact.normal.x) > 0.1f)
            {
                collisionNotFromBottomAndFromSide = true;
                break;
            }
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponent<HomeworkEnemyScript>() != null)
            {
                collision.gameObject.GetComponent<HomeworkEnemyScript>().audioSource.Play();
                collision.gameObject.GetComponent<HomeworkEnemyScript>().isDead = true;
                collision.gameObject.GetComponent<HomeworkEnemyScript>().scoreScript.AddScore(100);
                collision.gameObject.GetComponent<HomeworkEnemyScript>().scoreSpawnerScript.SpawnScore(transform.position, 100.ToString());
            }
            else
            {
                collision.gameObject.GetComponent<MinionEnemyScript>().audioSource.Play();
                collision.gameObject.GetComponent<MinionEnemyScript>().isDead = true;
                collision.gameObject.GetComponent<MinionEnemyScript>().scoreScript.AddScore(200);
                collision.gameObject.GetComponent<MinionEnemyScript>().scoreSpawnerScript.SpawnScore(transform.position, 200.ToString());
            }
            Destroy(boxCollider);
            explode = true;
            StartCoroutine(AnimateExplosion());
        }
        else if (collisionNotFromBottomAndFromSide && !collision.gameObject.CompareTag("Player"))
        {
            // Trigger explosion animation upon collision
            explode = true;
            Destroy(boxCollider);
            StartCoroutine(AnimateExplosion());
            audioSource.clip = bumpClip;
            audioSource.Play();
        }
        else
        {
            // Implement a bouncing effect to simulate the fireball jumping up and down
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, bounciness);
        }
    }
}
