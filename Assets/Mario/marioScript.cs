using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR;

public class marioScript : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    public mainCameraScript cameraScript;
    public topCollisionScript topCollisionScript;
    public bottomCollisionScript bottomCollisionScript;
    public FlagScript flagScript;
    public Sprite[] normalMarioSprites;
    public Sprite[] bigMarioSprites;
    public Sprite[] lightningMarioSprites;
    public Sprite[] smallThunderMarioSprites;
    public Sprite[] bigThunderMarioSprites;
    public Dictionary<string, int> myDictionary = new();
    private readonly Dictionary<string, Sprite[]> marioState = new();
    public GameObject lightning;
    public AudioScript mainAudio;
    public AudioSource jumpAudio;
    public AudioClip smallJump;
    public AudioClip bigJump;
    public AudioSource powerUpSound;
    public AudioSource shrinkingSound;
    public string currentState;
    private readonly float normalSpriteBoxX = 1.44f;
    private readonly float jumpingSpriteBoxX = 1.92f;
    public float JumpSpeed;
    public float MaxMoveSpeed;
    public float groundAcceleration;
    public float groundFriction;
    private bool pushingUpArrow = false;
    public float maxJumpHeight;
    private float heightBeforeJumping;
    private float timeRunning = 0;
    public float runningAnimationrate;
    private float slowAcceleration;
    private float originalMaxMoveSpeed;
    public float fastAcceleration;
    public float bigMaxSpeed;
    public bool isDead;
    public bool justDied;
    private float timeAfterDeath;
    public float respawnInterval;
    public bool startUpMario;
    private float timer;
    private bool blinking = false;
    private float blinkTimer;
    public float resetTime;
    public float winTime;
    public bool completedLevel = false;
    public string nextLevel;
    public string respawnScene;
    public string respawnPosition;
    private bool lightningKeyPressed = false;
    private bool upPressed = false;
    private readonly float joystickSensitivity = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        myDictionary["still"] = 0;
        myDictionary["jumping"] = 1;
        myDictionary["running_1"] = 2;
        myDictionary["running_2"] = 3;
        myDictionary["running_3"] = 4;
        myDictionary["drift"] = 5;
        myDictionary["dead"] = 6;
        myDictionary["duck"] = 7;
        myDictionary["climbing_1"] = 8;
        myDictionary["climbing_2"] = 9;

        slowAcceleration = groundAcceleration;
        originalMaxMoveSpeed = MaxMoveSpeed;
        justDied = false;
        timer = 0;
        blinkTimer = 0;

        marioState["normal_mario"] = normalMarioSprites;
        marioState["big_mario"] = bigMarioSprites;
        marioState["lightning_mario"] = bigMarioSprites; // Place Holder
        marioState["small_thunder_mario"] = smallThunderMarioSprites;
        marioState["big_thunder_mario"] = bigThunderMarioSprites;

        string[] spawnPos = PlayerPrefs.GetString("Spawn Position").Split(',');
        transform.position = new Vector3((int)double.Parse(spawnPos[0]), (int)double.Parse(spawnPos[1]), 0);
        SetState(PlayerPrefs.GetString("Current State"));
        if (currentState != "normal_mario" && currentState != "small_thunder_mario")
        {
            boxCollider.size = new Vector2(1.44f, 3.84f);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.96f, 0);
            GameObject.Find("Mario Top").GetComponent<BoxCollider2D>().offset = new Vector2(0, 1.922f);
            GameObject.Find("Mario Bottom").GetComponent<BoxCollider2D>().offset = new Vector2(0, -1.922f);
        }
    }

    // Update is called once per frame
    [Obsolete]
    void Update()
    {
        if (currentState == "normal_mario" || currentState == "small_thunder_mario")
        {
            if (jumpAudio.clip != smallJump)
            {
                jumpAudio.clip = smallJump;
            }
        }
        else
        {
            if (jumpAudio.clip != bigJump)
            {
                jumpAudio.clip = bigJump;
            }
        }
        if (completedLevel)
        {
            WinAnimation();
        }
        else if (!startUpMario)
        {
            if (!isDead)
            {
                // Lightning Shooting
                if (LightningKeyPressed() && (currentState == "lightning_mario" || currentState == "big_thunder_mario"))
                {
                    if (spriteRenderer.flipX)
                    {
                        Instantiate(lightning, new Vector2(transform.position.x - boxCollider.size.x / 2 - lightning.GetComponent<BoxCollider2D>().size.x - 0.05f, transform.position.y), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(lightning, new Vector2(transform.position.x + boxCollider.size.x / 2 + lightning.GetComponent<BoxCollider2D>().size.x + 0.05f, transform.position.y), Quaternion.identity);
                    }
                }

                // Move Mario based on input
                MoveMario();

                if (blinking && blinkTimer < resetTime)
                {
                    blinkTimer += Time.deltaTime;
                    if ((blinkTimer >= 0f && blinkTimer <= 0.3f) || (blinkTimer >= 0.6f && blinkTimer <= 0.9f) || (blinkTimer >= 1.2f && blinkTimer <= 1.5f))
                    {
                        spriteRenderer.sprite = null;
                    }
                    else
                    {
                        AlterMarioAnimation();
                    }

                    if (blinkTimer > resetTime)
                    {
                        blinkTimer = 0;
                        blinking = false;
                        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

                        foreach (GameObject enemy in enemyObjects)
                        {
                            if (enemy.GetComponent<BoxCollider2D>() != null)
                            {
                                Physics2D.IgnoreCollision(enemy.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>(), false);
                            }
                        }
                    }
                }
                else
                {
                    // Alter mario sprite based on running or jumping
                    AlterMarioAnimation();
                }

                // Keeping Mario in Camera view
                KeepMarioInView();

                if (transform.position.y <= -25.4f)
                {
                    OutOfBounds();
                }
            }
            else
            {
                // Dead animation
                if (justDied)
                {
                    mainAudio.PlayMarioDeadMusic();
                    if (currentState == "normal_mario")
                    {
                        SetSprite("dead");
                    }
                    boxCollider.enabled = false;
                    Destroy(transform.FindChild("Mario Top").gameObject);
                    Destroy(transform.FindChild("Mario Bottom").gameObject);
                    myRigidBody.gravityScale = 5;
                    myRigidBody.drag = 1;
                    myRigidBody.velocity = new Vector3(0, 20, 0);
                    justDied = false;
                }
                timeAfterDeath += Time.deltaTime;
                if (timeAfterDeath > respawnInterval)
                {
                    PlayerPrefs.SetInt("Total Time", 400);
                    if (PlayerPrefs.GetInt("Total Lives") == 0)
                    {
                        PlayerPrefs.SetInt("Total Lives", 3);
                        PlayerPrefs.Save();
                        SceneManager.LoadScene("Game Over");
                    }
                    else
                    {
                        SceneManager.LoadScene(respawnScene);
                        PlayerPrefs.SetString("Spawn Position", respawnPosition);
                        PlayerPrefs.Save();
                    }
                }
            }

        }
        else
        {
            StartUp();
        }

    }

    void MoveMario()
    {
        // Change fast / slow pace
        if (SprintKeyHeld())
        {
            groundAcceleration = fastAcceleration;
            MaxMoveSpeed = bigMaxSpeed;
        }
        else if (groundAcceleration != slowAcceleration)
        {
            groundAcceleration = slowAcceleration;
            MaxMoveSpeed = originalMaxMoveSpeed;
        }

        // New x and y velocity
        Vector2 newVelocity = myRigidBody.velocity;

        if (RightMovement())
        {
            spriteRenderer.flipX = false;
            if (newVelocity.x < MaxMoveSpeed)
            {
                // Calculate acceleration based on current speed
                float acceleration = Mathf.Min(55 + (newVelocity.x / MaxMoveSpeed) * (groundAcceleration - 55), groundAcceleration);
                newVelocity.x += acceleration * Time.deltaTime;
                if (newVelocity.x > MaxMoveSpeed)
                {
                    newVelocity.x = MaxMoveSpeed;
                }
            }
        }
        else if (LeftMovement())
        {
            spriteRenderer.flipX = true;
            if (newVelocity.x > -MaxMoveSpeed)
            {
                // Calculate acceleration based on current speed
                float acceleration = Mathf.Min(35 + (Mathf.Abs(newVelocity.x) / MaxMoveSpeed) * (groundAcceleration - 35), groundAcceleration);
                newVelocity.x -= acceleration * Time.deltaTime;
                if (newVelocity.x < -MaxMoveSpeed)
                {
                    newVelocity.x = -MaxMoveSpeed;
                }
            }
        }
        else
        {
            // Decelerate when no input is given
            if (newVelocity.x > 0)
            {
                newVelocity.x -= groundAcceleration * Time.deltaTime;
                if (newVelocity.x < 0)
                {
                    newVelocity.x = 0;
                }
            }
            else if (newVelocity.x < 0)
            {
                newVelocity.x += groundAcceleration * Time.deltaTime;
                if (newVelocity.x > 0)
                {
                    newVelocity.x = 0;
                }
            }
        }

        // Jump when up arrow pressed
        if (UpMovementPressed() && bottomCollisionScript.IsGrounded() && myRigidBody.velocity.y == 0)
        {
            jumpAudio.Play();
            newVelocity.y = JumpSpeed;
            pushingUpArrow = true;
            heightBeforeJumping = transform.position.y;
        }

        // Keep being able to go higher while holding up arrow until max height reached
        if (UpMovement() && pushingUpArrow)
        {
            newVelocity.y = JumpSpeed;
            if ((transform.position.y - heightBeforeJumping) >= maxJumpHeight)
            {
                pushingUpArrow = false;
            }
        }
        else
        {
            pushingUpArrow = false;
        }

        if (topCollisionScript.HeadCollided())
        {
            pushingUpArrow = false;
            newVelocity.y = 0;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
        }

        // Apply the updated velocity
        myRigidBody.velocity = newVelocity;
    }

    void AlterMarioAnimation()
    {

        // Change mario to desired Sprite
        if (bottomCollisionScript.IsGrounded())
        {
            // Change box collider size
            Vector2 currentSize = boxCollider.size;
            currentSize.x = normalSpriteBoxX;
            boxCollider.size = currentSize;
            topCollisionScript.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.44f, 0.0025f);
            bottomCollisionScript.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.44f, 0.0025f);

            if (bottomCollisionScript.boxSize != 1.44f)
            {
                bottomCollisionScript.boxSize = 1.44f;
            }
            // Mario running Sprite animation
            if ((myRigidBody.velocity.x > 0 && LeftMovement()) || (myRigidBody.velocity.x < 0 && RightMovement()))
            {
                SetSprite("drift");
            }
            else if (myRigidBody.velocity.x != 0)
            {
                // Change running image frame based on running time
                float running_frame = ((timeRunning / runningAnimationrate) % 3) + 2;
                if (((int)running_frame == 2 || (int)running_frame == 3) || (int)running_frame == 4)
                {
                    spriteRenderer.sprite = marioState[currentState][((int)running_frame)];
                }
                timeRunning += Time.deltaTime;
            }
            else
            {
                SetSprite("still");
                // Reset timeRunning
                timeRunning = 0;
            }
        }
        else
        {
            SetSprite("jumping");
            // Change box collider size
            Vector2 currentSize = boxCollider.size;
            currentSize.x = jumpingSpriteBoxX;
            boxCollider.size = currentSize;
            topCollisionScript.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.92f, 0.0025f);
            bottomCollisionScript.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.92f, 0.0025f);
        }
    }

    void KeepMarioInView()
    {
        Vector3 newTransform = transform.position;
        float border = cameraScript.transform.position.x - cameraScript.GetSize() * 16 / 12 + boxCollider.size.x / 2;
        if (transform.position.x <= border && !RightMovement())
        {
            newTransform.x = border;
            myRigidBody.velocity = new Vector3(0, myRigidBody.velocity.y, 0);
        }
        transform.position = newTransform;
    }

    public void SetSprite(string sprite)
    {
        if (spriteRenderer.sprite = marioState[currentState][myDictionary[sprite]])
        {
            spriteRenderer.sprite = marioState[currentState][myDictionary[sprite]];
        }
    }

    public void PowerUpCollected()
    {
        powerUpSound.Play();
        if (currentState == "normal_mario")
        {
            SetState("big_mario");
            boxCollider.size = new Vector2(1.44f, 3.84f);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.96f, 0);
            GameObject.Find("Mario Top").GetComponent<BoxCollider2D>().offset = new Vector2(0, 1.922f);
            GameObject.Find("Mario Bottom").GetComponent<BoxCollider2D>().offset = new Vector2(0, -1.922f);
        }
        else if (currentState == "big_mario")
        {
            SetState("lightning_mario");
        }
    }

    public void HitEnemy()
    {
        if (currentState == "normal_mario")
        {
            isDead = true;
            justDied = true;
        }
        else
        {
            shrinkingSound.Play();
            SetState("normal_mario");
            boxCollider.size = new Vector2(1.44f, 1.92f);
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.96f, 0);
            GameObject.Find("Mario Top").GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.9622f);
            GameObject.Find("Mario Bottom").GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.962f);
            blinking = true;
            GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemyObjects)
            {
                if (enemy.GetComponent<Collider2D>() != null)
                {
                    Physics2D.IgnoreCollision(enemy.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                }
            }
        }
    }

    public void OutOfBounds()
    {
        isDead = true;
        justDied = true;
        SetState("normal_mario");
    }

    public void ThunderMode()
    {
        if (currentState == "normal_mario")
        {
            SetState("small_thunder_mario");
        }
        else
        {
            SetState("big_thunder_mario");
        }
    }

    public void SetState(string state)
    {
        currentState = state;
        PlayerPrefs.SetString("Current State", state);
        PlayerPrefs.Save();
    }

    void StartUp()
    {
        timer += Time.deltaTime;
        if (timer < 1.8f)
        {
            spriteRenderer.flipX = false;
            if (myRigidBody.velocity.x <= MaxMoveSpeed - 1.5)
            {
                myRigidBody.velocity = new Vector3(myRigidBody.velocity.x + groundAcceleration * Time.deltaTime * 2, 0, 0);
            }

            // Change running image frame based on running time
            float running_frame = ((timeRunning / runningAnimationrate) % 3) + 2;
            if (((int)running_frame == 2 || (int)running_frame == 3) || (int)running_frame == 4)
            {
                spriteRenderer.sprite = marioState[currentState][((int)running_frame)];
            }
            timeRunning += Time.deltaTime;

        }
        else
        {
            SetSprite("dead");
            boxCollider.enabled = false;
            Destroy(GameObject.Find("Mario Top"));
            Destroy(GameObject.Find("Mario Bottom"));
            myRigidBody.gravityScale = 5;
            myRigidBody.drag = 1;
            myRigidBody.velocity = new Vector3(0, 20, 0);
            Destroy(gameObject.GetComponent<marioScript>());
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Flag Pole"))
        {
            mainAudio.PlayLevelCompletedMusic();
            Destroy(collision.gameObject.GetComponent<BoxCollider2D>());
            completedLevel = true;
            flagScript.enabled = true;
            transform.position = new Vector3(flagScript.gameObject.transform.position.x + 0.5142f, transform.position.y, 0);
        }
    }

    void WinAnimation()
    {
        winTime += Time.deltaTime;
        if (winTime < 2.2f)
        {
            myRigidBody.velocity = new Vector3(0, -10, 0);
            if ((winTime >= 0 && winTime <= 0.15) || (winTime >= 0.3 && winTime <= 0.45) || (winTime >= 0.6 && winTime <= 0.75) || (winTime >= 0.9 && winTime <= 1.05) || (winTime >= 1.2 && winTime <= 1.35) || (winTime >= 1.5 && winTime <= 1.65) || (winTime >= 1.8 && winTime <= 1.95) || (winTime >= 2.1 && winTime <= 2.25) || (winTime >= 2.4 && winTime <= 2.55) || (winTime >= 2.7 && winTime <= 2.85) || (winTime >= 3 && winTime <= 3))
            {
                SetSprite("climbing_1");
            }
            else
            {
                SetSprite("climbing_2");
            }
        }
        else if (winTime < 2.7)
        {
            spriteRenderer.flipX = true;
            transform.position = new Vector3(flagScript.gameObject.transform.position.x + 2.0142f, transform.position.y, 0);
        }
        else if (winTime < 2.95f)
        {
            myRigidBody.velocity = new Vector3(8, 25, 0);
            AlterMarioAnimation();
        }
        else if (winTime < 5.2f)
        {
            if (winTime >= 4f)
            {
                spriteRenderer.flipX = false;
            }
            myRigidBody.velocity = new Vector3(8, myRigidBody.velocity.y, 0);
            AlterMarioAnimation();
        }
        else
        {
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<SpriteRenderer>());
        }
        if (winTime >= 14)
        {
            PlayerPrefs.SetString("Spawn Position", "-67.37,17.07");
            PlayerPrefs.SetInt("Total Time", 400);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Play Scene");
        }
    }

    private bool LightningKeyPressed()
    {
        if (PlayerPrefs.GetString("Input Device") == "School Arcade")
        {
            return Input.GetKeyDown(KeyCode.Joystick2Button1) || Input.GetKeyDown(KeyCode.LeftShift);
        }
        else if (PlayerPrefs.GetString("Input Device") == "PS4")
        {
            bool a = Input.GetAxis("Lightning") > 0;
            bool isNewlyPressed = a && !lightningKeyPressed;
            lightningKeyPressed = a || (Input.GetKeyDown(KeyCode.LeftShift)); // Update the last state
            return (Input.GetKeyDown(KeyCode.LeftShift) || isNewlyPressed);
        }
        return Input.GetKeyDown(KeyCode.LeftShift);
    }
    private bool SprintKeyHeld()
    {
        if (PlayerPrefs.GetString("Input Device") == "School Arcade")
        {
            return Input.GetKey(KeyCode.Joystick2Button1) || Input.GetKey(KeyCode.LeftShift);
        }
        else if (PlayerPrefs.GetString("Input Device") == "PS4")
        {
            return (Input.GetKey(KeyCode.LeftShift) || Input.GetAxis("Sprint") > 0);
        }
        return Input.GetKey(KeyCode.LeftShift);
    }
    private bool RightMovement()
    {
        if (PlayerPrefs.GetString("Input Device") == "School Arcade")
        {
            return Input.GetKey(KeyCode.RightArrow) || (Input.GetAxis("Horizontal") < -joystickSensitivity);
        }
        else if (PlayerPrefs.GetString("Input Device") == "PS4")
        {
            return Input.GetKey(KeyCode.RightArrow) || (Input.GetAxis("Horizontal") > 0);
        }
        return Input.GetKey(KeyCode.RightArrow);
    }
    private bool LeftMovement()
    {
        if (PlayerPrefs.GetString("Input Device") == "School Arcade")
        {
            return Input.GetKey(KeyCode.LeftArrow) || (Input.GetAxis("Horizontal") > joystickSensitivity);
        }
        else if(PlayerPrefs.GetString("Input Device") == "PS4")
        {
            return Input.GetKey(KeyCode.LeftArrow) || (Input.GetAxis("Horizontal") < -joystickSensitivity);
        }
        return Input.GetKey(KeyCode.LeftArrow);
    }
    private bool UpMovement()
    {
        if (PlayerPrefs.GetString("Input Device") == "School Arcade")
        {
            return Input.GetKey(KeyCode.UpArrow) || (Input.GetKey(KeyCode.Joystick2Button0));
        }
        else if (PlayerPrefs.GetString("Input Device") == "PS4")
        {
            return Input.GetKey(KeyCode.UpArrow) || (Input.GetAxis("Jump") > 0);
        }
        return Input.GetKey(KeyCode.UpArrow);
    }
    private bool UpMovementPressed()
    {
        if (PlayerPrefs.GetString("Input Device") == "School Arcade")
        {
            return Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetKeyDown(KeyCode.Joystick2Button0));
        }
        else if (PlayerPrefs.GetString("Input Device") == "PS4")
        {
            float currentUpAxis = Input.GetAxis("Jump");
            bool isNewlyPressed = currentUpAxis > 0 && !upPressed;
            upPressed = currentUpAxis > 0 || Input.GetKeyDown(KeyCode.UpArrow); // Update the last state
            return isNewlyPressed || Input.GetKeyDown(KeyCode.UpArrow);
        }
        return Input.GetKeyDown(KeyCode.UpArrow);
    }
}