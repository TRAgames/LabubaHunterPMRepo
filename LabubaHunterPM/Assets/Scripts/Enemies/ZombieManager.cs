using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField] private Zombie _zombie;
    [SerializeField] private bool _overrideCoinsForKills;
    [SerializeField] private int _coinsForKills;
    [SerializeField] private float _runSpeed = 3;
    [SerializeField] private float _walkSpeed = 1.2f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private float _chanceApply;
    [SerializeField] private ZombieType _type;
    [SerializeField] private bool _isMustSeePlayer;

    public float RunSpeed { get => _runSpeed; set => _runSpeed = value; }
    public float WalkSpeed { get => _walkSpeed; set => _walkSpeed = value; }
    public float AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }
    public ZombieType Type { get => _type; set => _type = value; }
    public float ChanceApply { get => _chanceApply; set => _chanceApply = value; }
    public bool OverrideCoinsForKills { get => _overrideCoinsForKills; set => _overrideCoinsForKills = value; }
    public int CoinsForKills { get => _coinsForKills; set => _coinsForKills = value; }

    public string GetZombieType()
    {
        return _zombie.Type;
    }

    public string GetMaterialName()
    {
        return (_zombie as ZombieSimple).Material.name;
    }

    public bool IsRunner()
    {
        return _type == ZombieType.Runner ? true : false;
    }
    public bool IsCrawler()
    {
        return _type == ZombieType.Crawler ? true : false;
    }
    public bool IsRunnerCrawler()
    {
        return _type == ZombieType.RunnerCrawler ? true : false;
    }

    public bool IsMustSeePlayer()
    {
        return _isMustSeePlayer;
    }

}

public enum ZombieType
{
    Runner,
    Crawler,
    Walking,
    RunnerCrawler
}
