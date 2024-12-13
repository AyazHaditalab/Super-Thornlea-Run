using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class SpawnedScoreScript : MonoBehaviour
{
    private float timer;
    private float floatingSpeed;
    private float floatTime;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        floatTime = 120;
        floatingSpeed = 0.03f;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.8f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        timer++;
        if (timer < 75)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + floatingSpeed, 0);
        }
        if (timer > floatTime)
        {
            Destroy(gameObject);
            Destroy(this);
        }
    }
}