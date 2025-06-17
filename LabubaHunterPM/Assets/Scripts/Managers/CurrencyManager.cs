using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textCoins;
    [SerializeField] private TextMeshProUGUI _textBoxes;
    //[SerializeField] private TextMeshProUGUI _textCrystals;
    //[SerializeField] private TextMeshProUGUI _textRaiting;
    [SerializeField] private PlayerStatsManager _playerStatsManager;

    public void GetPlayerCurrency()
    {
        _textCoins.text = Progress.Instance.User.Coins.ToString();
        _textBoxes.text = Progress.Instance.User.Boxes.ToString();
        //_textCrystals.text = Progress.Instance.User.Crystals.ToString();
        //_textRaiting.text = Progress.Instance.User.Rating.ToString(); //Progress.Instance.User.CountMaxWave.ToString();
    }

    public void OnAddCurrencies()
    {
        CoinsManager.Instance.AddCoins(10000);
        CrystalsManager.Instance.AddCrystals(1000);
        GetPlayerCurrency();
        _playerStatsManager.GetPlayerStats(Progress.Instance.User.CurrentCharacter);
    }
}
