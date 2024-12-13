using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayScreenScript : MonoBehaviour
{
    private float timer = 0;
    public float blinkSpeed;
    public SpriteRenderer spriteRenderer;
    private Sprite arrowSprite;

    void Start()
    {
        arrowSprite = spriteRenderer.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("Home Page");
        }

        timer += Time.deltaTime;

        if (timer > blinkSpeed)
        {
            timer = 0;
            if (spriteRenderer.sprite != null)
            {
                spriteRenderer.sprite = null;
            }
            else
            {
                spriteRenderer.sprite = arrowSprite;
            }
        }
    }
}
