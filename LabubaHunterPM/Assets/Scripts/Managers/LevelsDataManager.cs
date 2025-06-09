using DatabaseSystem.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatabaseSystem.Managers
{
    public class LevelsDataManager : DataManager<int, LevelData>
    {
        #region Variables
        private string resourcesLevelsFolder = "";
        private int _selectedLevel;
        private static LevelsDataManager _instance;

        public static LevelsDataManager Instance { get => _instance; set => _instance = value; }
        public int SelectedLevel { get => _selectedLevel; set => _selectedLevel = value; }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }                
            else
                Destroy(gameObject);
        }
        #endregion

        #region Private Methods
        private void LoadFromResources()
        {
            dataDictionary = new Dictionary<int, LevelData>();
            LevelData[] itemsFromResources = Resources.LoadAll<LevelData>(resourcesLevelsFolder);
            foreach (var itemData in itemsFromResources)
            {
                TryPutDataItem(itemData.Id, itemData);
            }
        }
        #endregion

        #region Public Methods
        public void Initialize()
        {
            LoadFromResources();
        }

        public void SelectLevel(int level)
        {
            _selectedLevel = level;
            SetParameters(level);
            SetCharacter(level);
        }

        public void SetParameters(int level)
        {
            PlayerPrefs.SetInt("COINS_FOR_LEVEL", GetDataObject(level).CoinsForLevel);
            PlayerPrefs.SetFloat("ENEMY_HEALTH_MULTIPLIER", GetDataObject(level).EnemyHealthMultiplier);
            PlayerPrefs.SetInt("LEVEL_ID", GetDataObject(level).Id);
        }

        private void SetCharacter(int level)
        {
             PlayerPrefs.SetInt("GAMEPLAY_CHARACTER", Progress.Instance.User.CurrentCharacter);
        }

        #endregion
    }
}