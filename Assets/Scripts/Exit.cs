using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Exit : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Sound played when exit is triggered.")]
    [SerializeField] private AudioClip exitSfx;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    [Header("Exit / Scene")]
    [Tooltip("Name of the menu scene to load (must be added to Build Settings).")]
    [SerializeField] private string menuSceneName = "Menu";
    [Tooltip("Delay in seconds before switching to the menu. Measured in real time (ignores Time.timeScale).")]
    [SerializeField] private float delaySeconds = 2f;

    [Header("Behavior")]
    [Tooltip("If true, the game will be frozen (Time.timeScale = 0) immediately after triggering.")]
    [SerializeField] private bool freezeOnExit = true;

    private bool exiting = false;

    /// <summary>
    /// Call this to trigger the exit sequence: play SFX, optionally freeze game, wait unscaled time, then load the menu.
    /// </summary>
    public void TriggerExit()
    {
        if (exiting) return;
        exiting = true;

        // Play sound at camera position so it continues when objects are deactivated/destroyed.
        if (exitSfx != null)
        {
            Vector3 playPos = Camera.main != null ? Camera.main.transform.position : transform.position;
            AudioSource.PlayClipAtPoint(exitSfx, playPos, sfxVolume);
        }

        if (freezeOnExit)
            Time.timeScale = 0f;

        StartCoroutine(ExitCoroutine());
    }

    private IEnumerator ExitCoroutine()
    {
        // Wait using real time so wait is not affected by Time.timeScale = 0
        yield return new WaitForSecondsRealtime(delaySeconds);

        // Restore time scale before loading the menu so the new scene runs normally.
        Time.timeScale = 1f;

        if (string.IsNullOrWhiteSpace(menuSceneName))
        {
            Debug.LogWarning("Exit: menuSceneName is empty. Aborting load.");
            exiting = false;
            yield break;
        }

        SceneManager.LoadScene(menuSceneName);
    }

    // Trigger the exit when the player enters this trigger collider (2D).
    // Ensure the Exit GameObject has a Collider2D with "Is Trigger" checked and the player has the "Player" tag.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (exiting) return;
        if (other.CompareTag("Player"))
        {
            TriggerExit();
        }
    }
}
