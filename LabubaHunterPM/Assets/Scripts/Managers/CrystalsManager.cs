using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalsManager : MonoBehaviour
{
    private static CrystalsManager _instance;
    public static CrystalsManager Instance { get => _instance; set => _instance = value; }
    private void Awake()
    {
        _instance = this;
    }

    public void AddCrystals(int value)
    {
        Progress.Instance.User.Crystals += value;
#if !UNITY_EDITOR && UNITY_WEBGL
		Progress.Instance.Save();
#endif
    }

    public void SpendCrystals(int value)
    {
        Progress.Instance.User.Crystals -= value;
#if !UNITY_EDITOR && UNITY_WEBGL
		Progress.Instance.Save();
#endif
    }

    public bool IsEnoughCrystals(int value)
    {
        return Progress.Instance.User.Crystals >= value;
    }
}
