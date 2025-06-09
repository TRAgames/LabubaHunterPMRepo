/*
 * This file is a part of the Yandex Advertising Network
 *
 * Version for Android (C) 2023 YANDEX
 *
 * You may not use this file except in compliance with the License.
 * You may obtain a copy of the License at https://legal.yandex.com/partner_ch/
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

public class AdsInterstitialController : MonoBehaviour
{
    private string message = "";

    private InterstitialAdLoader interstitialAdLoader;
    private Interstitial interstitial;

    private void Awake()
    {
        this.interstitialAdLoader = new InterstitialAdLoader();
        this.interstitialAdLoader.OnAdLoaded += this.HandleAdLoaded;
        this.interstitialAdLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;
#if !UNITY_EDITOR && UNITY_ANDROID
        RequestInterstitial();
#endif

    }

    private void RequestInterstitial()
    {
        //Sets COPPA restriction for user age under 13
        MobileAds.SetAgeRestrictedUser(true);

        // Replace demo Unit ID 'demo-interstitial-yandex' with actual Ad Unit ID
        string adUnitId = "R-M-8034244-2";

        if (this.interstitial != null)
        {
            this.interstitial.Destroy();
        }

        this.interstitialAdLoader.LoadAd(this.CreateAdRequest(adUnitId));
        this.DisplayMessage("Interstitial is requested");
    }

    private void ShowInterstitial()
    {
        if (this.interstitial == null)
        {
            this.DisplayMessage("Interstitial is not ready yet");
            return;
        }

        this.interstitial.OnAdClicked += this.HandleAdClicked;
        this.interstitial.OnAdShown += this.HandleAdShown;
        this.interstitial.OnAdFailedToShow += this.HandleAdFailedToShow;
        this.interstitial.OnAdImpression += this.HandleImpression;
        this.interstitial.OnAdDismissed += this.HandleAdDismissed;

        this.interstitial.Show();
    }

    private AdRequestConfiguration CreateAdRequest(string adUnitId)
    {
        return new AdRequestConfiguration.Builder(adUnitId).Build();
    }

    private void DisplayMessage(string message)
    {
        this.message = message + (this.message.Length == 0 ? "" : "\n--------\n" + this.message);
    }

    #region Interstitial callback handlers

    public void HandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
    {
        this.DisplayMessage("HandleAdLoaded event received");

        this.interstitial = args.Interstitial;
        ShowInterstitial();
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        this.DisplayMessage($"HandleAdFailedToLoad event received with message: {args.Message}");
    }
    public void HandleAdClicked(object sender, EventArgs args)
    {
        this.DisplayMessage("HandleAdClicked event received");
    }

    public void HandleAdShown(object sender, EventArgs args)
    {
        this.DisplayMessage("HandleAdShown event received");
    }

    public void HandleAdDismissed(object sender, EventArgs args)
    {
        this.DisplayMessage("HandleAdDismissed event received");

        this.interstitial.Destroy();
        this.interstitial = null;
    }

    public void HandleImpression(object sender, ImpressionData impressionData)
    {
        var data = impressionData == null ? "null" : impressionData.rawData;
        this.DisplayMessage($"HandleImpression event received with data: {data}");
    }

    public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
    {
        this.DisplayMessage($"HandleAdFailedToShow event received with message: {args.Message}");
    }

    #endregion
}
