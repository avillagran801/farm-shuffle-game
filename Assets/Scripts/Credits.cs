using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditPage
{
    public string description;

    public CreditPage(string d)
    {
        description = d;
    }
}

public class Credits : MonoBehaviour
{
    public TextMeshProUGUI descriptionText;
    public GameObject nextButton;
    public GameObject backButton;
    public GameObject homeButton;
    public AudioClip selectSound;
    private List<CreditPage> creditPages = new List<CreditPage>();
    private int currentIndex = 0;

    void Start()
    {
        creditPages.Add(new CreditPage(
            "Game developed by Ana Banana (avillagran801 on Github)\n\n" +
            "Sprout Lands UI Pack by Cup Nooble on itch.io\n\n" +
            "Hana Caraka Series by Otterisk on itch.io"
        ));

        creditPages.Add(new CreditPage(
            "Sound effects made with bfxr.net\n\n"
        ));

        backButton.GetComponent<Button>().interactable = false;
        ShowPage(0);
    }

    void ShowPage(int index)
    {
        if (index < 0 || index >= creditPages.Count)
        {
            return;
        }

        descriptionText.text = creditPages[index].description;
    }

    public void GoNext()
    {
        if (currentIndex < creditPages.Count - 1)
        {
            SoundManager.Instance.PlayEffect(selectSound);

            currentIndex++;
            ShowPage(currentIndex);

            if (currentIndex == 1)
            {
                backButton.GetComponent<Button>().interactable = true;
            }
        }

        if (currentIndex == creditPages.Count - 1)
        {
            nextButton.GetComponent<Button>().interactable = false;
        }
    }

    public void GoBack()
    {
        if (currentIndex > 0)
        {
            SoundManager.Instance.PlayEffect(selectSound);

            currentIndex--;
            ShowPage(currentIndex);

            if (currentIndex == 0)
            {
                backButton.GetComponent<Button>().interactable = false;
            }
        }

        if (currentIndex == creditPages.Count - 2)
        {
            nextButton.GetComponent<Button>().interactable = true;
        }
    }

    public void GoHome()
    {
        SoundManager.Instance.PlayEffect(selectSound);

        SceneManager.LoadSceneAsync(0);
    }


}
