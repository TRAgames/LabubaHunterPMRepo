using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogObject
{
    public bool IsLeftSide;
    public Sprite Image;
    [Multiline] public string TextRu;
    [Multiline] public string TextEn;
    [Multiline] public string TextTr;
}
