using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarpLockerScript : MonoBehaviour
{
    public GameObject player;
    public BoxCollider2D myBoxCollider;
    public bool warping;
    public float warpSpeed;
    public AudioScript warpAudio;
    public string warpDirection;
    public TimeManagerScript timeScript;
    public string warpLocation;
    public string warpExitPosition;

    // Start is called before the first frame update
    void Start()
    {
        warping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (warping)
        {
            if (warpDirection == "down")
            {
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - warpSpeed, 0);
                if (player.transform.position.y < transform.position.y)
                {
                    warping = false;
                    PlayerPrefs.SetString("Spawn Position", warpExitPosition);
                    PlayerPrefs.Save();
                    SceneManager.LoadScene(warpLocation);

                }
            }
            else if (warpDirection == "right")
            {
                player.transform.position = new Vector3(player.transform.position.x + warpSpeed, player.transform.position.y, 0);
                if (player.transform.position.x > transform.position.x)
                {
                    warping = false;
                    PlayerPrefs.SetString("Spawn Position", warpExitPosition);
                    PlayerPrefs.Save();
                    SceneManager.LoadScene(warpLocation);


                }
            }
        }
    }

    private void OnCollisionStay2D(UnityEngine.Collision2D collision)
    {
        if ((warpDirection == "down" && DownMovement() && Mathf.Abs(player.transform.position.x - transform.position.x) <= 0.4f) || (warpDirection == "right" && RightMovement() && transform.position.x - myBoxCollider.size.y/2 >= player.transform.position.x))
        {
            Destroy(myBoxCollider);
            player.GetComponent<marioScript>().SetSprite("still");
            Destroy(player.GetComponent<marioScript>());
            Destroy(player.GetComponent<BoxCollider2D>());
            Destroy(player.GetComponent<Rigidbody2D>());
            foreach (Transform child in player.transform)
            {
                Destroy(child.gameObject);
            }
            warpAudio.PlayWarpingAudio();
            warping = true;
            timeScript.allowCount = false;
            PlayerPrefs.SetInt("Total Time", timeScript.timeLeft);
            PlayerPrefs.Save();

        }
    }

    private bool DownMovement()
    {
        if (PlayerPrefs.GetString("Input Device") == "School Arcade")
        {
            return Input.GetKey(KeyCode.DownArrow) || Mathf.Abs(Input.GetAxis("Vertical")) >= 0.3;
        }
        else if (PlayerPrefs.GetString("Input Device") == "PS4")
        {
            return Input.GetKey(KeyCode.DownArrow) || Mathf.Abs(Input.GetAxis("Vertical")) >= 0.3;
        }
        return Input.GetKey(KeyCode.DownArrow);

    }
    private bool RightMovement()
    {
        if (PlayerPrefs.GetString("Input Device") == "School Arcade")
        {
            return Input.GetKey(KeyCode.RightArrow) || (Input.GetAxis("Horizontal") < 0.3);
        }
        else if (PlayerPrefs.GetString("Input Device") == "PS4")
        {
            return Input.GetKey(KeyCode.RightArrow) || (Input.GetAxis("Horizontal") > 0.3);
        }
        return Input.GetKey(KeyCode.RightArrow);
    }
}
