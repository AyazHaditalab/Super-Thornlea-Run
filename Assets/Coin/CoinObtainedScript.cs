using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinObtainedScript : MonoBehaviour
{
    public SpriteRenderer mySpriteRenderer;
    public Rigidbody2D myRigidbody;
    public Sprite[] animationSprites;
    public AudioSource audioSource;
    public ScoreManagerScript scoreManagerScript;
    public ScoreSpawnerScript spawnerScript;
    public CoinsManagerScript coinsManagerScript;
    public float coinSpeed;
    public int animationSpeed;
    private int currentIndex = 0;
    private float ogPosition;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody.velocity = new Vector3(0, coinSpeed, 0);
        StartCoroutine(AnimateSprites());
        ogPosition = transform.position.y;
        scoreManagerScript = GameObject.Find("Stats Manager").GetComponent<ScoreManagerScript>();
        coinsManagerScript = GameObject.Find("Stats Manager").GetComponent<CoinsManagerScript>();
        spawnerScript = GameObject.Find("Score Spawner").GetComponent<ScoreSpawnerScript>();
        scoreManagerScript.AddScore(200);
        coinsManagerScript.CoinObtained();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < ogPosition)
        {
            if (mySpriteRenderer.sprite != null)
            {
                spawnerScript.SpawnScore(transform.position, "200");
            }
            if (!audioSource.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                mySpriteRenderer.sprite = null;
            }
        }
    }

    private IEnumerator AnimateSprites()
    {
        while (transform.position.y >= ogPosition)
        {
            // Change the sprite
            SetSprite(animationSprites[currentIndex]);

            // Move to the next sprite
            currentIndex = (currentIndex + 1) % animationSprites.Length;

            // Wait for a certain amount of time based on speed
            yield return new WaitForSeconds(1 / animationSpeed);
        }
    }

    void SetSprite(Sprite sprite)
    {
        mySpriteRenderer.sprite = sprite;
    }
}
