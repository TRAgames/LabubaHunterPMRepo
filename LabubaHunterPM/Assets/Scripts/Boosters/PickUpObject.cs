using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    [Header("Количество к добавлению (аддитивно)")]
    [SerializeField] private int _add;

    public int Add { get => _add; set => _add = value; }

    [Header("Увеличение параметра на определённый процент")]
    [Tooltip("Result = base * (1 + Parameter) + Add")]
    [SerializeField] private float _parameter;

    public float Parameter { get => _parameter; set => _parameter = value; }
}
