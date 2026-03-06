using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Call this from the button
    public void PlayGame()
    {
        // Replace "GameScene" with the name of your main scene
        SceneManager.LoadScene("Mr Labtrinth");
    }
}
