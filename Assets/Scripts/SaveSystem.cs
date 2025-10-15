using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
  public int totalScore;
  public float musicVolume;
}

public class SaveSystem : MonoBehaviour
{
  private string path;

  void Awake()
  {
    path = Path.Combine(Application.persistentDataPath, "save.json");
    Debug.Log("Save path: " + path);
  }

  public void SaveData(GameData data)
  {
    string json = JsonUtility.ToJson(data, true);
    File.WriteAllText(path, json);
    Debug.Log("Game saved to: " + path);
  }

  public GameData LoadData()
  {
    if (File.Exists(path))
    {
      Debug.Log("Game loaded!");
      string json = File.ReadAllText(path);
      return JsonUtility.FromJson<GameData>(json);
    }
    else
    {
      Debug.Log("No save file found — starting fresh.");
      return new GameData(); // Default values
    }
  }
}
