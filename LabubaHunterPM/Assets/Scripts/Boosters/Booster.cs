using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    [Header("����� �������� �������")]
    [SerializeField] private int _interval;

    public int Interval { get => _interval; set => _interval = value; }

    [Header("���������� ��������� �� ����������� �������")]
    [Tooltip("Result = base * (1 + Parameter)")]
    [SerializeField] private float _parameter;

    public float Parameter { get => _parameter; set => _parameter = value; }
}
