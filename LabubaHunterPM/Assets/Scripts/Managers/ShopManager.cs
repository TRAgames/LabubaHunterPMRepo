using RuStore;
using RuStore.BillingClient;
using RuStore.Example.UI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void BuyCrystalsExtern(int value);
#endif

    //[SerializeField] private TextMeshProUGUI _debugText;

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
         RuStoreBillingClient.Instance.Init();

        ConfirmPurchase("Crystals10", 10);
        ConfirmPurchase("Crystals50", 50);
        ConfirmPurchase("Crystals120", 120);
        ConfirmPurchase("Crystals250", 250);
#endif
    }
    public void OnButtonBuyCoins(ButtonShop buttonShop)
    {
        SoundController.Instance.PlayClick();
        if (CrystalsManager.Instance.IsEnoughCrystals(buttonShop.PriceValue))
        {
            CoinsManager.Instance.AddCoins(buttonShop.GoodValue);
            CrystalsManager.Instance.SpendCrystals(buttonShop.PriceValue);
            Launcher.Instance.CurrencyManager.GetPlayerCurrency();
        }
        else
        {
            Launcher.Instance.PopupNotEnough.SetPosition();
        }
    }

    public void OnButtonBuyCrystals(ButtonShop buttonShop)
    {
        SoundController.Instance.PlayClick();
#if !UNITY_EDITOR && UNITY_WEBGL
        BuyCrystalsExtern(buttonShop.GoodValue);
#endif
#if UNITY_EDITOR
        BuyCrystals(buttonShop.GoodValue);
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
        RuStoreBillingClient.Instance.PurchaseProduct(
            productId: "Crystals" + buttonShop.GoodValue,
            quantity: 1,
            developerPayload: "test payload",
                onFailure: (error) => {
                    },
                onSuccess: (result) => {
                    //BuyCrystals(buttonShop.GoodValue);
                    ConfirmPurchase("Crystals" + buttonShop.GoodValue, buttonShop.GoodValue);
            });
#endif
    }

    public void BuyCrystals(int value)
    {
        CrystalsManager.Instance.AddCrystals(value);
        Launcher.Instance.CurrencyManager.GetPlayerCurrency();
    }

    public void NotEnoughBuyCrystals()
    {
        Launcher.Instance.PopupNotEnough.SetPosition("no_purchase");
    }
    private void OnError(RuStoreError error)
    {
        //_debugText.text = error.name + ": " + error.description;
    }
    public void ConfirmPurchase(string purchaseId, int value)
    {
        RuStoreBillingClient.Instance.ConfirmPurchase(
            purchaseId: purchaseId,
            onFailure: (error) => {
            },
            onSuccess: () => {
                BuyCrystals(value);
            });
    }
}
