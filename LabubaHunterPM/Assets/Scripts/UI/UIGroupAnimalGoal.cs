using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGroupAnimalGoal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textCount;
    [SerializeField] private Image _icon;

    public TextMeshProUGUI TextCount { get => _textCount; set => _textCount = value; }
    public Image Icon { get => _icon; set => _icon = value; }
}
