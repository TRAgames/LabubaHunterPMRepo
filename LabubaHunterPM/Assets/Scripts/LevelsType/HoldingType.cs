using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Type/Holding")]
public class HoldingType : LevelType
{
    [Header("���������� ������� ��� ���������� ������")]
    private int _countEnemies;
    [Header("���� ��� �������� �� ������")]
    public AnimalGoal[] Goals;
    public override string Type { get => "Holding"; }
    public int CountEnemies { get => _countEnemies; set => _countEnemies = value; }
}

[Serializable]
public class AnimalGoal
{
    public AnimalData AnimalData;
    public int Count;
}
