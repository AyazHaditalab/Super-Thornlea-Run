using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManagerScript : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int playerScore;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        playerScore = PlayerPrefs.GetInt("Total Score");
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = $"score\n{playerScore}";
    }

    public void AddScore(int score)
    {
        playerScore += score;
        PlayerPrefs.SetInt("Total Score", PlayerPrefs.GetInt("Total Score") + score);
    }

    public void PlayAudio()
    {
        audioSource.Play();
    }
}
