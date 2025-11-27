using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialPage
{
    public string title;
    public string description;

    public TutorialPage(string t, string d)
    {
        title = t;
        description = d;
    }
}

public class Tutorial : MonoBehaviour
{

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public GameObject nextButton;
    public GameObject backButton;
    public GameObject playButton;
    public GameObject gameIconsExplanation;
    private List<TutorialPage> tutorialPages = new List<TutorialPage>();
    private int currentIndex = 0;

    void Start()
    {
        tutorialPages.Add(new TutorialPage(
            "Game background",
            "After a strong earthquake in the south of the country, your little farm has ended up completely upside down: " +
            "the cows are next to the lettuces and your shoes are right next to the eggs. What a mess!" + "\n\n" +
            "To make things easier for yourself and assess the damage, your mission is to group every pair of items you " +
            "come across as quickly as possible."
        ));

        tutorialPages.Add(new TutorialPage(
            "How to play",
            "During the game, you’ll see two panels filled with items." + "\n\n" +
            "Every item is different, except for one matching pair that appears in both panels." + "\n\n" +
            "Your goal is to find and click the matching items as quickly as possible to earn a maqui point!"
        ));

        tutorialPages.Add(new TutorialPage(
            "How to play",
            ""
        ));

        tutorialPages.Add(new TutorialPage(
            "How to play",
            "Each time you score a maqui point, 5 extra seconds are added to the countdown." + "\n\n" +
            "But watch out, the game ends when time runs out!"
        ));


        backButton.GetComponent<Button>().interactable = false;
        ShowPage(0);
    }

    public void ShowPage(int index)
    {
        if (index < 0 || index >= tutorialPages.Count)
        {
            return;
        }

        if (index == 2)
        {
            gameIconsExplanation.SetActive(true);
        }
        else
        {
            gameIconsExplanation.SetActive(false);
        }

        titleText.text = tutorialPages[index].title;
        descriptionText.text = tutorialPages[index].description;
    }

    public void GoNext()
    {
        if (currentIndex < tutorialPages.Count - 1)
        {
            SoundManager.Instance.PlaySelectEffect();

            currentIndex++;
            ShowPage(currentIndex);

            if (currentIndex == 1)
            {
                backButton.GetComponent<Button>().interactable = true;
            }
        }

        if (currentIndex == tutorialPages.Count - 1)
        {
            nextButton.SetActive(false);
            playButton.SetActive(true);
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

        if (currentIndex == tutorialPages.Count - 2)
        {
            nextButton.SetActive(true);
            playButton.SetActive(false);
        }
    }

    public void GoHome()
    {
        SoundManager.Instance.PlaySelectEffect();
        // Scene 0: Main menu
        SceneManager.LoadSceneAsync(0);
    }

    public void PlayGame()
    {
        SoundManager.Instance.PlaySelectEffect();

        // Deactivate the tutorial for the next gameplay
        DataManager.Instance.userData.tutorial = false;
        DataManager.Instance.SaveUserData();

        // Scene 2: Gameplay
        SceneManager.LoadSceneAsync(2);
    }
}
