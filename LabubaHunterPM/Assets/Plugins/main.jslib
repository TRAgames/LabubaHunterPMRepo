mergeInto(LibraryManager.library, {

RateGame: function () {
  ysdk.feedback.canReview()
  .then(({ value, reason }) => {
    if (value) {
        ysdk.feedback.requestReview()
        .then(({ feedbackSent }) => {
            console.log(feedbackSent);
        })
    } else {
        console.log(reason)
    }
})
},

SetLeaderboardExtern: function (value) {
    ysdk.getLeaderboards()
        .then(lb => {
    lb.setLeaderboardScore('rating2', value);
  });
},

SaveExtern: function (data) {
        try {

    var dataString = UTF8ToString(data);
    var myobj = JSON.parse(dataString);
    player.setData(myobj);

    } catch (e) {

myGameInstance.SendMessage('Progress','SaveEmpty');
console.log("BAD SAVE")

        }
},

LoadExtern: function () {
        try {
        if (!PlayerExist) {
            myGameInstance.SendMessage('Progress', 'LoadEmpty'); 
            getLanguage();
            getTypeDevice();
       } else {
        player.getData().then(_data => {
            const myJSON = JSON.stringify(_data);
            myGameInstance.SendMessage('Progress', 'Load', myJSON); 
            getLanguage();
            getTypeDevice();
            GameReady();  
            myGameInstance.SendMessage('Progress', 'DeactivateBlackLoadingScreen'); 
            //myGameInstance.SendMessage('Launcher', 'TurnOffAllSound', osInfo.name);  
            console.log(osInfo.name + detectBrowser());
        });
       }
       } catch (e) {
    myGameInstance.SendMessage('Progress','LoadEmpty');
    getLanguage();
    getTypeDevice();
    GameReady(); 
}

},
BuyCrystalsExtern: function (value) {
    try {
        payments.purchase({ id: 'Crystals' + value }).then(purchase => {
            myGameInstance.SendMessage('ShopManager', 'BuyCrystals', value);
            payments.consumePurchase(purchase.purchaseToken);      
        }).catch(err => {
            //myGameInstance.SendMessage('ShopManager', 'NotEnoughBuyCrystals');
        })

    } catch (e) {
        //myGameInstance.SendMessage('PurchaseController', 'GetAllGirlsBadConnection');
    }
},

GetRewardExtern: function () {
ysdk.adv.showRewardedVideo({
    callbacks: {
        onOpen: () => {
          //myGameInstance.SendMessage('Progress', 'PauseMusic');
        },
        onRewarded: () => {
          myGameInstance.SendMessage('AdsController', 'RewardedCheck');
        },
        onClose: () => {
          myGameInstance.SendMessage('AdsController', 'Rewarded');
          //myGameInstance.SendMessage('Progress', 'UnpauseMusic');
        }, 
        onError: (e) => {
          myGameInstance.SendMessage('AdsController', 'Rewarded');
          //myGameInstance.SendMessage('Progress', 'UnpauseMusic');
        }
    }
})
},
GetRewardUnblockCharacterExtern: function () {
ysdk.adv.showRewardedVideo({
    callbacks: {
        onOpen: () => {
          //myGameInstance.SendMessage('Progress', 'PauseMusic');
        },
        onRewarded: () => {
          myGameInstance.SendMessage('AdsController', 'RewardedCheck');
        },
        onClose: () => {
          myGameInstance.SendMessage('AdsController', 'Rewarded');
          myGameInstance.SendMessage('PlayerStatsManager', 'UnblockRewardCharacter');
          //myGameInstance.SendMessage('Progress', 'UnpauseMusic');
        }, 
        onError: (e) => {
          myGameInstance.SendMessage('AdsController', 'Rewarded');
          //myGameInstance.SendMessage('Progress', 'UnpauseMusic');
        }
    }
})
},

FinishLevel: function () {
ysdk.adv.showFullscreenAdv({
    callbacks: {
        onOpen: function() {
          //myGameInstance.SendMessage('Progress', 'PauseMusic');
        },
        onClose: function(wasShown) {
          //myGameInstance.SendMessage('Progress', 'UnpauseMusic');
          myGameInstance.SendMessage('Progress', 'DeactivateBlackLoadingScreen');
        },
        onError: function(error) {
          myGameInstance.SendMessage('Progress', 'DeactivateBlackLoadingScreen');
        }
    }
})
},

FinishLevelReload: function () {
ysdk.adv.showFullscreenAdv({
    callbacks: {
        onOpen: function() {
          //myGameInstance.SendMessage('Progress', 'PauseMusic');
        },
        onClose: function(wasShown) {
          //myGameInstance.SendMessage('Progress', 'UnpauseMusic');
          myGameInstance.SendMessage('Progress', 'ReloadScene');
        },
        onError: function(error) {
          myGameInstance.SendMessage('Progress', 'ReloadScene');
        }
    }
})
},

});