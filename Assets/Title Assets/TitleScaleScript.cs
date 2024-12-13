using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScript : MonoBehaviour
{
    public float initialScale;
    private float finalScale;
    private float count;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        finalScale = transform.localScale.x;
        count = 0;
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            transform.localScale = new Vector3(initialScale - count, initialScale - count, 0);
            if (count >= initialScale - finalScale)
            {
                transform.localScale = new Vector3(finalScale, finalScale, 0);
                active = false;
            }
            count += 0.4f;

        }
    }
}
