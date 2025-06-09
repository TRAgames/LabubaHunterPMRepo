using DatabaseSystem.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Type/Survive")]
public class SurviveType : LevelType
{
    public bool IsEndless = false;
    public int MaxWave;

    public LevelData SpecialLevel;
    public override string Type { get => "Survive"; }
}
