using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBreakScript : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public Vector3 initialVelocity;
    public float killPoint;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody.velocity = initialVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < killPoint)
        {
            Destroy(gameObject);
            Destroy(transform.parent.gameObject);
        }
    }
}
