using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private DialogData _data;

    public DialogData Data { get => _data; set => _data = value; }

    public int Length { get => _data.DialogObject.Length; }

    public bool GetSide(int index)
    {
        return _data.DialogObject[index].IsLeftSide;
    }
    public Sprite GetSprite(int index)
    {
        return _data.DialogObject[index].Image;
    }

    public string GetText(int index, string language)
    {
        switch (language)
        {
            case "ru": return _data.DialogObject[index].TextRu;
            case "en": return _data.DialogObject[index].TextEn;
            case "tr": return _data.DialogObject[index].TextTr;
            default: return _data.DialogObject[index].TextRu;
        }                  
    }
}
