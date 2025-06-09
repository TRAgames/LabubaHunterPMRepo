using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerksManager : MonoBehaviour
{
    [SerializeField] private Perk[] _perks;

    public Perk[] Perks { get => _perks; set => _perks = value; }

    public GameObject GetExtraWeaponPrefab(int index)
    {
        return (_perks[index] as GetExtraWeapon).ExtraWeapon.Prefab;
    }

    public GameObject GetAutoWeaponPrefab(int index)
    {
        return (_perks[index] as GetAutoWeapon).AutoWeapon.Prefab;
    }

    public int GetLengthPerks()
    {
        return _perks.Length - 1;
    }

    public string GetPerkType(int index)
    {
        return _perks[index].Type;
    }

    public string GetPerkName(int index)
    {
        return _perks[index].Name;
    }
    public Sprite GetPerkIcon(int index)
    {
        return _perks[index].Icon;
    }

    public float GetTime(int index)
    {
        return (_perks[index] as GetExtraWeapon).Time;
    }
    public float GetTimeBottom(int index)
    {
        return (_perks[index] as GetAutoWeapon).TimeBottom;
    }
    public float GetTimeTop(int index)
    {
        return (_perks[index] as GetAutoWeapon).TimeTop;
    }

    public int GetDamageBodyAutoWeapon(int index)
    {
        return (_perks[index] as GetAutoWeapon).AutoWeapon.DamageBody;
    }
    public int GetDamageHeadAutoWeapon(int index)
    {
        return (_perks[index] as GetAutoWeapon).AutoWeapon.DamageHead;
    }

    public void SetText(TextMeshProUGUI tmp, string text)
    {
        tmp.text = LocalizationManager.Instance.GetLocalizedValue(text);
    }

    public void SetTextCurrent(TextMeshProUGUI tmp, string text, float value, bool isAutoWeapon)
    {
        if (isAutoWeapon)
        {
            value = value >= 1 ? value : 0;
            tmp.text = value == 0 ? "" : LocalizationManager.Instance.GetLocalizedValue(text) + " " + value.ToString();
        }
        else
        {
            value = value >= 1 ? Mathf.Clamp(Mathf.Round((value - 1) * 100), 0, Mathf.Infinity) : Mathf.Clamp(Mathf.Round(value * 100), 0, Mathf.Infinity);
            tmp.text = value == 0 ? "" : LocalizationManager.Instance.GetLocalizedValue(text) + "<color=#AAE3FF>" + value.ToString() + "%</color>";
        }
    }

    public void SetImage(Image image, Sprite icon)
    {
        image.sprite = icon;
    }
}
