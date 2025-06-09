using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    [SerializeField] private AnimalData _animalData;
    [SerializeField] bool _isAgressive;

    public bool CanHear()
    {
        return _animalData.CanHear;
    }
    public bool MustSeePlayer()
    {
        return _animalData.MustSeePlayer;
    }
    public bool IsAgressive()
    {
        return _isAgressive;
    }
    public float WalkSpeed()
    {
        return _animalData.WalkSpeed;
    }
    public float RunSpeed()
    {
        return _animalData.RunSpeed;
    }
    public float AttackSpeed()
    {
        return _animalData.AttackSpeed;
    }
    public AnimalData AnimalData() 
    {
        return _animalData;
    }


}
