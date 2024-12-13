using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsManagerScript : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    private int totalCoins;

    // Start is called before the first frame update
    void Start()
    {
        totalCoins = PlayerPrefs.GetInt("Total Coins");
    }

    // Update is called once per frame
    void Update()
    {
        coinsText.text = $"coins\n{totalCoins}";

    }

    public void CoinObtained()
    {
        totalCoins += 1;
        PlayerPrefs.SetInt("Total Coins", totalCoins);
    }

}
