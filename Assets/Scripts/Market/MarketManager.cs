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
    public GameObject equipButton;
    public TextMeshProUGUI equipText;
    private int score;
    private IconPack[] iconPacks;
    private int index = 0;

    void Start()
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

        SetBought(DataManager.Instance.userData.boughtIconPacks[index]);
        SetEquiped(DataManager.Instance.userData.equipedIconPacks[index]);
    }

    void SetBought(bool bought)
    {
        price.SetActive(!bought);
        circleIcon.SetActive(!bought);

        checkIcon.SetActive(bought);

        buyButton.SetActive(!bought);
    }

    void SetEquiped(bool equiped)
    {
        // equipButton.SetActive(equiped);

        if (index == 0)
        {
            equipText.text = "---";
            equipButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            equipText.text = equiped ? "Unequip" : "Equip";
            equipButton.GetComponent<Button>().interactable = true;
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    public void leftClick()
    {
        SoundManager.Instance.PlaySelectEffect();
        index = (index - 1 + iconPacks.Length) % iconPacks.Length;
        SetPreviewIcon();
    }

    public void rightClick()
    {
        SoundManager.Instance.PlaySelectEffect();
        index = (index + 1) % iconPacks.Length;
        SetPreviewIcon();
    }

    public void BuyPack()
    {
        if ((DataManager.Instance.userData.boughtIconPacks[index] == false) && (score >= iconPacks[index].price))
        {
            SoundManager.Instance.PlayBuyEffect();

            DataManager.Instance.userData.boughtIconPacks[index] = true;
            DataManager.Instance.userData.totalScore -= iconPacks[index].price;

            DataManager.Instance.SaveUserData();

            score = DataManager.Instance.userData.totalScore;

            UpdateScoreText();
            SetBought(true);
            EquipPack();
            SetEquiped(true);
        }
        else
        {
            SoundManager.Instance.PlayIncorrectEffect();
        }
    }

    public void EquipPack()
    {
        if (DataManager.Instance.userData.boughtIconPacks[index] == true)
        {
            SoundManager.Instance.PlaySelectEffect();

            bool isEquiped = DataManager.Instance.userData.equipedIconPacks[index];

            SetEquiped(!isEquiped);
            DataManager.Instance.userData.equipedIconPacks[index] = !isEquiped;

            DataManager.Instance.SaveUserData();
        }
    }

    public void GoHome()
    {
        SoundManager.Instance.PlaySelectEffect();
        // Scene 0: Main Menu
        SceneManager.LoadSceneAsync(0);
    }
}
