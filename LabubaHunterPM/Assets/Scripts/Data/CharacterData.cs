using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
public class CharacterData : ScriptableObject
{
    [SerializeField] private int _id;
    [Header("Информация о персонаже")]
    //[SerializeField] private string _characterNameKey;
    //[SerializeField] private Sprite _characterPortrait;
    //[SerializeField] private Sprite _bgCharacter;
    [SerializeField] private GameObject _prefab;
    //[Header("Специальная способность")]
    //[SerializeField] private string _descriptionSpecialKey;
    //[SerializeField] private Sprite _iconSpecial;
    //[SerializeField] private Sprite _bgSpecial;
    [Header("Принцип разблокировки героя")]
    [Tooltip("Заблокирован ли с начала")]
    [SerializeField] private bool _isBlocked;
    [Tooltip("Разблокировка за рекламу")]
    [SerializeField] private bool _isAdsUnblock;
    [Tooltip("Разблокировка за монеты")]
    [SerializeField] private bool _isPaidCoins;
    [Tooltip("Разблокировка за кристаллы")]
    [SerializeField] private bool _isPaidCrystals;
    [Tooltip("Стоимость разблокировки за валюту")]
    [SerializeField] private int _costUnblock;
    [Header("Множители")]
    [Tooltip("Урон")]
    [SerializeField] private float _damageMul;
    [Tooltip("Здоровье")]
    [SerializeField] private float _healthMul;
    [Header("Базовые значения")]
    [Tooltip("Урон в тело")]
    [SerializeField] private float _damageBodyBase;
    [Tooltip("Урон в голову")]
    [SerializeField] private float _damageHeadBase;
    [Tooltip("Скорострельность")]
    [SerializeField] private float _fireRateBase;
    [Tooltip("Время перезарядки")]
    [SerializeField] private float _reloadTime;
    [Tooltip("Магазин")]
    [SerializeField] private float _magazineBase;
    [Tooltip("Скорость")]
    [SerializeField] private float _speedBase;
    [Tooltip("Здоровье")]
    [SerializeField] private float _healthBase;
    [Header("Прокачка")]
    [SerializeField] private float _costUpgradeMul;
    [SerializeField] private float _costUpgradeBase;
    public int Id { get => _id;}
    public GameObject Prefab { get => _prefab; set => _prefab = value; }
    public float DamageMul { get => _damageMul; set => _damageMul = value; }
    public float HealthMul { get => _healthMul; set => _healthMul = value; }
    public float DamageBodyBase { get => _damageBodyBase; set => _damageBodyBase = value; }
    public float DamageHeadBase { get => _damageHeadBase; set => _damageHeadBase = value; }
    public float HealthBase { get => _healthBase; set => _healthBase = value; }
    public float CostUpgradeMul { get => _costUpgradeMul; set => _costUpgradeMul = value; }
    public float CostUpgradeBase { get => _costUpgradeBase; set => _costUpgradeBase = value; }
    public bool IsPaidCoins { get => _isPaidCoins; set => _isPaidCoins = value; }
    public int CostUnblock { get => _costUnblock; set => _costUnblock = value; }
    public bool IsPaidCrystals { get => _isPaidCrystals; set => _isPaidCrystals = value; }
    public bool IsBlocked { get => _isBlocked; set => _isBlocked = value; }
    public float FireRateBase { get => _fireRateBase; set => _fireRateBase = value; }
    public float MagazineBase { get => _magazineBase; set => _magazineBase = value; }
    public string SpeedBaseKey 
    {
        get 
        {
            if (_speedBase < 2) return "slow";
            else if (_speedBase >= 2 && _speedBase < 3) return "medium";
            else if (_speedBase >= 3) return "high";
            else return "medium";
        }  
    }
    public string FireRateKey
    {
        get
        {
            if (_fireRateBase >= 0.4) return "slow";
            else if (_fireRateBase >= 0.2 && _fireRateBase < 0.4) return "medium";
            else if (_fireRateBase < 0.2) return "fast";
            else return "medium";
        }
    }
    public float AverageFireRate
    {
        get
        {
            return (_fireRateBase * (_magazineBase - 1) + _reloadTime) / _magazineBase;
        }
    }
    //public string DescriptionSpecialKey { get => _descriptionSpecialKey; set => _descriptionSpecialKey = value; }
    //public Sprite IconSpecial { get => _iconSpecial; set => _iconSpecial = value; }
    public float SpeedBase { get => _speedBase; set => _speedBase = value; }
    //public Sprite BgSpecial { get => _bgSpecial; set => _bgSpecial = value; }
    //public string CharacterNameKey { get => _characterNameKey; set => _characterNameKey = value; }
    //public Sprite CharacterPortrait { get => _characterPortrait; set => _characterPortrait = value; }
    //public Sprite BgCharacter { get => _bgCharacter; set => _bgCharacter = value; }
    public bool IsAdsUnblock { get => _isAdsUnblock; set => _isAdsUnblock = value; }
}
