using UnityEngine;
using TMPro;

public class CollectibleItem : MonoBehaviour
{
    public int points = 10;                  // Points this item gives
    public TMP_Text scoreText;               // Reference to your TMP text
    private int currentScore = 0;            // Tracks total score

    private void Start()
    {
        if(scoreText != null)
            scoreText.text = "Score: " + currentScore;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))       // Make sure player has the tag "Player"
        {
            currentScore += points;
            if(scoreText != null)
                scoreText.text = "Score: " + currentScore;

            Destroy(gameObject);            // Remove the item from the scene
        }
    }
}
