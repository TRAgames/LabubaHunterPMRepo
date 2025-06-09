using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupRecommend : MonoBehaviour
{
    [SerializeField] private Image _characterPortrait;

    public Image CharacterPortrait { get => _characterPortrait; set => _characterPortrait = value; }
}
