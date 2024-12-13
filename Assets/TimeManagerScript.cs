using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManagerScript : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public marioScript marioScript;
    public int timeLeft;
    public float countInterval;
    private float timePast;
    private bool timeRunOut;
    public bool justDied;
    public bool allowCount;
    public ScoreManagerScript scoreManagerScript;
    private float countTimer;
    public float soundInterval;

    // Start is called before the first frame update
    void Start()
    {
        timePast = 0;
        timeRunOut = false;
        justDied = false;
        allowCount = true;
        countTimer = 0;
        timeLeft = PlayerPrefs.GetInt("Total Time");
    }

    // Update is called once per frame
    void Update()
    {
        if (allowCount)
        {
            timePast += Time.deltaTime;
            if (timeLeft == 0 && !justDied)
            {
                marioScript.SetState("normal_mario");
                marioScript.isDead = true;
                marioScript.justDied = true;
                justDied = true;
            }
            if (marioScript.completedLevel)
            {
                allowCount = false;
            }
            if (timePast >= countInterval && timeLeft > 0)
            {
                timePast = 0;
                timerText.text = $"time\n{--timeLeft}";
            }

        }
        else if (marioScript != null)
        {
            if (marioScript.gameObject.GetComponent<SpriteRenderer>() == null)
            {
                countTimer += Time.deltaTime;
                if (timeLeft == 1)
                {
                    timerText.text = "0";
                    Destroy(this);
                }

                timerText.text = $"time\n{--timeLeft}";
                scoreManagerScript.AddScore(50);
                if (countTimer >= soundInterval)
                {
                    scoreManagerScript.PlayAudio();
                    countTimer = 0;
                }
            }
        }
    }

    public bool TimeRunOut()
    {
        return timeRunOut;
    }
}
