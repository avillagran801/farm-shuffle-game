using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public AudioClip selectSound;

    public void OnEffectsSliderValueChanged(float value)
    {
        SoundManager.Instance.ChangeEffectsVolume(value);
        SoundManager.Instance.PlayEffect(selectSound);
    }

    public void OnMusicSliderValueChanged(float value)
    {
        SoundManager.Instance.ChangeMusicVolume(value);
    }

    public void GoHome()
    {
        SoundManager.Instance.PlayEffect(selectSound);
        // Scene 0: Main Menu
        SceneManager.LoadSceneAsync(0);
    }
}
