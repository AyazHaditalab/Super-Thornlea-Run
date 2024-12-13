using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBrickScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite lockedSprite;
    public List<Sprite> animationSprites;
    private BrickJiggleScript brickJiggleScript;
    private int hitCount;
    public int maxHitCount;
    public float animationRate;
    public GameObject spawningObject;
    public bool hasAnimation;

    // Start is called before the first frame update
    void Start()
    {
        brickJiggleScript = gameObject.GetComponent<BrickJiggleScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitCount = 0;
        StartCoroutine(AnimationCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (brickJiggleScript.BrickHit())
        {
            hitCount++;
        }
        if (hitCount == maxHitCount)
        {
            brickJiggleScript.allowBrickJiggle = false;
            SetSprite(lockedSprite);
        }
    }

    private void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    private IEnumerator AnimationCoroutine()
    {
        int currentFrame = 0;

        while (spriteRenderer.sprite != lockedSprite && hasAnimation)
        {
            // Switch to the next sprite
            SetSprite(animationSprites[currentFrame]);

            // Move to the next sprite in a circular manner
            currentFrame = (currentFrame + 1) % animationSprites.Count;

            // Wait for a certain amount of time based on animation rate
            yield return new WaitForSeconds(1 / animationRate);
        }
    }

    public void SpawnObject()
    {
        if (spawningObject != null && hitCount <= maxHitCount)
        {
            if (spawningObject.name == "Coin Animation")
            {
                Instantiate(spawningObject, new Vector3(transform.position.x, transform.position.y + 1.92f, 0), Quaternion.identity);
            }
            else
            {
                Instantiate(spawningObject, transform.position, Quaternion.identity);
            }
        }
    }
}