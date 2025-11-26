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
    private float animationInterval = 4f; // 2.5f;
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

    public void OnPause()
    {
        SoundManager.Instance.StopGameMusic();
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
        spawner.SpawnItems();

        score = 0;
        UpdateScoreText();

        remainingTime = startingTime;
        UpdateCountdownText();
    }

    public void OnGameOver()
    {
        SoundManager.Instance.StopGameMusic();
        SoundManager.Instance.PlayGameOverEffect();

        // When the time is over, save the score and open the Game Over screen
        remainingTime = 0f;
        isPlaying = false;
        SaveScore();

        interfaceManager.GetComponent<InterfaceManager>().OpenGameOver();
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

        leftClickedItem.SetBorder(false);
        rightClickedItem.SetBorder(false);

        leftClickedItem = null;
        rightClickedItem = null;
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

        leftClickedItem.SetBorder(false);
        rightClickedItem.SetBorder(false);

        leftClickedItem = null;
        rightClickedItem = null;
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
