using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ItemSpawner spawner;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI countdownText;
    public GameObject leftSlot;
    public GameObject rightSlot;
    private ClickeableItem leftClickedItem;
    private ClickeableItem rightClickedItem;
    private bool isPlaying = true;
    private float animationInterval = 2.5f;
    private float animationTimer = 0f;
    private int score = 0;
    private float startingTime = 30f;
    private float remainingTime;
    private SaveSystem saveSystem;

    void Awake()
    {
        saveSystem = gameObject.AddComponent<SaveSystem>();
        LoadGame();
    }

    void LoadGame()
    {
        GameData data = saveSystem.LoadData();
        // RELLENAR
    }

    void SaveScore()
    {
        GameData prevData = saveSystem.LoadData();
        GameData data = new GameData();
        data.totalScore = prevData.totalScore + score;
        saveSystem.SaveData(data);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawner.InitializeItems(leftSlot.GetComponent<Slot>(), rightSlot.GetComponent<Slot>());
        spawner.SpawnItems();

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

        remainingTime -= Time.deltaTime;
        animationTimer += Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isPlaying = false;
            SaveScore();
            Debug.Log("Game finished!");
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

    public void OnSettings()
    {
        isPlaying = false;
    }

    void UpdateCountdownText()
    {
        int seconds = Mathf.FloorToInt(remainingTime);
        int milliseconds = Mathf.FloorToInt((remainingTime - seconds) * 100);
        countdownText.text = string.Format("{0:00}.{1:00}", seconds, milliseconds);
    }

    void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    void AddPoints()
    {
        score += 1;
        remainingTime += 5f;
        UpdateScoreText();
    }

    public void OnItemClicked(ClickeableItem clickedItem)
    {
        if (clickedItem.GetAssignedSlot() == 0)
        {
            if (leftClickedItem != null)
            {
                leftClickedItem.SetBorder(false);
            }

            leftClickedItem = clickedItem;
            leftClickedItem.SetBorder(true);
        }
        else
        {
            if (rightClickedItem != null)
            {
                rightClickedItem.SetBorder(false);
            }

            rightClickedItem = clickedItem;
            rightClickedItem.SetBorder(true);
        }

        if (leftClickedItem != null && rightClickedItem != null)
        {
            if (leftClickedItem.GetPairValue() && rightClickedItem.GetPairValue())
            {
                AddPoints();

                leftClickedItem = null;
                rightClickedItem = null;
                spawner.SpawnItems();
            }
        }
    }
}
