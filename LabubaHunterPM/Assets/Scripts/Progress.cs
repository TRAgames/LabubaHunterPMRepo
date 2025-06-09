using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DeviceType
{
    Mobile,
    Desktop
}

public class Progress : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void LoadExtern();
    [DllImport("__Internal")]
    private static extern void SaveExtern(string data);
#endif
    public static Progress Instance;
    public bool IsSecondStart;
    public bool IsPlatformKnown = false;



    [HideInInspector] public bool IsAdsShowing = false;

    [HideInInspector] public User User;
    [HideInInspector] public UserExtern UserExtern;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        LoadExtern();
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
        LoadEmpty();
#endif
#if UNITY_EDITOR
        LoadEmpty();      
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) && Input.GetKeyDown(KeyCode.Home))
        {
            DeleteAllData();
            Launcher.Instance.PlayerStatsManager.GetPlayerStats(User.CurrentCharacter);
            Launcher.Instance.CurrencyManager.GetPlayerCurrency();
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && Input.GetKeyDown(KeyCode.End))
        {
            User.CountOpenLevels = 30;
            CoinsManager.Instance.AddCoins(50000);
        }

        if (Input.GetKeyDown(KeyCode.End))
        {
            ScreenCapture.CaptureScreenshot("Gameplay_" + UnityEngine.Random.Range(0,90) + ".jpg");
        }
    }

    public void OnButtonReset()
    {
        DeleteAllData();
        Launcher.Instance.PlayerStatsManager.GetPlayerStats(User.CurrentCharacter);
        Launcher.Instance.CurrencyManager.GetPlayerCurrency();
    }

    public void Save()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        string jsonString = JsonUtility.ToJson(User.GetCurrentUser());
        SaveExtern(jsonString);
#endif
    }

    public void SaveEmpty()
    {
        User.SetCurrentUser(User.GetCurrentUser());
    }

    public void Load(string value)
    {
        UserExtern = JsonUtility.FromJson<UserExtern>(value);
        User.SetCurrentUser(UserExtern);              
    }

    public void LoadEmpty()
    {
        UserExtern = new UserExtern();
        UserExtern = User.GetCurrentUser();
        User.SetCurrentUser(UserExtern);

        SetLanguage(Launcher.Instance.Language);
        SetCurrentDeviceType(Launcher.Instance.Platform);

        DeactivateBlackLoadingScreen();
    }

    private void DeleteAllData()
    {
        StopAllCoroutines();
        PlayerPrefs.DeleteAll();
        User = new User();
        UserExtern = new UserExtern();
#if !UNITY_EDITOR && UNITY_WEBGL
		Save();
#endif
    }

    void OnApplicationFocus(bool hasFocus)
    {
        Silence(!hasFocus);
    }

    void OnApplicationPause(bool isPaused)
    {
        Silence(isPaused);
    }

    private void Silence(bool silence)
    {
        if (!IsAdsShowing)
            Launcher.Instance.MuteSound(silence);
    }

    public void PauseMusic()
    {
        IsAdsShowing = true;
        Launcher.Instance.MuteSound(true);
    }

    public void UnpauseMusic()
    {
        IsAdsShowing = false;
        Launcher.Instance.MuteSound(false);
    }

    public void DeactivateBlackLoadingScreen()
    {
        Launcher.Instance.DeactivateLoadingScreen();
    }

    public void ReloadScene()
    {
        IsPlatformKnown = true;
        SceneManager.LoadScene("Preloader");
    }

    public void SetLanguage(string data)
    {
        switch (data)
        {
            case "ru": User.Language = data; break;
            case "en": User.Language = data; break;
            case "tr": User.Language = data; break;
            case "de": User.Language = data; break;
            case "es": User.Language = data; break;
            default: User.Language = "en"; break;
        }

        LocalizationManager.Instance.SetLanguage(User.Language);
        Launcher.Instance.LevelsScreen.SetActive(true);
    }
    public void SetCurrentDeviceType(string data)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        User.Platform = data == "desktop" ? "PC" : "PHONE";
        if (User.Platform == "PC")
            QualitySettings.SetQualityLevel(3);
        else
            QualitySettings.SetQualityLevel(0);
#endif
#if UNITY_EDITOR
        User.Platform = data;
        if (User.Platform == "PC")
            QualitySettings.SetQualityLevel(3);
        else
            QualitySettings.SetQualityLevel(0);
#endif
        Launcher.Instance.BeforeStart();
    }


}
