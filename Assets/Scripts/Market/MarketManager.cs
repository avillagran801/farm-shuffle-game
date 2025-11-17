using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MarketManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public GameObject leftIconRender;
    public GameObject centerIconRender;
    public GameObject rightIconRender;
    public TextMeshProUGUI priceText;

    public GameObject price;
    public GameObject circleIcon;
    public GameObject checkIcon;
    public GameObject buyButton;

    private int score;
    private IconPack[] iconPacks;
    private int index = 0;

    void Awake()
    {
        score = DataManager.Instance.userData.totalScore;
        iconPacks = DataManager.Instance.allIconPacks;

        UpdateScoreText();
        SetPreviewIcon();
    }

    void SetPreviewIcon()
    {
        leftIconRender.GetComponent<PackRender>().SetDesign(iconPacks[(index - 1 + iconPacks.Length) % iconPacks.Length].getPreviewIcon());
        centerIconRender.GetComponent<PackRender>().SetDesign(iconPacks[index].getPreviewIcon());
        rightIconRender.GetComponent<PackRender>().SetDesign(iconPacks[(index + 1) % iconPacks.Length].getPreviewIcon());

        priceText.text = iconPacks[index].price.ToString();

        SetBoughtIcon(DataManager.Instance.userData.boughtIconPacks[index]);
    }

    void SetBoughtIcon(bool bought)
    {
        price.SetActive(!bought);
        circleIcon.SetActive(!bought);
        buyButton.GetComponent<Button>().interactable = !bought;

        checkIcon.SetActive(bought);
    }

    void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    public void leftClick()
    {
        index = (index - 1 + iconPacks.Length) % iconPacks.Length;
        SetPreviewIcon();
    }

    public void rightClick()
    {
        index = (index + 1) % iconPacks.Length;
        SetPreviewIcon();
    }

    public void BuyPack()
    {
        if ((DataManager.Instance.userData.boughtIconPacks[index] == false) && (score >= iconPacks[index].price))
        {
            DataManager.Instance.userData.boughtIconPacks[index] = true;
            DataManager.Instance.userData.totalScore -= iconPacks[index].price;

            DataManager.Instance.SaveUserData();

            score = DataManager.Instance.userData.totalScore;

            UpdateScoreText();
            SetBoughtIcon(true);
        }
    }

    public void GoHome()
    {
        // Scene 0: Main Menu
        SceneManager.LoadSceneAsync(0);
    }
}
