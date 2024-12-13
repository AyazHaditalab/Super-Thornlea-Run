using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public ScoreManagerScript scoreManagerScript;
    public GameObject brickUnderCoin;
    public List<Sprite> animationSprites = new List<Sprite>();
    public GameObject coinObtained;
    public int scoreIncrease;
    public float animationSpeed = 5.0f; // Adjust the animation speed as needed

    private void Start()
    {
        // Start the sprite animation coroutine
        StartCoroutine(AnimateSprites());
    }

    private IEnumerator AnimateSprites()
    {
        int currentFrame = 0;

        while (true)
        {
            // Switch to the next sprite
            spriteRenderer.sprite = animationSprites[currentFrame];

            // Move to the next sprite in a circular manner
            currentFrame = (currentFrame + 1) % animationSprites.Count;

            // Wait for a certain amount of time based on animation speed
            yield return new WaitForSeconds(1 / animationSpeed);
        }
    }

    void Update()
    {
        if (brickUnderCoin != null)
        {
            if (brickUnderCoin.GetComponent<BrickJiggleScript>().brickJiggle)
            {
                scoreManagerScript.AddScore(scoreIncrease);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            scoreManagerScript.AddScore(scoreIncrease);
            Destroy(gameObject);
        }
    }
}
