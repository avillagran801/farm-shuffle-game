using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class UserData
{
  public int totalScore;
  public bool[] availableIconPacks;
}

public class UserSettings
{
  public float musicVolume;
}

public class DataManager : MonoBehaviour
{
  private static DataManager _instance;
  public static DataManager Instance
  {
    get
    {
      // If there's no instance yet, create one dynamically
      if (_instance == null)
      {
        GameObject go = new GameObject("DataManager");
        _instance = go.AddComponent<DataManager>();
        DontDestroyOnLoad(go);
        _instance.Initialize(); // Load file immediately
      }
      return _instance;
    }
  }
  public UserData userData = new UserData();
  public UserSettings userSettings = new UserSettings();
  private string dataPath;
  private string settingsPath;

  void Awake()
  {
    if (_instance != null && _instance != this)
    {
      Destroy(gameObject);
      return;
    }

    _instance = this;
    DontDestroyOnLoad(gameObject);
    Initialize();
  }

  private void Initialize()
  {
    dataPath = Path.Combine(Application.persistentDataPath, "userData.json");
    settingsPath = Path.Combine(Application.persistentDataPath, "userSettings.json");

    LoadUserData();
    LoadUserSettings();
  }

  public void SaveUserData()
  {
    try
    {
      string json = JsonUtility.ToJson(userData, true);
      File.WriteAllText(dataPath, json);
      Debug.Log("User data saved to " + dataPath);
    }
    catch (Exception ex)
    {
      Debug.LogError($"Error saving user data: {ex.Message}");
    }
  }
  public void SaveUserSettings()
  {
    try
    {
      string json = JsonUtility.ToJson(userSettings, true);
      File.WriteAllText(settingsPath, json);
      Debug.Log("User settings saved to " + settingsPath);
    }
    catch (Exception ex)
    {
      Debug.LogError($"Error saving user settings: {ex.Message}");
    }
  }

  public void LoadUserData()
  {
    try
    {
      if (File.Exists(dataPath))
      {
        string json = File.ReadAllText(dataPath);
        userData = JsonUtility.FromJson<UserData>(json);
        Debug.Log("User data loaded from file");
      }
      else
      {
        Debug.Log("No user data file found, creating new data");
        userData = new UserData();
      }
    }
    catch (Exception ex)
    {
      Debug.LogError($"Error loading user data: {ex.Message}");
      userData = new UserData();
    }
  }

  public void LoadUserSettings()
  {
    try
    {
      if (File.Exists(settingsPath))
      {
        string json = File.ReadAllText(settingsPath);
        userSettings = JsonUtility.FromJson<UserSettings>(json);
        Debug.Log("User settings loaded from file");
      }
      else
      {
        Debug.Log("No user settings file found, creating new settings");
        userSettings = new UserSettings();
      }
    }
    catch (Exception ex)
    {
      Debug.LogError($"Error loading user settings: {ex.Message}");
      userSettings = new UserSettings();
    }
  }

  private void OnApplicationPause(bool pause)
  {
    if (pause)
    {
      SaveUserData();
    }
  }
  private void OnApplicationQuit()
  {
    SaveUserData();
    SaveUserSettings();
  }
}
