using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

public class AdsController : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void GetRewardExtern();
    [DllImport("__Internal")]
    private static extern void GetRewardUnblockCharacterExtern();
#endif

    [SerializeField] private PlayerStatsManager _playerStatsManager;
    [SerializeField] private bool _isRewardUnblockCharacter;

    private string message = "";

    private RewardedAdLoader rewardedAdLoader;
    private RewardedAd rewardedAd;

    public TextMeshProUGUI TextDebug;

    private int _isReward = 0;

    private bool _rewardCheck;

    private void Awake()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        this.rewardedAdLoader = new RewardedAdLoader();
        this.rewardedAdLoader.OnAdLoaded += this.HandleAdLoaded;
        this.rewardedAdLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;

        RequestRewardedAd();  
#endif
    }

    public int IsReward {
        get 
        {
#if UNITY_EDITOR
            return 1;
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
            return _isReward;
#endif
#if !UNITY_EDITOR && UNITY_WEBGL
            return _isReward;
#endif
        }
        set => _isReward = value; }

    public bool IsRewardUnblockCharacter { get => _isRewardUnblockCharacter; set => _isRewardUnblockCharacter = value; }
    public bool RewardCheck { get => _rewardCheck; set => _rewardCheck = value; }

    public void RewardedCheck()
    {
        _rewardCheck = true;
    }

    public void Rewarded()
    {
        if (_rewardCheck)
            _isReward = 1;         
        else if (!_rewardCheck)
            _isReward = -1;
    }

    public void GetReward()
    {
        _isReward = 0;
#if !UNITY_EDITOR && UNITY_WEBGL
        GetRewardExtern();
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
        ShowRewardedAd();
#endif
    }
    public void GetRewardUnblockCharacter()
    {
        _isReward = 0;
        _isRewardUnblockCharacter = true;
#if UNITY_EDITOR
        RewardedCheck();
        Rewarded();
        _playerStatsManager.UnblockRewardCharacter();
#endif
#if !UNITY_EDITOR && UNITY_WEBGL
        GetRewardUnblockCharacterExtern();       
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
        ShowRewardedAdUnblockCharacter();
#endif
    }

    private void RequestRewardedAd()
    {
        this.DisplayMessage("RewardedAd is not ready yet");
        //Sets COPPA restriction for user age under 13
        MobileAds.SetAgeRestrictedUser(true);

        if (this.rewardedAd != null)
        {
            this.rewardedAd.Destroy();
        }

        // Replace demo Unit ID 'demo-rewarded-yandex' with actual Ad Unit ID
        string adUnitId = "R-M-8034244-3";

        this.rewardedAdLoader.LoadAd(this.CreateAdRequest(adUnitId));
        this.DisplayMessage("Rewarded Ad is requested");
    }

    private void ShowRewardedAd()
    {
        DisplayMessage("RewardedAd show 0");
        if (this.rewardedAd == null)
        {
            this.DisplayMessage("RewardedAd is not ready yet");
            Rewarded();
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

    private void ShowRewardedAdUnblockCharacter()
    {
        DisplayMessage("RewardedAd show 0");
        if (this.rewardedAd == null)
        {
            this.DisplayMessage("RewardedAd is not ready yet");
            Rewarded();
            return;
        }
        DisplayMessage("RewardedAd show 1");
        this.rewardedAd.OnAdClicked += this.HandleAdClicked;
        this.rewardedAd.OnAdShown += this.HandleAdShown;
        this.rewardedAd.OnAdFailedToShow += this.HandleAdFailedToShow;
        this.rewardedAd.OnAdImpression += this.HandleImpression;
        this.rewardedAd.OnAdDismissed += this.HandleAdDismissed;
        this.rewardedAd.OnRewarded += this.HandleRewardedUnblockCharacter;
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
        if (TextDebug)
            TextDebug.text = message;
    }

    #region Rewarded Ad callback handlers

    public void HandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
    {
        this.DisplayMessage("HandleAdLoaded event received");
        this.rewardedAd = args.RewardedAd;
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        this.DisplayMessage(
            $"HandleAdFailedToLoad event received with message: {args.Message}");

        Rewarded();

        this.rewardedAd.Destroy();
        this.rewardedAd = null;

        RequestRewardedAd();
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
        Rewarded();

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

        RewardedCheck();
        Rewarded();

        this.rewardedAd.Destroy();
        this.rewardedAd = null;

        RequestRewardedAd();
    }

    public void HandleRewardedUnblockCharacter(object sender, Reward args)
    {
        this.DisplayMessage($"HandleRewarded event received: amout = {args.amount}, type = {args.type}");

        RewardedCheck();
        Rewarded();
        _playerStatsManager.UnblockRewardCharacter();

        this.rewardedAd.Destroy();
        this.rewardedAd = null;

        RequestRewardedAd();
    }

    public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
    {
        this.DisplayMessage(
            $"HandleAdFailedToShow event received with message: {args.Message}");

        Rewarded();

        this.rewardedAd.Destroy();
        this.rewardedAd = null;

        RequestRewardedAd();
    }

    #endregion
}
