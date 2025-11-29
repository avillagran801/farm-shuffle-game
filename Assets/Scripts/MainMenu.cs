using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        SoundManager.Instance.PlayMenuMusic();
    }
    public void PlayGame()
    {
        SoundManager.Instance.PlaySelectEffect();

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
        SoundManager.Instance.PlaySelectEffect();
        // Scene 3: Market
        SceneManager.LoadSceneAsync(3);
    }

    public void Credits()
    {
        SoundManager.Instance.PlaySelectEffect();
        // Scene 4: Credits
        SceneManager.LoadSceneAsync(4);
    }

    public void Settings()
    {
        SoundManager.Instance.PlaySelectEffect();
        // Scene 5: Settings
        SceneManager.LoadSceneAsync(5);
    }

    public void Tutorial()
    {
        SoundManager.Instance.PlaySelectEffect();
        // Scene 1: Settings
        SceneManager.LoadSceneAsync(1);
    }

    public void Records()
    {
        SoundManager.Instance.PlaySelectEffect();
        // Scene 6: Records
        SceneManager.LoadSceneAsync(6);
    }
}
