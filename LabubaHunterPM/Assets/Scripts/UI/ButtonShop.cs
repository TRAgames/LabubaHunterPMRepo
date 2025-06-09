using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonShop : MonoBehaviour
{
    [Header("Текстовые поля на кнопке")]
    [SerializeField] private TextMeshProUGUI _good;
    [SerializeField] private TextMeshProUGUI _price;
    [Header("Значения текстовых полей")]
    [SerializeField] private int _goodValue;
    [SerializeField] private int _priceValue;

    public int GoodValue { get => _goodValue; set => _goodValue = value; }
    public int PriceValue { get => _priceValue; set => _priceValue = value; }

    private void OnEnable()
    {
        _good.text = _goodValue.ToString();
        _price.text = _priceValue.ToString();
    }
}
