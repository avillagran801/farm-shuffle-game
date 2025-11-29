using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ItemSpawner spawner;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI countdownText;
    public GameObject leftSlot;
    public GameObject rightSlot;
    public GameObject interfaceManager;
    private ClickeableItem leftClickedItem;
    private ClickeableItem rightClickedItem;
    private bool isPlaying = true;
    public float baseAnimationInterval = 4f;
    public float minAnimationInterval = 0.8f;
    public float speedPerPoint = 0.04f;
    private float animationInterval;
    private float animationTimer = 0f;
    private int score = 0;
    private float startingTime = 20f;
    private float addTime = 3f;
    private float minusTime = 1f;
    private float maxTime = 60f;
    private float remainingTime;

    void Start()
    {
        SoundManager.Instance.PlayGameMusic();

        // Initialize and spawn items on each slot
        spawner.InitializeItems(leftSlot.GetComponent<Slot>(), rightSlot.GetComponent<Slot>());
        spawner.SpawnItems();

        // Update the text to score 0 and the start time for the countdown (10 seconds in this case)
        UpdateScoreText();

        remainingTime = startingTime;
        animationInterval = baseAnimationInterval;
        UpdateCountdownText();
    }

    void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        // While the game is running and there's still time, update the countdown on screen and
        // spawn the items on different positions inside their assigned slot
        remainingTime -= Time.deltaTime;
        animationTimer += Time.deltaTime;

        if (remainingTime <= 0f)
        {
            OnGameOver();
        }
        else
        {
            if (animationInterval <= animationTimer)
            {
                animationTimer = 0f;
                spawner.ChangeItemsPosition();
            }
        }

        UpdateCountdownText();
    }

    void SaveScore()
    {
        // Save the score inside the JSON file for persistent data
        DataManager.Instance.userData.totalScore += score;
        DataManager.Instance.SaveUserData();
    }

    bool SaveHighScore()
    {
        ScoreEntry entry = new ScoreEntry
        {
            score = score,
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        DataManager.Instance.userData.topScores.Add(entry);

        DataManager.Instance.userData.topScores = DataManager.Instance.userData.topScores
        .OrderByDescending(e => e.score)
        .ToList();

        bool isHighScore = DataManager.Instance.userData.topScores.IndexOf(entry) < 5;

        if (!isHighScore)
        {
            DataManager.Instance.userData.topScores.Remove(entry);
            return false;
        }

        if (DataManager.Instance.userData.topScores.Count > 5)
        {
            DataManager.Instance.userData.topScores = DataManager.Instance.userData.topScores.Take(5).ToList();
        }

        DataManager.Instance.SaveUserData();
        return true;
    }

    public void OnPause()
    {
        SoundManager.Instance.PlayMenuMusic();
        isPlaying = false;
    }

    public void OnContinue()
    {
        SoundManager.Instance.PlayGameMusic();
        isPlaying = true;
    }

    public void OnRestart()
    {
        SoundManager.Instance.PlayGameMusic();

        // Restart the items positions, score and timer just like in the start function
        isPlaying = true;

        RestartSelectedItems();
        spawner.SpawnItems();

        score = 0;
        UpdateScoreText();

        remainingTime = startingTime;
        UpdateCountdownText();
    }

    public void OnGameOver()
    {
        SoundManager.Instance.PlayMenuMusic();
        SoundManager.Instance.PlayGameOverEffect();

        // When the time is over, save the score and open the Game Over screen
        remainingTime = 0f;
        isPlaying = false;
        bool isHighScore = false;
        if (score > 0)
        {
            isHighScore = SaveHighScore();
            SaveScore();
        }

        interfaceManager.GetComponent<InterfaceManager>().OpenGameOver(isHighScore);
    }

    void UpdateCountdownText()
    {
        // It updates the UI text component that shows the remaining time on screen
        int seconds = Mathf.FloorToInt(remainingTime);
        int milliseconds = Mathf.FloorToInt((remainingTime - seconds) * 100);
        countdownText.text = string.Format("{0:00}.{1:00}", seconds, milliseconds);
    }

    void UpdateScoreText()
    {
        // It updates the UI text component that shows the score on screen
        scoreText.text = score.ToString();
    }

    void AddPoints()
    {
        SoundManager.Instance.VibrateMedium();
        SoundManager.Instance.PlayScoreEffect();

        // Add 1 point to the score and 5 seconds to the remaining time
        score += 1;
        UpdateScoreText();

        // Prevent animation bug
        animationTimer = 0f;

        // Max remaining time is 60 seconds
        if (remainingTime + addTime < maxTime)
        {
            remainingTime += addTime;
        }
        else
        {
            remainingTime = maxTime;
        }

        UpdateGameDificulty();
        RestartSelectedItems();
    }

    void IncorrectPair()
    {
        SoundManager.Instance.VibrateStrong();
        SoundManager.Instance.PlayIncorrectEffect();

        if (remainingTime - minusTime > 0f)
        {
            remainingTime -= minusTime;
        }
        else
        {
            remainingTime = 0f;
        }

        RestartSelectedItems();
    }

    void RestartSelectedItems()
    {
        if (leftClickedItem != null)
        {
            leftClickedItem.SetBorder(false);
            leftClickedItem = null;
        }
        if (rightClickedItem != null)
        {
            rightClickedItem.SetBorder(false);
            rightClickedItem = null;
        }
    }

    void UpdateGameDificulty()
    {
        float speedMultiplier = 1f + (score * speedPerPoint);

        animationInterval = baseAnimationInterval / speedMultiplier;

        animationInterval = Mathf.Max(animationInterval, minAnimationInterval);
    }

    public void OnItemClicked(ClickeableItem clickedItem)
    {
        // Detects and saves the last clicked item on the left slot
        if (clickedItem.GetAssignedSlot() == 0)
        {
            if (leftClickedItem != null)
            {
                leftClickedItem.SetBorder(false);
            }

            leftClickedItem = clickedItem;
            leftClickedItem.SetBorder(true);
        }
        // Detects and saves the last clicked item on the right slot
        else
        {
            if (rightClickedItem != null)
            {
                rightClickedItem.SetBorder(false);
            }

            rightClickedItem = clickedItem;
            rightClickedItem.SetBorder(true);
        }

        // If there are items clicked on both sides, it checks if there's a match
        if (leftClickedItem != null && rightClickedItem != null)
        {
            if (leftClickedItem.GetPairValue() && rightClickedItem.GetPairValue())
            {
                AddPoints();
                spawner.SpawnItems();
            }
            else
            {
                IncorrectPair();
            }
        }
    }
}
