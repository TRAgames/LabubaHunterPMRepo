using DatabaseSystem.Managers;
using DatabaseSystem.ScriptableObjects;
using Ilumisoft.VolumeControl;
using System.Collections;
using UnityEngine;

namespace DatabaseSystem.Tests
{
    public class LevelGenerator : MonoBehaviour
    {
        #region Variables
        [Header("Качество графики на ПК (только редактор)")]
        [SerializeField] private int _qualityPC;
        [Header("Качество графики на Телефоне (только редактор)")]
        [SerializeField] private int _qualityMobile;
        [Header("Тестовый запуск?")]
        [SerializeField] private bool _isEditorMode;
        [Header("Префаб уровня для тестового запуска")]
        [SerializeField] private LevelData _levelData;
        [Header("Уровень игрока")]
        [SerializeField] private int _playerLevel;
        [Header("Персонаж игрока")]
        [SerializeField] private CharacterData _characterData;
        [Header("Язык игры")]
        [SerializeField] private string _language = "ru";
        [Header("Платформа игры")]
        [SerializeField] private string _platform = "PC";
        [Header("Объект для спавна компонентов")]
        [SerializeField] private GameObject _tempObject;

        private int _damageBody;
        private int _damageHead;
        private int _health;
        private float _fireRate;
        private float _magazine;
        private float _speed;

        public bool IsEditorMode { get => _isEditorMode; set => _isEditorMode = value; }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            try
            {
                LevelsDataManager.Instance.Initialize();
                _isEditorMode = false;
            }
            #region Editor
            catch
            {
                
                Debug.Log("LevelsDataManager can't initialize");

                _tempObject.AddComponent<LocalizationManager>();

                PlayerPrefs.SetInt("GAMEPLAY_CHARACTER", _characterData.Id);

                PlayerPrefs.SetString("PLATFORM", _platform);
                LocalizationManager.Instance.SetLanguage(_language);

                _fireRate = _characterData.FireRateBase;
                _magazine = _characterData.MagazineBase;
                _speed = _characterData.SpeedBase;

                float damageMultiplierResult = GetMultiplierResult(_characterData.DamageMul, _playerLevel);
                float healthMultiplierResult = GetMultiplierResult(_characterData.HealthMul, _playerLevel);

                _damageBody = (int)Mathf.Round(damageMultiplierResult * _characterData.DamageBodyBase);
                _damageHead = (int)Mathf.Round(damageMultiplierResult * _characterData.DamageHeadBase);
                _health = (int)Mathf.Round(healthMultiplierResult * _characterData.HealthBase);

                PlayerPrefs.SetInt("DAMAGE_BODY", _damageBody);
                PlayerPrefs.SetInt("DAMAGE_HEAD", _damageHead);
                PlayerPrefs.SetInt("HEALTH", _health);
                PlayerPrefs.SetFloat("FIRE_RATE", _fireRate);
                PlayerPrefs.SetFloat("MAGAZINE", _magazine);
                PlayerPrefs.SetFloat("SPEED", _speed);

                PlayerPrefs.SetInt("COINS_FOR_LEVEL", _levelData.CoinsForLevel);
                PlayerPrefs.SetFloat("ENEMY_HEALTH_MULTIPLIER", _levelData.EnemyHealthMultiplier);
                PlayerPrefs.SetInt("LEVEL_ID", _levelData.Id);
            }
            #endregion

#if !UNITY_EDITOR && UNITY_WEBGL
            if (PlayerPrefs.GetString("PLATFORM") == "PC")
                QualitySettings.SetQualityLevel(4);
            else
                QualitySettings.SetQualityLevel(0);
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
            QualitySettings.SetQualityLevel(3);
#endif
#if UNITY_EDITOR
            if (PlayerPrefs.GetString("PLATFORM") == "PC")
                QualitySettings.SetQualityLevel(_qualityPC);
            else
                QualitySettings.SetQualityLevel(_qualityMobile);
#endif
        }

        private float GetMultiplierResult(float mul, int level)
        {
            return Mathf.Pow(mul + 1, level);
        }

        private void Start()
        {
            if (_isEditorMode)
            {
                VolumeControl.SFX.IsMuted = false;

                //if (PlayerPrefs.GetString("PLATFORM") == "PC")
                    //Instantiate(_levelData.Prefab);
                //else
                    Instantiate(_levelData.PrefabMobile);

                //RenderSettings.skybox = _levelData.Skybox;
                //bool darkMode = _levelData.DarkMode;

                LevelTypeManager levelTypeManager = GameObject.Find("LevelType_New").GetComponent<LevelTypeManager>();
                levelTypeManager.LevelType = LevelsDataManager.Instance.GetDataObject(LevelsDataManager.Instance.SelectedLevel).LevelType;

                GameObject player = GameObject.Find("FPSController");
                GameObject startPoint = GameObject.Find("START POINT");
                //if (darkMode)
                //{
                //    GameObject light = GameObject.Find("DirectionalLight");
                //    light.SetActive(false);
                //}
                player.transform.position = startPoint.transform.position;
                player.transform.rotation = startPoint.transform.rotation;
                startPoint.gameObject.SetActive(false);
                VolumeControl.Music.IsMuted = false;
                VolumeControl.Music.Volume = 0.7f;            
            }
            else
            {
                //if (PlayerPrefs.GetString("PLATFORM") == "PC")
                    //Instantiate(LevelsDataManager.Instance.GetDataObject(LevelsDataManager.Instance.SelectedLevel).Prefab);
                //else
                    Instantiate(LevelsDataManager.Instance.GetDataObject(LevelsDataManager.Instance.SelectedLevel).PrefabMobile);

                //RenderSettings.skybox = LevelsDataManager.Instance.GetDataObject(LevelsDataManager.Instance.SelectedLevel).Skybox;
                //bool darkMode = LevelsDataManager.Instance.GetDataObject(LevelsDataManager.Instance.SelectedLevel).DarkMode;

                LevelsDataManager.Instance.SetParameters(LevelsDataManager.Instance.SelectedLevel);

                GameObject player = GameObject.Find("FPSController");
                GameObject startPoint = GameObject.Find("START POINT");

                //if (darkMode)
                //{
                //    GameObject light = GameObject.Find("DirectionalLight");
                //    light.SetActive(false);
                //}
                player.transform.position = startPoint.transform.position;
                player.transform.rotation = startPoint.transform.rotation;
                startPoint.gameObject.SetActive(false);
            }

            //try
                //{                   

                //} 
                //catch
                //{
                    //Debug.Log("Can't instantiate level");
                //}
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.End))
            {
                ScreenCapture.CaptureScreenshot("Gameplay_" + Random.Range(0, 90) + ".jpg");
            }
        }
        #endregion
    }
}