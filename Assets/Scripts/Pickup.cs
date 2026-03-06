using UnityEngine;
using TMPro;

public class CollectibleItem : MonoBehaviour
{
    [Header("Gameplay")]
    public int points = 10;                  // Points this item gives

    [Tooltip("Optional identifier for this item. If empty, the GameObject's name (without '(Clone)') is used.")]
    public string itemId = "";

    [Header("UI")]
    public TMP_Text scoreText;               // Reference to your TMP text
    private int currentScore = 0;            // Tracks total score

    [Header("Audio")]
    [Tooltip("Sound played when the item is collected. If null, no sound is played.")]
    public AudioClip pickupSfx;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private void Start()
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            // Use the GameObject name as fallback, strip common "(Clone)" suffix
            itemId = gameObject.name.Replace("(Clone)", "").Trim();
        }

        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))       // Make sure collider is Player
            return;

        // Update score
        currentScore += points;
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;

        // Play pickup sound (use PlayClipAtPoint so sound plays even if the world item is deactivated/destroyed)
        if (pickupSfx != null)
        {
            var playPosition = Camera.main != null ? Camera.main.transform.position : transform.position;
            AudioSource.PlayClipAtPoint(pickupSfx, playPosition, sfxVolume);
        }

        // Notify ItemManager using the itemId (name-based matching)
        var manager = ItemManager.Instance;
        if (manager != null)
        {
            manager.CollectByName(itemId, gameObject);
        }
        else
        {
            // Fallback behavior: remove the item from the scene
            Destroy(gameObject);
        }
    }
}
