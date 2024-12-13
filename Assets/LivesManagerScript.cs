using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class LivesManagerScript : MonoBehaviour
{
    public marioScript playerScript;
    public TextMeshProUGUI livesText;
    private bool justDied;

    // Start is called before the first frame update
    void Start()
    {
        int myVariable = PlayerPrefs.GetInt("Total Lives");
        livesText.text = $"lives\n{myVariable}";
        justDied = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript.isDead && justDied)
        {
            int myVariable = PlayerPrefs.GetInt("Total Lives");
            myVariable--;  // Decrease the local variable
            PlayerPrefs.SetInt("Total Lives", myVariable);  // Update PlayerPrefs
            PlayerPrefs.Save();
            livesText.text = $"lives\n{myVariable}";  // Set the text with the updated value
            justDied = false;
        }
    }
}
