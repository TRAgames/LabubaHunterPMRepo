using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLevel : MonoBehaviour
{
    [Header("Текстовые поля")]
    [SerializeField] private TextMeshProUGUI _textLevelNumber;
    [SerializeField] private TextMeshProUGUI _textPlay;
    //[SerializeField] private TextMeshProUGUI _textCoins;
    [Header("Цветовая маркировка кнопки")]
    [SerializeField] private Color _colorActive;
    [SerializeField] private Color _colorComplited;
    [SerializeField] private Color _colorUnactive;
    //[Header("Основной фон")]
    //[SerializeField] private Image _icon;
    [Header("Заблокированный уровень")]
    [SerializeField] private GameObject _groupLock;
    [Header("Кнопка")]
    [SerializeField] private Button _button;
    //[Header("Префаб с целями уровня")]
    //[SerializeField] private UIGroupAnimalGoal _uiGroupAnimalGoal;
    //[Header("Место спавна префаба")]
    //[SerializeField] private Transform _spawnPoint;

    public TextMeshProUGUI TextLevelNumber { get => _textLevelNumber; set => _textLevelNumber = value; }
    public Button Button { get => _button; set => _button = value; }
    //public Image Icon { get => _icon; set => _icon = value; }
    public TextMeshProUGUI TextPlay { get => _textPlay; set => _textPlay = value; }
    //public TextMeshProUGUI TextCoins { get => _textCoins; set => _textCoins = value; }
    //public UIGroupAnimalGoal UiGroupAnimalGoal { get => _uiGroupAnimalGoal; set => _uiGroupAnimalGoal = value; }
    //public Transform SpawnPoint { get => _spawnPoint; set => _spawnPoint = value; }

    public void SetUnactive()
    {
        _button.SetUnactive(_textPlay, _colorUnactive);
        _groupLock.gameObject.SetActive(true);
    }
    public void SetActive()
    {
        _button.SetActive(_textPlay, _colorActive, Color.white);
        _groupLock.gameObject.SetActive(false);
    }
    public void SetComplited()
    {
        _button.SetActive(_textPlay, _colorComplited, Color.white);
    }
}
