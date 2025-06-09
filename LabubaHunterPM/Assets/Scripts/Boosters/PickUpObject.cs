using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    [Header("���������� � ���������� (���������)")]
    [SerializeField] private int _add;

    public int Add { get => _add; set => _add = value; }

    [Header("���������� ��������� �� ����������� �������")]
    [Tooltip("Result = base * (1 + Parameter) + Add")]
    [SerializeField] private float _parameter;

    public float Parameter { get => _parameter; set => _parameter = value; }
}
