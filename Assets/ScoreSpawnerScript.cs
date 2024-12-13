using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSpawnerScript : MonoBehaviour
{
    public GameObject scoreObtainedText;
    public Canvas canvas;
    public GameObject mainCamera;

    public void SpawnScore(Vector3 scenePosition, string score)
    {
        // Convert world space position to screen space
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, scenePosition);

        // Convert screen space position to canvas space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, null, out Vector2 canvasPosition);

        // Instantiate and position the text
        GameObject spawnedText = Instantiate(scoreObtainedText);
        spawnedText.transform.SetParent(canvas.transform, false);
        spawnedText.AddComponent<SpawnedScoreScript>();
        spawnedText.GetComponent<RectTransform>().anchoredPosition = canvasPosition;
        spawnedText.GetComponent<TextMeshProUGUI>().text = score;
    }
}
