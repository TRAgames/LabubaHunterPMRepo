using DatabaseSystem.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILevelGenerator : MonoBehaviour
{
    #region Variables
    [SerializeField] private LevelsDataManager _levelsDataManager;
    [Header("Зона отображения всех уровней")]
    [SerializeField] private Transform _content;
    [Header("UI элемент с уровнем")]
    [SerializeField] private ButtonLevel _element;
    [Header("Ключ \"Уровень ещё не открыт\"")]
    [SerializeField] private string _notOpenedYet;
    [Header("Ключ \"Играть\"")]
    [SerializeField] private string _playKey;
    [Header("Ключ \"Переиграть\"")]
    [SerializeField] private string _replayKey;
    [Header("Ключ \"Заблокировано\"")]
    [SerializeField] private string _blockedKey; 

    #endregion

    #region Unity Methods
    private void Awake()
    {
        _levelsDataManager.Initialize();
    }

    public void GenerateUILevels()
    {
        foreach (var item in _levelsDataManager.GetAllDataObjects())
        {
            ButtonLevel newButtonLevel = Instantiate(_element);
            newButtonLevel.TextLevelNumber.text = (item.Value.Id + 1).ToString();//LocalizationManager.Instance.GetLocalizedValue(item.Value.NameKey) + " " + (item.Value.Id + 1);
            //newButtonLevel.Icon.sprite = item.Value.Icon;
            //newButtonLevel.TextCoins.text = item.Value.CoinsForLevel.ToString();
            newButtonLevel.transform.SetParent(_content);
            newButtonLevel.transform.localScale = Vector3.one;
            newButtonLevel.transform.localPosition = Vector3.zero;
            newButtonLevel.Button.onClick.AddListener(delegate { SelectLevel(item.Value.Id); });

            //foreach (AnimalGoal goal in item.Value.LevelType.Goals)
            //{
                //UIGroupAnimalGoal newUIGroupAnimalGroup = Instantiate(newButtonLevel.UiGroupAnimalGoal);
                //newUIGroupAnimalGroup.TextCount.text = goal.Count.ToString();
                //newUIGroupAnimalGroup.Icon.sprite = goal.AnimalData.Icon;
                //newUIGroupAnimalGroup.transform.SetParent(newButtonLevel.SpawnPoint);
                //newUIGroupAnimalGroup.transform.localScale = Vector3.one;
                //newUIGroupAnimalGroup.transform.localPosition = Vector3.zero;
            //}
                
            if (item.Value.Id > Progress.Instance.User.CountOpenLevels)
            {
                newButtonLevel.TextPlay.text = LocalizationManager.Instance.GetLocalizedValue(_blockedKey);
                newButtonLevel.SetUnactive();
            }
            else if (item.Value.Id < Progress.Instance.User.CountOpenLevels)
            {
                newButtonLevel.TextPlay.text = LocalizationManager.Instance.GetLocalizedValue(_replayKey);
                newButtonLevel.SetComplited();
            }
            else 
                newButtonLevel.TextPlay.text = LocalizationManager.Instance.GetLocalizedValue(_playKey);           
        }
        Vector3 contentPosition = _content.transform.localPosition;
        Vector3 newPosition = new Vector3(contentPosition.x 
            - 138 
            * Mathf.Clamp((Progress.Instance.User.CountOpenLevels - 1), 0, 138 * _levelsDataManager.GetAllDataObjects().Count), 
            contentPosition.y, contentPosition.z);
        _content.transform.localPosition = newPosition;
    }

    private void SelectLevel(int level)
    {
        SoundController.Instance.PlayClick();
        if (Progress.Instance.User.CountOpenLevels >= level)
        {
            Launcher.Instance.OnButtonLevel(level);
        }
        else
        {
            Launcher.Instance.PopupNotEnough.SetPosition(_notOpenedYet);
        }
        
    }
    #endregion
}

