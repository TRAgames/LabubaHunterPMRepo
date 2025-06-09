using DatabaseSystem.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelTypeManager : MonoBehaviour
{
    private LevelType _levelType;

    public LevelType LevelType { get => _levelType; set => _levelType = value; }

    public string GetLevelType()
    {
        return _levelType.Type;
    }
    public int GetCountEnemies()
    {
        return (_levelType as HoldingType).CountEnemies;
    }
    public int GetGoalLength()
    {
        return (_levelType as HoldingType).Goals.Length;
    }
    public AnimalData GetAnimalFromGoals(int index)
    {
        return  (_levelType as HoldingType).Goals[index].AnimalData;
    }
    public int GetCountFromGoals(int index)
    {
        return (_levelType as HoldingType).Goals[index].Count;
    }
}
