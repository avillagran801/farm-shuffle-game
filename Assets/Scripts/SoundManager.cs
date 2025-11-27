using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SoundSettings
{
    public float effectsVolume = 1f;
    public float musicVolume = 1f;
    public float vibrationIntensity = 1f;
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SoundManager");
                _instance = go.AddComponent<SoundManager>();
                DontDestroyOnLoad(go);

                _instance.Initialize();
            }

            return _instance;
        }
    }

    public SoundSettings soundSettings = new SoundSettings();
    public AudioClip selectSound;
    public AudioClip scoreSound;
    public AudioClip incorrectSound;
    public AudioClip gameOverSound;
    public AudioClip buySound;
    public AudioClip gameMusic;
    public AudioClip menuMusic;
    private string settingsPath;
    private AudioSource effectsSource;
    private AudioSource musicSource;
    private AudioSource previewSource;
    private Coroutine previewCoroutine = null;
    private float previewDuration = 1f; // seconds to play the music preview
    private float previewFadeOut = 0.25f; // fade-out length for music preview (sec)
    private bool effectsPreviewCooldown = false;
    private bool vibrationPreviewCooldown = false;

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
        settingsPath = Path.Combine(Application.persistentDataPath, "soundSettings.json");

        effectsSource = gameObject.AddComponent<AudioSource>();
        effectsSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;

        previewSource = gameObject.AddComponent<AudioSource>();
        previewSource.playOnAwake = false;
        previewSource.loop = false;

        LoadSoundSettings();
    }

    public void LoadSoundSettings()
    {
        // Try to load the last user settings on its json file
        try
        {
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                soundSettings = JsonUtility.FromJson<SoundSettings>(json);

                effectsSource.volume = soundSettings.effectsVolume;
                musicSource.volume = soundSettings.musicVolume;
                Debug.Log("Sound settings loaded from file");
            }
            else
            {
                // If theres no json file, create a new one
                Debug.Log("No sound settings file found, creating new settings");
                soundSettings = new SoundSettings();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading user settings: {ex.Message}");
            soundSettings = new SoundSettings();
        }
    }

    public void SaveSoundSettings()
    {
        // Try to write the last sound settings on its json file
        try
        {
            string json = JsonUtility.ToJson(soundSettings, true);
            File.WriteAllText(settingsPath, json);
            Debug.Log("Sound settings saved to " + settingsPath);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving sound settings: {ex.Message}");
        }
    }

    public void ChangeEffectsVolume(float volume)
    {
        soundSettings.effectsVolume = volume;
        effectsSource.volume = volume;
        PlayEffectsPreview();
    }

    public void ChangeMusicVolume(float volume)
    {
        soundSettings.musicVolume = volume;
        musicSource.volume = volume;
        PlayMusicPreview();
    }

    public void ChangeVibrationIntensity(float intensity)
    {
        soundSettings.vibrationIntensity = intensity;
        PlayVibrationPreview();
    }

    private System.Collections.IEnumerator SFXPreviewRoutine()
    {
        effectsPreviewCooldown = true;

        // Play full clip once
        effectsSource.PlayOneShot(selectSound);

        // Wait for the entire duration of the clip
        yield return new WaitForSeconds(selectSound.length);

        // Small cooldown to prevent overlap spam
        yield return new WaitForSeconds(0.1f);

        effectsPreviewCooldown = false;
    }

    public void PlayEffectsPreview()
    {
        if (effectsPreviewCooldown) return; // Avoid overlapping previews

        StartCoroutine(SFXPreviewRoutine());
    }

    private System.Collections.IEnumerator MusicPreviewRoutine()
    {
        // If there's no gameMusic assigned, nothing to preview
        if (gameMusic == null)
            yield break;

        // Ensure previewSource uses the latest clip and volume
        previewSource.clip = gameMusic;
        previewSource.volume = soundSettings.musicVolume;
        previewSource.time = 0f;

        // If preview is currently playing, stop it first to avoid overlap
        if (previewSource.isPlaying)
            previewSource.Stop();

        // Play from start
        previewSource.Play();

        // Play for previewDuration - previewFadeOut, then fade out
        float playTime = Mathf.Max(0f, previewDuration - previewFadeOut);
        float elapsed = 0f;
        while (elapsed < playTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Fade out (linear). Capture starting volume so we restore it later.
        float startVol = previewSource.volume;
        float t = 0f;
        while (t < previewFadeOut)
        {
            t += Time.deltaTime;
            float f = Mathf.Clamp01(t / previewFadeOut);
            previewSource.volume = Mathf.Lerp(startVol, 0f, f);
            yield return null;
        }

        // Stop and restore volume for next preview
        previewSource.Stop();
        previewSource.volume = soundSettings.musicVolume; // ensure it's back to normal

        // Clear coroutine reference
        previewCoroutine = null;
    }

    public void PlayMusicPreview()
    {
        // Stop any running preview coroutine so previews never overlap
        if (previewCoroutine != null)
        {
            StopCoroutine(previewCoroutine);
            previewCoroutine = null;
        }

        // Start a new preview coroutine
        previewCoroutine = StartCoroutine(MusicPreviewRoutine());
    }

    private System.Collections.IEnumerator VibrationPreviewRoutine()
    {
        vibrationPreviewCooldown = true;

        VibrateStrong();

        // Duration of VibrateStrong() = 90 ms → 0.09 seconds
        yield return new WaitForSeconds(0.09f);

        // Small cooldown to avoid spamming while dragging the slider
        yield return new WaitForSeconds(0.1f);

        vibrationPreviewCooldown = false;
    }

    public void PlayVibrationPreview()
    {
        if (vibrationPreviewCooldown) return;

        StartCoroutine(VibrationPreviewRoutine());
    }

    public void PlaySelectEffect()
    {
        effectsSource.PlayOneShot(selectSound);
    }

    public void PlayIncorrectEffect()
    {
        effectsSource.PlayOneShot(incorrectSound);
    }

    public void PlayGameOverEffect()
    {
        effectsSource.PlayOneShot(gameOverSound);
    }

    public void PlayScoreEffect()
    {
        effectsSource.PlayOneShot(scoreSound);
    }

    public void PlayBuyEffect()
    {
        effectsSource.PlayOneShot(buySound);
    }

    public void PlayGameMusic()
    {
        if (musicSource.clip == menuMusic)
        {
            musicSource.Stop();
        }

        if (gameMusic == null)
        {
            Debug.LogWarning("No gameMusic AudioClip assigned!");
            return;
        }

        if (musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = gameMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayMenuMusic()
    {
        if (musicSource.clip == gameMusic)
        {
            musicSource.Stop();
        }

        if (menuMusic == null)
        {
            Debug.LogWarning("No menuMusic AudioClip assigned!");
            return;
        }

        if (musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = menuMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void Vibrate(long milliseconds, int amplitude)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    try
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
        using (var vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator"))
        {
            if (vibrator == null) return;

            int api = new AndroidJavaClass("android.os.Build$VERSION")
                .GetStatic<int>("SDK_INT");

            if (api >= 26)
            {
                amplitude = (int)(amplitude*soundSettings.vibrationIntensity);

                // Android 8+ → use VibrationEffect
                using (var vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect"))
                using (var effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                    "createOneShot", milliseconds, amplitude))
                {
                    vibrator.Call("vibrate", effect);
                }
            }
            else
            {
                // Pre-Android 8
                if(soundSettings.vibrationIntensity >= 0.5){
                    vibrator.Call("vibrate", milliseconds);
                }
            }
        }
    }
    catch (System.Exception e)
    {
        Debug.LogWarning("Vibration failed: " + e.Message);
    }
#endif
    }

    public void VibrateWeak() => Vibrate(30, 80);
    public void VibrateMedium() => Vibrate(60, 180);
    public void VibrateStrong() => Vibrate(90, 255);
}
