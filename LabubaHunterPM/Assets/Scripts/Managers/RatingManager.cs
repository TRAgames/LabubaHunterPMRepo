using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatingManager : MonoBehaviour
{
    private static RatingManager _instance;
    public static RatingManager Instance { get => _instance; set => _instance = value; }
    private void Awake()
    {
        _instance = this;
    }

    public void AddRating(int value)
    {
        Progress.Instance.User.Rating += value;
#if !UNITY_EDITOR && UNITY_WEBGL
		Progress.Instance.Save();
#endif
    }

    public void SpendRating(int value)
    {
        Progress.Instance.User.Rating -= value;
#if !UNITY_EDITOR && UNITY_WEBGL
		Progress.Instance.Save();
#endif
    }

    public bool IsEnoughRating(int value)
    {
        return Progress.Instance.User.Rating >= value;
    }
}
