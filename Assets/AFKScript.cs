using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AFKScript : MonoBehaviour
{
    private float lastInputTime;
    private float timer;
    public float afkTimeLimit;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        lastInputTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if ( (timer - lastInputTime > afkTimeLimit) || ( Input.GetAxis("Quit") != 0) )
        {
            SceneManager.LoadScene("Play Scene");
        }

        if (Input.anyKey || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            lastInputTime = timer;
        }
    }
}
