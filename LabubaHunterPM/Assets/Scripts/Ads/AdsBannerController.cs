/*
 * This file is a part of the Yandex Advertising Network
 *
 * Version for Android (C) 2023 YANDEX
 *
 * You may not use this file except in compliance with the License.
 * You may obtain a copy of the License at https://legal.yandex.com/partner_ch/
 */

using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

public class AdsBannerController : MonoBehaviour
{
    private string message = "";

    private Banner banner;

    public static AdsBannerController Instance;

    private void Awake()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        RequestBanner();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);      
#endif
    }


    private void RequestBanner()
    {
        //Sets COPPA restriction for user age under 13
        MobileAds.SetAgeRestrictedUser(true);
        // Replace demo Unit ID 'demo-banner-yandex' with actual Ad Unit ID
        string adUnitId = "R-M-8034244-1";
        if (this.banner != null)
        {
            this.banner.Destroy();
        }
        // Set sticky banner width
        BannerAdSize bannerSize = BannerAdSize.StickySize(GetScreenWidthDp());
        // Or set inline banner maximum width and height
        //BannerAdSize bannerSize = BannerAdSize.InlineSize(GetScreenWidthDp(), 50);
        this.banner = new Banner(adUnitId, bannerSize, AdPosition.TopCenter);
        this.banner.OnAdLoaded += this.HandleAdLoaded;
        this.banner.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        this.banner.OnReturnedToApplication += this.HandleReturnedToApplication;
        this.banner.OnLeftApplication += this.HandleLeftApplication;
        this.banner.OnAdClicked += this.HandleAdClicked;
        this.banner.OnImpression += this.HandleImpression;
        this.banner.LoadAd(this.CreateAdRequest());
        //this.DisplayMessage("Banner is requested");
    }

    // Example how to get screen width for request
    private int GetScreenWidthDp()
    {
        int screenWidth = Screen.width;
        return ScreenUtils.ConvertPixelsToDp(screenWidth);
    }


    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder().Build();
    }

    private void DisplayMessage(string message)
    {
        this.message = message + (this.message.Length == 0 ? "" : "\n--------\n" + this.message);
        //GameObject textDebug = GameObject.Find("Text_Debug");
        //textDebug.GetComponent<TextMeshProUGUI>().text = message;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) RequestBanner();
    }

    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        this.DisplayMessage("HandleAdLoaded event received");
        this.banner.Show();
    }

    public void HandleAdFailedToLoad(object sender, AdFailureEventArgs args)
    {
        this.DisplayMessage("HandleAdFailedToLoad event received with message: " + args.Message);
    }

    public void HandleLeftApplication(object sender, EventArgs args)
    {
        this.DisplayMessage("HandleLeftApplication event received");
    }

    public void HandleReturnedToApplication(object sender, EventArgs args)
    {
        this.DisplayMessage("HandleReturnedToApplication event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        this.DisplayMessage("HandleAdLeftApplication event received");
    }

    public void HandleAdClicked(object sender, EventArgs args)
    {
        this.DisplayMessage("HandleAdClicked event received");
    }

    public void HandleImpression(object sender, ImpressionData impressionData)
    {
        var data = impressionData == null ? "null" : impressionData.rawData;
        this.DisplayMessage("HandleImpression event received with data: " + data);
    }

    #endregion
}
