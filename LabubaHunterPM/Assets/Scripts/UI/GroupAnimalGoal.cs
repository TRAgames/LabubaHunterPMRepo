using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroupAnimalGoal : MonoBehaviour
{
    public TextMeshProUGUI TextCounter;
    public TextMeshProUGUI TextCount;
    [SerializeField] private Image _icon;

    public void SetIcon(Sprite sprite)
    {
        _icon.sprite = sprite;
    }
}
