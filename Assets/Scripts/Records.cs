using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Records : MonoBehaviour
{
    public TextMeshProUGUI datesListText;
    public TextMeshProUGUI scoresListText;

    void Start()
    {
        string datesListFormat = "";
        string scoresListFormat = "";

        var scores = DataManager.Instance.userData.topScores;

        for (int i = 0; i < 5; i++)
        {
            if (scores != null && i < scores.Count)
            {
                // Valid entry exists
                datesListFormat += (i + 1) + ". " + scores[i].timestamp + "\n";
                scoresListFormat += scores[i].score + "\n";
            }
            else
            {
                // No score for this position
                datesListFormat += (i + 1) + ". ---" + "\n";
                scoresListFormat += "---" + "\n";
            }
        }

        datesListText.text = datesListFormat;
        scoresListText.text = scoresListFormat;
    }

    public void GoHome()
    {
        SoundManager.Instance.PlaySelectEffect();

        SceneManager.LoadSceneAsync(0);
    }
}
