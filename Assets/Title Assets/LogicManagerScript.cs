using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.XR;

public class LogicManagerScript : MonoBehaviour
{
    public AudioScript audioScript;
    public GameObject mario;
    public GameObject titleScaleScript;
    private float timer;
    public float whenToAppear;
    private bool appeared;
    private bool allowInput;
    public string inputDevice;

    // Start is called before the first frame update
    void Start()
    {
        // Permanent variables unless game resets
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("Total Lives", 3);
        PlayerPrefs.SetInt("Total Score", 0);
        PlayerPrefs.SetInt("Total Coins", 0);
        PlayerPrefs.SetInt("Total Time", 400);
        PlayerPrefs.SetString("Current State", "normal_mario");
        PlayerPrefs.SetString("Spawn Position", "-13.3800001,-10.21");
        PlayerPrefs.Save();
        if (inputDevice == "School Arcade")
        {
            PlayerPrefs.SetString("Input Device", "School Arcade");
            PlayerPrefs.Save();
        }
        else if (inputDevice == "PS4")
        {
            PlayerPrefs.SetString("Input Device", "PS4");
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetString("Input Device", "None");
            PlayerPrefs.Save();
        }
        appeared = false;
        allowInput = false;
        titleScaleScript.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && allowInput)
        {
            PlayerPrefs.SetString("Spawn Position", "-59.01,-9.494");
            PlayerPrefs.Save();
            SceneManager.LoadSceneAsync("9-1");
        }
        timer += Time.deltaTime;

        if (timer > whenToAppear && !appeared)
        {
            titleScaleScript.SetActive(true);
            appeared = true;
        }
        if (timer > whenToAppear + 2f)
        {
            allowInput = true;
        }
    }
}
