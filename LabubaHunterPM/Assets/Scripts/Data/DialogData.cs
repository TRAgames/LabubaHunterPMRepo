using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog")]
public class DialogData : ScriptableObject
{
    [SerializeField] private DialogObject[] _dialogObject;

    public DialogObject[] DialogObject { get => _dialogObject; set => _dialogObject = value; }
}
