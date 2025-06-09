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
using TMPro;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

public class AdsRewardedController : MonoBehaviour
{
    private string message = "";

    private RewardedAdLoader rewardedAdLoader;
    private RewardedAd rewardedAd;

    public bool IsRewarded = false;
    public TextMeshProUGUI TextDebug;

    public static AdsRewardedController Instance;

    private void Awake()
    {
        this.rewardedAdLoader = new RewardedAdLoader();
        this.rewardedAdLoader.OnAdLoaded += this.HandleAdLoaded;
        this.rewardedAdLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;

#if !UNITY_EDITOR && UNITY_ANDROID
        RequestRewardedAd();  
#endif
    }

    public void RequestRewardedAd()
    {
        IsRewarded = false;
        this.DisplayMessage("RewardedAd is not ready yet");
        //Sets COPPA restriction for user age under 13
        MobileAds.SetAgeRestrictedUser(true);

        if (this.rewardedAd != null)
        {
            this.rewardedAd.Destroy();
        }

        // Replace demo Unit ID 'demo-rewarded-yandex' with actual Ad Unit ID
        string adUnitId = "demo-rewarded-yandex";

        this.rewardedAdLoader.LoadAd(this.CreateAdRequest(adUnitId));
        this.DisplayMessage("Rewarded Ad is requested");
    }

    public void ShowRewardedAd()
    {
        DisplayMessage("RewardedAd show 0"); 
        if (this.rewardedAd == null)
        {
            this.DisplayMessage("RewardedAd is not ready yet");
            return;
        }
        DisplayMessage("RewardedAd show 1");
        this.rewardedAd.OnAdClicked += this.HandleAdClicked;
        this.rewardedAd.OnAdShown += this.HandleAdShown;
        this.rewardedAd.OnAdFailedToShow += this.HandleAdFailedToShow;
        this.rewardedAd.OnAdImpression += this.HandleImpression;
        this.rewardedAd.OnAdDismissed += this.HandleAdDismissed;
        this.rewardedAd.OnRewarded += this.HandleRewarded;
        DisplayMessage("RewardedAd show 2");
        this.rewardedAd.Show();
    }

    private AdRequestConfiguration CreateAdRequest(string adUnitId)
    {
        return new AdRequestConfiguration.Builder(adUnitId).Build();
    }

    private void DisplayMessage(string message)
    {
        this.message = message + (this.message.Length == 0 ? "" : "\n--------\n" + this.message);
        TextDebug.text = message;
    }

    #region Rewarded Ad callback handlers

    public void HandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
    {
        this.DisplayMessage("HandleAdLoaded event received");
        this.rewardedAd = args.RewardedAd;
        DisplayMessage("test");
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        this.DisplayMessage(
            $"HandleAdFailedToLoad event received with message: {args.Message}");
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
        // Clear resources after an ad dismissed.

        // Now you can preload the next rewarded ad.
        

        this.rewardedAd.Destroy();
        this.rewardedAd = null;

        RequestRewardedAd();
    }

    public void HandleImpression(object sender, ImpressionData impressionData)
    {
        var data = impressionData == null ? "null" : impressionData.rawData;
        this.DisplayMessage($"HandleImpression event received with data: {data}");
    }

    public void HandleRewarded(object sender, Reward args)
    {
        this.DisplayMessage($"HandleRewarded event received: amout = {args.amount}, type = {args.type}");

        //IsRewarded = true;

        //this.rewardedAd.Destroy();
        //this.rewardedAd = null;

        //RequestRewardedAd();
    }

    public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
    {
        this.DisplayMessage(
            $"HandleAdFailedToShow event received with message: {args.Message}");
        this.rewardedAd.Destroy();
        this.rewardedAd = null;

        RequestRewardedAd();
    }

    #endregion
}
