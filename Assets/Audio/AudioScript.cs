using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip mainMusic;
    public AudioClip levelCompletedMusic;
    public AudioClip marioDieMusic;
    public AudioClip warpAudio;

    public void PlayMarioDeadMusic()
    {
        audioSource.clip = marioDieMusic;
        audioSource.Play();
        audioSource.loop = false;
    }

    public void PlayLevelCompletedMusic()
    {
        audioSource.volume = 1;
        audioSource.clip = levelCompletedMusic;
        audioSource.Play();
        audioSource.loop = false;
    }

    public void PlayWarpingAudio()
    {
        audioSource.clip = warpAudio;
        audioSource.Play();
        audioSource.loop = false;

    }
}
