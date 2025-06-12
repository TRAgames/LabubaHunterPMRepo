using DatabaseSystem.Managers;
using Ilumisoft.VolumeControl;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using DatabaseSystem.ScriptableObjects;

public class Launcher : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void FinishLevel();
    [DllImport("__Internal")]
    private static extern void FinishLevelReload();
#endif

    [Header("Редактор")]
    [SerializeField] private bool _isAndroid;
    [SerializeField] private string _platform;
    [SerializeField] private string _language;

    [Header("Адаптер экрана на Android")]
    [SerializeField] private GameObject _AndroidScreenAdapter;

    [Header("Отключение звука на iOS")]
    [SerializeField] private GameObject _soundFPSController;
    [SerializeField] private GameObject _soundSettings;
    [SerializeField] private GameObject _textSoundFPSController;
    [SerializeField] private GameObject _textSoundSettings;

    [Header("Игрок")]
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _cameraUI;
    [SerializeField] private GameObject _canvas;

    [Header("Кнопки")]
    [SerializeField] private Button _btnPC;
    [SerializeField] private Button _btnPhone;
    [SerializeField] private Button _btnSettings;
    [Header("Окна")]
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _heroScreen;
    [SerializeField] private GameObject _platformScreen;
    [SerializeField] private GameObject _languageScreen;
    [SerializeField] private GameObject _blackLoadingScreen;
    [SerializeField] private GameObject _lobbyCharacter;
    [SerializeField] private GameObject _levelsScreen;
    [SerializeField] private GameObject _inputScreen;
    [SerializeField] private GameObject _settingsScreen;
    [Header("Менеджеры")]
    [SerializeField] private UILevelGenerator _uiLevelGenerator;
    [SerializeField] private CurrencyManager _currencyManager;
    [SerializeField] private PlayerStatsManager _playerStatsManager;
    [SerializeField] private PopupNotEnough _popupNotEnough;
    [SerializeField] private LevelsDataManager _levelsDataManager;
    [Header("Нехватка ресурсов")]
    [SerializeField] private string _characterBlockedKey;
    [Header("Место спавна целей уровня")]
    [SerializeField] private Transform _spawnPoint;
    [Header("Префаб с целями уровня")]
    [SerializeField] private UIGroupAnimalGoal _uiGroupAnimalGoal;

    private int _level;
    private GameObject _currentLevelPrefab;

    public static Launcher Instance { get; private set; }
    public PlayerStatsManager PlayerStatsManager { get => _playerStatsManager; set => _playerStatsManager = value; }
    public CurrencyManager CurrencyManager { get => _currencyManager; set => _currencyManager = value; }
    public PopupNotEnough PopupNotEnough { get => _popupNotEnough; set => _popupNotEnough = value; }
    public string Platform { get => _platform; set => _platform = value; }
    public GameObject LevelsScreen { get => _levelsScreen; set => _levelsScreen = value; }
    public string Language { get => _language; set => _language = value; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (Progress.Instance.IsPlatformKnown)
            CheckFirstSession();
#if !UNITY_EDITOR && UNITY_WEBGL
		else 
            FinishLevel();
#endif
    }
    public void BeforeStart()
    {
        Time.timeScale = 1;
        _playerStatsManager.CreateCharactersModels();
        _playerStatsManager.GetPlayerStats(Progress.Instance.User.CurrentCharacter);
        _currencyManager.GetPlayerCurrency();
        _uiLevelGenerator.GenerateUILevels();
        _heroScreen.SetActive(true);
        _levelsScreen.SetActive(true);
        _platformScreen.SetActive(false);
        MusicController.Instance.PlayMusicLobby();     
        MuteSound(false);
    }

    private void MakeUIGroupAnimalGoal()
    {
        foreach (AnimalGoal goal in _levelsDataManager.GetDataObject(_level).LevelType.Goals)
        {
            UIGroupAnimalGoal newUIGroupAnimalGroup = Instantiate(_uiGroupAnimalGoal);
            newUIGroupAnimalGroup.TextCount.text = goal.Count.ToString();
            newUIGroupAnimalGroup.Icon.sprite = goal.AnimalData.Icon;
            newUIGroupAnimalGroup.transform.SetParent(_spawnPoint);
            newUIGroupAnimalGroup.transform.localScale = Vector3.one;
            newUIGroupAnimalGroup.transform.localPosition = Vector3.zero;
        }
    }

    private void DestroyChildren()
    {
        foreach (Transform child in _spawnPoint)
        {
            Destroy(child.gameObject);
        }
    }

    public void DeactivateLoadingScreen()
    {
        _blackLoadingScreen.SetActive(false);
    }

    public void CheckFirstSession()
    {
        LocalizationManager.Instance.SetLanguage(Progress.Instance.User.Language);
        _levelsScreen.SetActive(true);
        DeactivateLoadingScreen();
#if !UNITY_EDITOR && UNITY_ANDROID
        _AndroidScreenAdapter.SetActive(true);
#endif
#if UNITY_EDITOR
        if (_isAndroid)
        {
            _AndroidScreenAdapter.SetActive(true);
        }
#endif
        BeforeStart();
    }

    public void OnButtonLanguage(string language)
    {

#if !UNITY_EDITOR && UNITY_ANDROID
        PlayerPrefs.DeleteKey("ANDROID");
        PlayerPrefs.SetString("ANDROID", "yes");
#endif

#if UNITY_EDITOR
        PlayerPrefs.DeleteKey("ANDROID");
        if (_isAndroid)
        {
            PlayerPrefs.SetString("ANDROID", "yes");
            _AndroidScreenAdapter.SetActive(true);
            _platform = "PHONE";
        }
#endif

        Progress.Instance.User.Language = language;

        LocalizationManager.Instance.SetLanguage(Progress.Instance.User.Language);

        _languageScreen.SetActive(false);


        _platformScreen.SetActive(true);


#if UNITY_EDITOR   
        //Progress.Instance.User.Platform = _platform;
        //PlatformMemory.Instance.IsPlatformKnown = true;
        //BeforeStart();
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
        Progress.Instance.User.Platform = "PHONE";
        PlatformMemory.Instance.IsPlatformKnown = true;
        _AndroidScreenAdapter.SetActive(true);       
        BeforeStart();
#endif
    }

    public void OnButtonPlatform(string platform)
    {
        Progress.Instance.User.Platform = platform;
        PlatformMemory.Instance.IsPlatformKnown = true;

#if UNITY_EDITOR || UNITY_WEBGL
        if (PlayerPrefs.GetString("PLATFORM") == "PC")
           QualitySettings.SetQualityLevel(0);
        else
            QualitySettings.SetQualityLevel(0);
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
            QualitySettings.SetQualityLevel(3);
#endif

        BeforeStart();
    }

    public void OnButtonLevel(int level)
    {

        _level = level; 
        DestroyChildren();
        MakeUIGroupAnimalGoal();
        _lobbyCharacter.SetActive(true);
        _heroScreen.SetActive(true);
        _levelsScreen.SetActive(false); 
    }

    public void OnButtonPlay()
    {
        SoundController.Instance.PlayClick();
        if (Progress.Instance.User.GetUnblockStatus(Progress.Instance.User.CurrentCharacter))
        {
            _loadingScreen.SetActive(true);
            _lobbyCharacter.SetActive(false);
            _btnSettings.gameObject.SetActive(false);

            LevelsDataManager.Instance.SelectLevel(_level);

            LevelData currentLevel = LevelsDataManager.Instance.GetDataObject(LevelsDataManager.Instance.SelectedLevel);

            _player.transform.position = currentLevel.PositionStartPoint;
            _player.transform.rotation = currentLevel.RotationStartPoint;

            _player.SetActive(true);

           
            _currentLevelPrefab = Instantiate(currentLevel.PrefabMobile);

            LevelTypeManager levelTypeManager = GameObject.Find("LevelType_New").GetComponent<LevelTypeManager>();
            levelTypeManager.LevelType = currentLevel.LevelType;

            Destroy(_cameraUI);
            Destroy(_canvas);


            GameObject startPoint = GameObject.Find("START POINT");
            //GameObject checkpoint = GameObject.Find("CHECKPOINT");



            //checkpoint.transform.position = currentLevel.PositionStartPoint;
            startPoint.gameObject.SetActive(false);

        }
        else
        {
            _popupNotEnough.SetPosition(_characterBlockedKey);
        }
    }

    private IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(0.1f);
        

    }

    public void DestroyCurrentLevelPrefab()
    {
        Destroy(_currentLevelPrefab);
    }

    public void OnButtonSlaughter()
    {
        SoundController.Instance.PlayClick();
        if (Progress.Instance.User.GetUnblockStatus(Progress.Instance.User.CurrentCharacter))
        {
            LevelsDataManager.Instance.SelectLevel(-1);
            _lobbyCharacter.SetActive(false);
            _heroScreen.SetActive(false);
            _loadingScreen.SetActive(true);

            StartCoroutine(StartToLoad());
        }
        else
        {
            _popupNotEnough.SetPosition(_characterBlockedKey);
        }
    }

    public void OnButtonBack()
    {
        SoundController.Instance.PlayClick();
        _heroScreen.SetActive(true);
        //_shopScreen.SetActive(false);
        _levelsScreen.SetActive(false);
        _lobbyCharacter.SetActive(true);
        _btnSettings.gameObject.SetActive(true);
    }

    public void OnButtonBackShop()
    {
        SoundController.Instance.PlayClick();
        _heroScreen.SetActive(true);
        //_shopScreen.SetActive(false);
        _lobbyCharacter.SetActive(true);
        _btnSettings.gameObject.SetActive(true);
        _playerStatsManager.GetPlayerStats(Progress.Instance.User.CurrentCharacter);
    }

    public void OnButtonShop()
    {
        SoundController.Instance.PlayClick();
        //_shopScreen.SetActive(true);
        _lobbyCharacter.SetActive(false);
        _btnSettings.gameObject.SetActive(false);
    }

    public void OnButtonBackSettings()
    {
        SoundController.Instance.PlayClick();
        _settingsScreen.SetActive(false);
        _btnSettings.gameObject.SetActive(true);
    }

    public void OnButtonInput()
    {
        SoundController.Instance.PlayClick();
        _inputScreen.SetActive(true);
    }

    public void OnButtonBackInput()
    {
        SoundController.Instance.PlayClick();
        _inputScreen.SetActive(false);
    }

    public void OnToggleAudioTutorial(bool value)
    {
        if (!value)
        {
            VolumeControl.Music.Volume = 0;
            VolumeControl.SFX.Volume = 0;
        }
        else
        {
            VolumeControl.Music.Volume = 0.5f;
            VolumeControl.SFX.Volume = 0.5f;
        }
    }
    public void MuteSound(bool value)
    {
        VolumeControl.Music.IsMuted = value;
        VolumeControl.SFX.IsMuted = value;
    }

    public void SetTutorialStep(int step)
    {
        Progress.Instance.User.TutorialStep = step;
        Progress.Instance.Save();
    }

    public int GetTutorialStep()
    {
        return Progress.Instance.User.TutorialStep;
    }

    public void ShowInterstitialAds()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
		FinishLevelReload();
#endif
#if UNITY_EDITOR
        Progress.Instance.ReloadScene();
#endif
    }

    private IEnumerator StartToLoad()
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadSceneAsync("Gameplay");
    }


    public void TurnOffAllSound(string data)
    {
        //if (data == "iOSSafari")
        Debug.Log("From Unity: " + data);
        if (data == "iOS")
        {
            _soundFPSController.SetActive(false);
            _soundSettings.SetActive(false);

            _textSoundFPSController.SetActive(true);
            _textSoundSettings.SetActive(true);

            VolumeControl.Music.Volume = 0;
            VolumeControl.SFX.Volume = 0;

            VolumeControl.Music.IsMuted = true;
            VolumeControl.SFX.IsMuted = true;
        }
    }
}
