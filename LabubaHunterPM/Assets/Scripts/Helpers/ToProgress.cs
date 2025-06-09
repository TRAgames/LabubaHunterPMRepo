using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ToProgress : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SetLeaderboardExtern(int value);
#endif

    public void SetLeaderboard(int value)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        SetLeaderboardExtern(value);
#endif
    }
    public void Save()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
		Progress.Instance.Save();  
#endif
    }
    public void LevelFinishedSend(string name)
    {
        var eventParams = new Dictionary<string, string>
            {
                { "level_finished", name }
            };
        YandexMetrica.Send("level_finished", eventParams);
    }
}
