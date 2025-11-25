using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip selectSound;
    public void PlayGame()
    {
        SoundManager.Instance.PlayEffect(selectSound);

        // Run tutorial if tutorial == true
        if (DataManager.Instance.userData.tutorial)
        {
            // Scene 1: Tutorial
            SceneManager.LoadSceneAsync(1);
        }
        else
        {
            // Scene 2: Gameplay
            SceneManager.LoadSceneAsync(2);
        }
    }

    public void Market()
    {
        SoundManager.Instance.PlayEffect(selectSound);
        // Scene 3: Market
        SceneManager.LoadSceneAsync(3);
    }

    public void Credits()
    {
        SoundManager.Instance.PlayEffect(selectSound);
        // Scene 4: Credits
        SceneManager.LoadSceneAsync(4);
    }

    public void Settings()
    {
        SoundManager.Instance.PlayEffect(selectSound);
        // Scene 5: Settings
        SceneManager.LoadSceneAsync(5);
    }
}
