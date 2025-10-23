using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class InterfaceManager : MonoBehaviour
{
    public GameObject GameManager;
    public GameObject GameplayObjects;
    public GameObject InformationBar;
    public GameObject SettingsPanel;
    public GameObject GameOverPanel;
    public TextMeshProUGUI gameOverScoreText;
    private static InterfaceManager _instance;
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    public void OpenSettings()
    {
        GameManager.GetComponent<GameManager>().OnPause();

        GameplayObjects.SetActive(false);
        InformationBar.SetActive(false);
        GameOverPanel.SetActive(false);

        SettingsPanel.SetActive(true);
    }

    public void ContinueGame()
    {
        GameOverPanel.SetActive(false);
        SettingsPanel.SetActive(false);

        GameplayObjects.SetActive(true);
        InformationBar.SetActive(true);

        GameManager.GetComponent<GameManager>().OnContinue();
    }

    public void RestartGame()
    {
        GameOverPanel.SetActive(false);
        SettingsPanel.SetActive(false);

        GameplayObjects.SetActive(true);
        InformationBar.SetActive(true);

        GameManager.GetComponent<GameManager>().OnRestart();
    }

    public void GoHome()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void OpenGameOver()
    {
        GameplayObjects.SetActive(false);
        InformationBar.SetActive(false);
        SettingsPanel.SetActive(false);

        gameOverScoreText.text = "You got " + GameManager.GetComponent<GameManager>().scoreText.text + " maqui points";

        GameOverPanel.SetActive(true);
    }

}
