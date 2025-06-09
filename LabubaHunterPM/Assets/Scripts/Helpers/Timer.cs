using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private int _timerCounter;

    public int TimerCounter { get => _timerCounter; set => _timerCounter = value; }

    public string Time(int timerCounter) 
    {
        return timerCounter < 10 ? "00:0" + timerCounter : "00:" + timerCounter;
    }
}
