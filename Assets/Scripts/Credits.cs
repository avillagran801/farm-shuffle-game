using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditPage
{
    public string title;
    public string description;

    public CreditPage(string t, string d)
    {
        title = t;
        description = d;
    }
}

public class Credits : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public GameObject nextButton;
    public GameObject backButton;
    public GameObject homeButton;
    private List<CreditPage> creditPages = new List<CreditPage>();
    private int currentIndex = 0;

    void Start()
    {
        creditPages.Add(new CreditPage(
            "Game development",
            "Game developed by Ana Villagran (avillagran801 on GitHub) for the UdeC Mobile Game Development course of 2025-2\n\n" +
            "Based on the card game Dobble®"
        ));

        creditPages.Add(new CreditPage(
            "Special thanks",
            "To my teachers Erich Gruttner and Fabian Cid for their advice and technical knowledge\n\n" +
            "To my boyfriend Bastian Cabrera for all his support\n\n" +
            "And to my dear classmates for all the laughs"
        ));

        creditPages.Add(new CreditPage(
            "Sprites and font credits",
            "Sprout Lands UI Pack by Cup Nooble\n\n" +
            "Found on cupnooble.itch.io/sprout-lands-ui-pack"
        ));

        creditPages.Add(new CreditPage(
            "Sprites credits",
            "Hana Caraka Series by Otterisk\n\n" +
            "Found on itch.io/c/3569702/hana-caraka-series"
        ));

        creditPages.Add(new CreditPage(
            "Music credits",
            "Manuel Graf - M.Sc. Media Informatics - Munich,Germany - manuelgraf.com- mg@apfelkuh\n\n" +
            "Found on freesound.org/people/ManuelGraf/sounds/410574/ and \n" +
            "freesound.org/people/ManuelGraf/sounds/410576/"
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

        titleText.text = creditPages[index].title;
        descriptionText.text = creditPages[index].description;
    }

    public void GoNext()
    {
        if (currentIndex < creditPages.Count - 1)
        {
            SoundManager.Instance.PlaySelectEffect();

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
            SoundManager.Instance.PlaySelectEffect();

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
        SoundManager.Instance.PlaySelectEffect();

        SceneManager.LoadSceneAsync(0);
    }
}
