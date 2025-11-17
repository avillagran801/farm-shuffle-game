using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class UserData
{
  public bool tutorial = true;
  public int totalScore;
  public bool[] boughtIconPacks;
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

        // Load file immediately
        _instance.Initialize();
      }
      return _instance;
    }
  }
  public UserData userData = new UserData();
  public UserSettings userSettings = new UserSettings();

  public IconPack[] allIconPacks;
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
    // Initialize the paths and load both the user data and settings
    dataPath = Path.Combine(Application.persistentDataPath, "userData.json");
    settingsPath = Path.Combine(Application.persistentDataPath, "userSettings.json");

    LoadUserData();
    LoadUserSettings();

    LoadIconPacks();
  }

  private void LoadIconPacks()
  {
    allIconPacks = Resources.LoadAll<IconPack>("IconPacks");

    if (userData.boughtIconPacks == null || userData.boughtIconPacks.Length != allIconPacks.Length)
    {
      userData.boughtIconPacks = new bool[allIconPacks.Length];
      userData.boughtIconPacks[0] = true;
    }
  }

  public void SaveUserData()
  {
    // Try to write the last user data on its json file
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
    // Try to write the last user settings on its json file
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
    // Try to load the last user data on its json file
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
        // If theres no json file, create a new one
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
    // Try to load the last user settings on its json file
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
        // If theres no json file, create a new one
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
      SaveUserSettings();
    }
  }
  private void OnApplicationQuit()
  {
    SaveUserData();
    SaveUserSettings();
  }
}
