using System;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

[System.Serializable]
public class UserData
{
  public bool tutorial = true;
  public int totalScore;
  public bool[] boughtIconPacks;
  public bool[] equipedIconPacks;
  public List<ScoreEntry> topScores = new List<ScoreEntry>(5);
}

[System.Serializable]
public class ScoreEntry
{
  public int score;
  public string timestamp;
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

  public IconPack[] allIconPacks;
  private string dataPath;
  // Your secret key (32 bytes for AES-256)
  private static readonly byte[] Key = Encoding.UTF8.GetBytes("F4rzL6ykHaVIzRc8wmoe0QD7OFgduG0Z");
  // 16-byte IV (initialization vector)
  private static readonly byte[] IV = Encoding.UTF8.GetBytes("8Sh9kCD4jNvX0aK5");

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

    LoadUserData();

    LoadIconPacks();
  }

  private void LoadIconPacks()
  {
    allIconPacks = Resources.LoadAll<IconPack>("IconPacks");

    if (userData.boughtIconPacks == null || userData.boughtIconPacks.Length != allIconPacks.Length)
    {
      userData.boughtIconPacks = new bool[allIconPacks.Length];
      userData.boughtIconPacks[0] = true;

      userData.equipedIconPacks = new bool[allIconPacks.Length];
      userData.equipedIconPacks[0] = true;
    }
  }

  public void SaveUserData()
  {
    // Try to write the last user data on its json file
    try
    {
      string json = JsonUtility.ToJson(userData, true);
      string encrypted = Encrypt(json);

      File.WriteAllText(dataPath, encrypted);
      // Debug.Log("User data saved to " + dataPath);
    }
    catch (Exception ex)
    {
      Debug.LogError($"Error saving user data: {ex.Message}");
    }
  }

  public void LoadUserData()
  {
    // Try to load the last user data on its json file
    try
    {
      if (File.Exists(dataPath))
      {
        string encryptedJson = File.ReadAllText(dataPath);
        string decryptedJson = Decrypt(encryptedJson);

        userData = JsonUtility.FromJson<UserData>(decryptedJson);
        // Debug.Log("User data loaded from file");
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

  string Encrypt(string plainText)
  {
    using Aes aes = Aes.Create();
    aes.Key = Key;
    aes.IV = IV;

    using MemoryStream ms = new MemoryStream();
    using CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
    using StreamWriter sw = new StreamWriter(cs);

    sw.Write(plainText);
    sw.Close();

    return Convert.ToBase64String(ms.ToArray());
  }

  string Decrypt(string encryptedText)
  {
    byte[] buffer = Convert.FromBase64String(encryptedText);

    using Aes aes = Aes.Create();
    aes.Key = Key;
    aes.IV = IV;

    using MemoryStream ms = new MemoryStream(buffer);
    using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
    using StreamReader sr = new StreamReader(cs);

    return sr.ReadToEnd();
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
  }
}
