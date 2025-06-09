using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsManager : MonoBehaviour
{
    [Header("Текстовые поля способностей")]
    //[SerializeField] private TextMeshProUGUI _textPlayerLevel;
    //[SerializeField] private TextMeshProUGUI _textDamageBody;
    [SerializeField] private TextMeshProUGUI _textDPS;
    [SerializeField] private TextMeshProUGUI _textDPSAdd;
    //[SerializeField] private TextMeshProUGUI _textDamageBodyAdd;
    //[SerializeField] private TextMeshProUGUI _textDamageHead;
    //[SerializeField] private TextMeshProUGUI _textDamageHeadAdd;
    //[SerializeField] private TextMeshProUGUI _textFireRate;
    //[SerializeField] private TextMeshProUGUI _textSpeed;
    //[SerializeField] private TextMeshProUGUI _textHealth;
    //[SerializeField] private TextMeshProUGUI _textHealthAdd;
    [SerializeField] private TextMeshProUGUI _textCostUpgrade;
    //[Header("Специальная способность")]
    //[SerializeField] private TextMeshProUGUI _textSpecial;
    //[SerializeField] private Image _imageSpecial;
    //[SerializeField] private Image _bgSpecial;
    //[Header("Иконка героя")]
    //[SerializeField] private TextMeshProUGUI _textCharacterName;
    //[SerializeField] private Image _imageCharacterPortrait;
    //[SerializeField] private Image _bgCharacter;
    [Header("Герои")]
    [SerializeField] private CharacterData[] _characterData;
    [Header("Менеджер валют")]
    [SerializeField] private CurrencyManager _currencyManager;
    [Header("Место спавна героя в лобби")]
    [SerializeField] private Transform _characterTransform;
    [Header("Покупка героя")]
    [SerializeField] private string _blockTextKey;
    [SerializeField] private string _upgradeTextKey;
    [SerializeField] private TextMeshProUGUI _textBlock;
    [SerializeField] private Button _btnUpgrade;
    [SerializeField] private Color _colorActive;
    [SerializeField] private Color _colorUnactive;
    [SerializeField] private Color _colorUnactiveAds;
    [SerializeField] private Image _imageCoin;
    [SerializeField] private Image _imageCrystal;
    [SerializeField] private Image _imageAds;
    [SerializeField] private AdsController _adsController;
    //[SerializeField] private GameObject _levelUpFX;
    [Header("Нехватка ресурсов")]
    [SerializeField] private string _notEnoughKey;
    [SerializeField] private Button _btnPlay;
    //[SerializeField] private Button _btnSlaughter;
    [SerializeField] private TextMeshProUGUI _textPlay;
    [SerializeField] private TextMeshProUGUI _textSlaughter;
    [SerializeField] private Color _colorActivePlay;
    [SerializeField] private Color _colorActiveSlaughter;
    [SerializeField] private Color _colorActiveSlaughterText;

    private int _dPS;
    private int _damageBody;
    private int _damageHead;
    private int _health;
    private float _fireRate;
    private string _fireRateTitle;
    private float _magazine;
    private float _speed;
    private string _speedTitle;
    private int _costUpgrade;
    private int _costUnblock;
    private List<int> _realID;

    private GameObject[] _character;

    private int _currentID;
    private int _currentIndex;

    public void GetPlayerStats(int index)
    {
        _imageAds.gameObject.SetActive(false);
        
        int currentID = index;
        _currentID = Progress.Instance.User.CurrentCharacter;

        index = _realID.IndexOf(index);
        _currentIndex = index;

        //_textPlayerLevel.text = Progress.Instance.User.GetPlayerLevel(currentID).ToString();

        _fireRate = _characterData[index].FireRateBase;
        _fireRateTitle = LocalizationManager.Instance.GetLocalizedValue(_characterData[index].FireRateKey);
        _magazine = _characterData[index].MagazineBase;
        _speed = _characterData[index].SpeedBase;
        _speedTitle = LocalizationManager.Instance.GetLocalizedValue(_characterData[index].SpeedBaseKey);

        //левый блок, способность персонажа
        //_imageSpecial.sprite = _characterData[index].IconSpecial;
        //_textSpecial.text = LocalizationManager.Instance.GetLocalizedValue(_characterData[index].DescriptionSpecialKey).ToString();
        //_bgSpecial.sprite = _characterData[index].BgSpecial;

        //правый блок, имя и иконка персонажа
        //_textCharacterName.text = LocalizationManager.Instance.GetLocalizedValue(_characterData[index].CharacterNameKey).ToString();
        //_imageCharacterPortrait.sprite = _characterData[index].CharacterPortrait;
        //_bgCharacter.sprite = _characterData[index].BgCharacter;

        float damageMultiplierResultNext = GetMultiplierResult(_characterData[index].DamageMul, currentID, 1);
        float healthMultiplierResultNext = GetMultiplierResult(_characterData[index].HealthMul, currentID, 1);

        int damageBodyNext = (int)Mathf.Round(damageMultiplierResultNext * _characterData[index].DamageBodyBase);
        int damageHeadNext = (int)Mathf.Round(damageMultiplierResultNext * _characterData[index].DamageHeadBase);
        int healthNext = (int)Mathf.Round(healthMultiplierResultNext * _characterData[index].HealthBase);
        int dPSNext = (int)Mathf.Round(((damageBodyNext / _characterData[index].AverageFireRate) + (damageHeadNext / _characterData[index].AverageFireRate)) / 2);

        float damageMultiplierResult = GetMultiplierResult(_characterData[index].DamageMul, currentID);
        float healthMultiplierResult = GetMultiplierResult(_characterData[index].HealthMul, currentID);
        float costMultiplierResult = GetLogMultiplierResult(_characterData[index].CostUpgradeMul, currentID);

        _damageBody = (int)Mathf.Round(damageMultiplierResult * _characterData[index].DamageBodyBase);
        _damageHead = (int)Mathf.Round(damageMultiplierResult * _characterData[index].DamageHeadBase);
        _dPS = (int)Mathf.Round(((_damageBody / _characterData[index].AverageFireRate) + (_damageHead / _characterData[index].AverageFireRate))/2);
        _health = (int)Mathf.Round(healthMultiplierResult * _characterData[index].HealthBase);
        _costUpgrade = (int)Mathf.Round(costMultiplierResult * _characterData[index].CostUpgradeBase);
        _costUnblock = _characterData[index].CostUnblock;

        _textDPS.text = _dPS.ToString();
        //_textDamageBody.text = _damageBody.ToString();
        //_textDamageHead.text = _damageHead.ToString();
        //_textHealth.text = _health.ToString();

        _textDPSAdd.text = "+" + (dPSNext - _dPS).ToString();
        //_textDamageBodyAdd.text = "+" + (damageBodyNext - _damageBody).ToString();
        //_textDamageHeadAdd.text = "+" + (damageHeadNext - _damageHead).ToString();
        //_textHealthAdd.text = "+" + (healthNext - _health).ToString();

        //_textFireRate.text = _fireRateTitle;
        //_textSpeed.text = _speedTitle;

        if (_characterData[index].IsBlocked)//проверяем заблочен ли по-умолчанию
        {
            _btnUpgrade.gameObject.SetActive(true);
            if (!Progress.Instance.User.GetUnblockStatus(currentID)) //если заблокирован у игрока
            {
                if (_characterData[index].IsPaidCoins)
                {
                    CheckUnblock(CoinsManager.Instance.IsEnoughCoins(_costUnblock), _blockTextKey, _costUnblock, true);
                }
                else if (_characterData[index].IsPaidCrystals)
                {
                    CheckUnblock(CrystalsManager.Instance.IsEnoughCrystals(_costUnblock), _blockTextKey, _costUnblock, false);
                }
                else if (_characterData[index].IsAdsUnblock)
                {
                    _btnUpgrade.SetActive(_textCostUpgrade, _colorUnactiveAds, Color.white);
                    _textBlock.color = Color.white;
                    _textCostUpgrade.text = LocalizationManager.Instance.GetLocalizedValue("ads");
                    _imageCoin.gameObject.SetActive(false);
                    _imageCrystal.gameObject.SetActive(false);
                    _imageAds.gameObject.SetActive(true);
                    _textBlock.text = LocalizationManager.Instance.GetLocalizedValue(_blockTextKey);
                }
                _btnPlay.SetUnactive(_textPlay, _colorUnactive);
                //_btnSlaughter.SetUnactive(_textSlaughter, _colorUnactive);
            }
            else
            {
                _btnUpgrade.gameObject.SetActive(false);
                _btnPlay.SetActive(_textPlay, _colorActivePlay, Color.white);
            }
        }
        else
        {
            _btnUpgrade.gameObject.SetActive(false);
            Progress.Instance.User.SetUnblockStatus(currentID);
            _btnPlay.SetActive(_textPlay, _colorActivePlay, Color.white);
        }

        _character[_realID.IndexOf(Progress.Instance.User.CurrentCharacter)].SetActive(true);

        PlayerPrefs.SetInt("DAMAGE_BODY", _damageBody);
        PlayerPrefs.SetInt("DAMAGE_HEAD", _damageHead);
        PlayerPrefs.SetInt("HEALTH", _health);
        PlayerPrefs.SetFloat("FIRE_RATE", _fireRate);
        PlayerPrefs.SetFloat("MAGAZINE", _magazine);
        PlayerPrefs.SetFloat("SPEED", _speed);
    }

    private float GetMultiplierResult(float mul, int index, int addIndex)
    {
        return Mathf.Pow(mul + 1, Progress.Instance.User.GetPlayerLevel(index) + addIndex);
    }
    private float GetMultiplierResult(float mul, int index)
    {
        return Mathf.Pow(mul + 1, Progress.Instance.User.GetPlayerLevel(index));
    }

    private float GetLogMultiplierResult(float mul, int index)
    {
        return (Mathf.Log(Progress.Instance.User.GetPlayerLevel(index), mul) + 1) * Progress.Instance.User.GetPlayerLevel(index);
    }

    private void CheckUnblock(bool condition, string locKey, int cost, bool isCoins)
    {
        if (condition)
        {
            _btnUpgrade.SetActive(_textCostUpgrade, _colorActive, Color.white);
            _textBlock.color = Color.white;
        }            
        else
        {
            _btnUpgrade.SetUnactive(_textCostUpgrade, _colorUnactive);
            _textBlock.color = Color.red;
        }           

        _textCostUpgrade.text = cost.ToString();
        _imageCoin.gameObject.SetActive(isCoins);
        _imageCrystal.gameObject.SetActive(!isCoins);
        _textBlock.text = LocalizationManager.Instance.GetLocalizedValue(locKey);
    }

    public bool OnUpgradePlayerLevel()
    {        
        SoundController.Instance.PlayClick();
        if (Progress.Instance.User.GetUnblockStatus(_currentID))
        {
            if (CoinsManager.Instance.IsEnoughCoins(_costUpgrade))
            {                
                //Progress.Instance.User.SetPlayerLevel(_currentID);
                CoinsManager.Instance.SpendCoins(_costUpgrade);
                _currencyManager.GetPlayerCurrency();
                GetPlayerStats(_currentID);
                SoundController.Instance.PlayLevelUp();
                return true;
            }
            else
            {
                Launcher.Instance.PopupNotEnough.SetPosition(_notEnoughKey);
                Debug.Log("NOT ENOUGH MONEY FOR UPGRADE");
                return false;
            }
        }
        else
        {
            if (_characterData[_currentIndex].IsPaidCoins)
            {
                if (CoinsManager.Instance.IsEnoughCoins(_costUnblock))
                {                   
                    Progress.Instance.User.SetUnblockStatus(_currentID);
                    CoinsManager.Instance.SpendCoins(_costUnblock);
                    _currencyManager.GetPlayerCurrency();
                    GetPlayerStats(_currentID);
                    SoundController.Instance.PlayLevelUp();
                    return true;
                }
                else
                {
                    Launcher.Instance.PopupNotEnough.SetPosition(_notEnoughKey);
                    Debug.Log("NOT ENOUGH MONEY FOR UNBLOCK");
                }
                return false;
            }
            else if (_characterData[_currentIndex].IsPaidCrystals)
            {
                if (CrystalsManager.Instance.IsEnoughCrystals(_costUnblock))
                {
                    Progress.Instance.User.SetUnblockStatus(_currentID);
                    CrystalsManager.Instance.SpendCrystals(_costUnblock);
                    _currencyManager.GetPlayerCurrency();
                    GetPlayerStats(_currentID);
                    SoundController.Instance.PlayLevelUp();
                    return true;
                }
                else
                {
                    Launcher.Instance.PopupNotEnough.SetPosition(_notEnoughKey);
                    Debug.Log("NOT ENOUGH MONEY FOR UNBLOCK");
                }
                return false;
            }
            else if (_characterData[_currentIndex].IsAdsUnblock)
            {
                _adsController.GetRewardUnblockCharacter();               
                return false;
            }
        }
        return false;
    }

    public void UnblockRewardCharacter()
    {
        if (_adsController.IsReward == 1)
        {
            Progress.Instance.User.SetUnblockStatus(_currentID);
            GetPlayerStats(_currentID);
            SoundController.Instance.PlayLevelUp();
            _imageAds.gameObject.SetActive(false);
            _adsController.RewardCheck = false;
        }
    }

    public void OnChangeCharacter(int index)
    {
        SoundController.Instance.PlayClick();

        int x = _realID.IndexOf(Progress.Instance.User.CurrentCharacter);     

        _character[x].SetActive(false);
        x += index;
        x = x < 0 ? _characterData.Length - 1 : x % _characterData.Length;

        _currentIndex = x;

        Progress.Instance.User.CurrentCharacter = _realID[x];
        _currentID = Progress.Instance.User.CurrentCharacter;
        GetPlayerStats(Progress.Instance.User.CurrentCharacter);        
    }

    public void CreateCharactersModels()
    {
        _character = new GameObject[_characterData.Length];
        _realID = new List<int>();

        for (int i = 0; i < _characterData.Length; i++)
        {
            _character[i] = Instantiate(_characterData[i].Prefab);
            _character[i].transform.parent = _characterTransform;
            _character[i].transform.localPosition = Vector3.zero;
            _character[i].transform.localScale = Vector3.one;
            _character[i].SetActive(false);
            _realID.Add(_characterData[i].Id);
        }
        
    }

    public void UnactiveCurrentCharacter()
    {
        _character[_currentID].SetActive(false);
    }

}
