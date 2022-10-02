//
//  CASUManager.m
//  CASUnityPlugin
//
//  Copyright © 2020 Clever Ads Solutions. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "CASUManager.h"
#import "CASUPluginUtil.h"

@implementation CASUManager

- (id)initWithManager:(CASMediationManager *)manager forClient:(CASUTypeManagerClientRef  _Nullable *)client{
    self = [super init];
    if (self) {
        self.casManager = manager;
        _client = client;
        _interCallback = [[CASUCallback alloc] initWithComplete:false];
        _interCallback.client = client;
        _rewardCallback = [[CASUCallback alloc] initWithComplete:true];
        _rewardCallback.client = client;
        _appReturnDelegate = [[CASUCallback alloc] initWithComplete:false];
        _appReturnDelegate.client = client;
    }
    return self;
    
}

- (void)presentInter {
    [_casManager presentInterstitialFromRootViewController:[CASUPluginUtil unityGLViewController]
                                                  callback:_interCallback];
}

- (void)presentReward {
    [_casManager presentRewardedAdFromRootViewController:[CASUPluginUtil unityGLViewController]
                                                callback:_rewardCallback];
}

- (void)setLastPageAdFor:(NSString *)content {
    self.casManager.lastPageAdContent = [CASLastPageAdContent createFrom:content];
}

- (void)onAdLoaded:(enum CASType)adType {
    if (adType == CASTypeInterstitial) {
        if (self.interCallback) {
            if (self.interCallback.didLoadedCallback) {
                self.interCallback.didLoadedCallback(self.client);
            }
        }
    } else if (adType == CASTypeRewarded) {
        if (self.rewardCallback) {
            if (self.rewardCallback.didLoadedCallback) {
                self.rewardCallback.didLoadedCallback(self.client);
            }
        }
    }
}

- (void)onAdFailedToLoad:(enum CASType) adType withError:(NSString *)error {
    if (adType == CASTypeInterstitial) {
        if (self.interCallback) {
            if (self.interCallback.didFailedCallback) {
                self.interCallback.didFailedCallback(self.client, [self getErrorCodeFromString:error]);
            }
        }
    } else if (adType == CASTypeRewarded) {
        if (self.rewardCallback) {
            if (self.rewardCallback.didFailedCallback) {
                self.rewardCallback.didFailedCallback(self.client, [self getErrorCodeFromString:error]);
            }
        }
    }
}

- (void)enableReturnAds {
    [_casManager enableAppReturnAdsWith:_appReturnDelegate];
}

- (void)disableReturnAds {
    [_casManager disableAppReturnAds];
}

- (void)skipNextAppReturnAd {
    [_casManager skipNextAppReturnAds];
}

- (NSInteger)getErrorCodeFromString:(NSString *)error {
    if (!error) {
        return CASErrorInternalError;
    }

    return [error isEqualToString:@"No internet connection detected"] ? CASErrorNoConnection
        : [error isEqualToString:@"No Fill"] ? CASErrorNoFill
        : [error isEqualToString:@"Invalid configuration"] ? CASErrorConfigurationError
        : [error isEqualToString:@"Ad are not ready. You need to call Load ads or use one of the automatic cache mode."] ? CASErrorNotReady
        : [error isEqualToString:@"Manager is disabled"] ? CASErrorManagerIsDisabled
        : [error isEqualToString:@"Reached cap for user"] ? CASErrorReachedCap
        : [error isEqualToString:@"The interval between impressions Ad has not yet passed."] ? CASErrorIntervalNotYetPassed
        : [error isEqualToString:@"Ad already displayed"] ? CASErrorAlreadyDisplayed
        : [error isEqualToString:@"Application is paused"] ? CASErrorAppIsPaused
        : [error isEqualToString:@"Not enough space to display ads"] ? CASErrorNotEnoughSpace
        : CASErrorInternalError;
}

@end
