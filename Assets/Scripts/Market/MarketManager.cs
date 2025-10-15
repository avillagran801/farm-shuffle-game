using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MarketManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int score;
    private SaveSystem saveSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        saveSystem = gameObject.AddComponent<SaveSystem>();
        LoadGame();

        UpdateScoreText();
    }

    void LoadGame()
    {
        GameData data = saveSystem.LoadData();
        score = data.totalScore;
    }

    void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    void BuyPack()
    {

    }

    public void GoHome()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
