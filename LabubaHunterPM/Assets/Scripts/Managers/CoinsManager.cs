using DatabaseSystem.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    private static CoinsManager _instance;
    public static CoinsManager Instance { get => _instance; set => _instance = value; }
    private void Awake()
    {
        _instance = this;
    }

    public void AddCoins(int value)
    {
        Progress.Instance.User.Coins += value;
#if !UNITY_EDITOR && UNITY_WEBGL
		Progress.Instance.Save();
#endif
    }

    public void SpendCoins(int value)
    {
        Progress.Instance.User.Coins -= value;
#if !UNITY_EDITOR && UNITY_WEBGL
		Progress.Instance.Save();
#endif
    }

    public bool IsEnoughCoins(int value)
    {
        return Progress.Instance.User.Coins >= value;
    }
}
