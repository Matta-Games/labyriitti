using UnityEngine;
using TMPro; // Remove if not using TMP
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SpeedrunTimer : MonoBehaviour
{
    [Header("Timer Mode")]
    public bool countdownMode = false;
    [SerializeField] private bool startBlocker = false;
    

    [Header("Starting Time (Example: 5 minutes)")]
    [SerializeField] private float startSeconds = 5f;

    [Header("Auto Start")]
    [SerializeField] private bool startOnPlay = true;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private Text uiText;

    [Header("Removing objects")]
    [SerializeField] private List<GameObject> objectsToRemove;

    public float currentTime;
    public bool isRunning;

    public bool timerEnded = false;

    void Start()
    {
        ResetTimer();

        if (startOnPlay)
            StartTimer();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime += countdownMode ? -Time.deltaTime : Time.deltaTime;

        if (countdownMode && currentTime <= 0f)
        {
            // Timer expired -> stop and mark ended
            currentTime = 0f;
            isRunning = false;
            timerEnded = true;

            // If startBlocker is enabled, remove configured objects before reload (optional behavior)
            if (startBlocker)
            {
                Debug.Log("Timer ended. Start Blocker is active — removing configured objects before reload.");
                foreach (GameObject obj in objectsToRemove)
                {
                    if (obj != null)
                        Destroy(obj);
                }
            }

            // Reload the current scene to "reset" the level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("SpeedrunTimer: Scene reloaded because timer reached zero.");
        }

        UpdateUI();
    }

    // --------------------
    // Public Controls
    // --------------------

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = startSeconds;
        UpdateUI();
    }

    // --------------------
    // UI Formatting
    // --------------------

    void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        int milliseconds = Mathf.FloorToInt((currentTime * 100f) % 100f);

        string timeString = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);

        if (tmpText != null)
            tmpText.text = timeString;

        if (uiText != null)
            uiText.text = timeString;
    }
}
