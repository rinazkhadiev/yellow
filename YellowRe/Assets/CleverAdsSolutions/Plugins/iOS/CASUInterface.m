//
//  CASUSettings.h
//  CASUnityPlugin
//
//  Copyright © 2020 Clever Ads Solutions. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CleverAdsSolutions/CleverAdsSolutions-Swift.h>
#import "CASUManager.h"
#import "CASUView.h"
#import "CASUTypes.h"
#import "CASUPluginUtil.h"
#import "CASUATTManager.h"

#pragma mark - CAS Settings

void CASUSetAnalyticsCollectionWithEnabled(BOOL enabled)
{
    [[CAS settings] setAnalyticsCollectionWithEnabled:enabled];
}

void CASUSetTestDeviceWithIds(const char **testDeviceIDs, int testDeviceIDLength)
{
    NSMutableArray *testDeviceIDsArray = [[NSMutableArray alloc] init];
    for (int i = 0; i < testDeviceIDLength; i++) {
        [testDeviceIDsArray addObject:[CASUPluginUtil stringFromUnity:testDeviceIDs[i]]];
    }
    [[CAS settings] setTestDeviceWithIds:testDeviceIDsArray];
}

void CASUSetBannerRefreshWithInterval(int interval)
{
    [[CAS settings] setBannerRefreshWithInterval:interval];
}

void CASUSetInterstitialWithInterval(int interval)
{
    [[CAS settings] setInterstitialWithInterval:interval];
}

void CASURestartInterstitialInterval(void)
{
    [[CAS settings] restartInterstitialInterval];
}

void CASUUpdateUserConsent(int consent)
{
    [[CAS settings] updateUserWithConsent:(CASConsentStatus)consent];
}

void CASUUpdateCCPAWithStatus(int doNotSell)
{
    [[CAS settings] updateCCPAWithStatus:(CASCCPAStatus)doNotSell];
}

void CASUSetTaggedWithAudience(int audience)
{
    [[CAS settings] setTaggedWithAudience:(CASAudience)audience];
}

void CASUSetDebugMode(BOOL mode)
{
    [[CAS settings] setDebugMode:mode];
}

void CASUSetMuteAdSoundsTo(BOOL muted)
{
    [[CAS settings] setMuteAdSoundsTo:muted];
}

void CASUSetLoadingWithMode(int mode)
{
    [[CAS settings] setLoadingWithMode:(CASLoadingManagerMode)mode];
}

void CASUSetInterstitialAdsWhenVideoCostAreLower(BOOL allow)
{
    [[CAS settings] setInterstitialAdsWhenVideoCostAreLowerWithAllow:allow];
}

void CASUSetiOSAppPauseOnBackground(BOOL pause)
{
    [CASUPluginUtil setPauseOnBackground:pause];
}

void CASUSetTrackLocationEnabled(BOOL enabled)
{
    [[CAS settings] setTrackLocationWithEnabled:enabled];
}

#pragma mark - User targeting options

void CASUSetUserGender(int gender)
{
    [[CAS targetingOptions] setGender:(Gender)gender];
}

void CASUSetUserAge(int age)
{
    [[CAS targetingOptions] setAge:age];
}

#pragma mark - Utils

void CASUValidateIntegration(void)
{
    [CAS validateIntegration];
}

void CASUOpenDebugger(CASUTypeManagerRef manager)
{
    UIStoryboard *storyboard =
        [UIStoryboard storyboardWithName:@"CASTestSuit"
                                  bundle:[NSBundle bundleForClass:[CASUManager class]]];
    if (!storyboard) {
        storyboard = [UIStoryboard storyboardWithName:@"CASDebugger"
                                               bundle:[NSBundle bundleForClass:[CASUManager class]]];
    }
    if (storyboard) {
        UIViewController *vc = [storyboard instantiateViewControllerWithIdentifier:@"DebuggerController"];
        if (vc) {
            UIViewController *root = [CASUPluginUtil unityGLViewController];
            SEL selector = NSSelectorFromString(@"setTargetManager:");
            if (![vc respondsToSelector:selector]) {
                NSLog(@"[CAS] Framework bridge cant connect to CASTestSuit");
                return;
            }

            CASUManager *internalManager = (__bridge CASUManager *)manager;
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Warc-performSelector-leaks"
            [vc performSelector:selector withObject:[internalManager casManager]];
#pragma clang diagnostic pop
            vc.modalPresentationStyle = UIModalPresentationFullScreen;
            [root presentViewController:vc animated:YES completion:nil];
            return;
        }
    }
    NSLog(@"[CAS] Framework bridge cant find CASDebugger");
}

const char * CASUGetActiveMediationPattern(void)
{
    return [CASUPluginUtil stringToUnity:[CASNetwork getActiveNetworkPattern]];
}

BOOL CASUIsActiveMediationNetwork(int net)
{
    NSArray *values = [CASNetwork values];
    if (net > 0 && net < [values count]) {
        return [CASNetwork isActiveNetwork:[values objectAtIndex:net]];
    }
    return NO;
}

const char * CASUGetSDKVersion(void)
{
    return [CASUPluginUtil stringToUnity:[CAS getSDKVersion]];
}

#pragma mark - CAS Manager

CASUTypeManagerRef CASUCreateBuilder(NSInteger  enableAd,
                                     BOOL       demoAd,
                                     const char *unityVersion,
                                     const char *userID)
{
    CASManagerBuilder *builder = [CAS buildManager];
    [builder withAdFlags:(CASTypeFlags)enableAd];
    [builder withTestAdMode:demoAd];
    [builder withFramework:@"Unity" version:[CASUPluginUtil stringFromUnity:unityVersion]];
    [builder withUserID:[CASUPluginUtil stringFromUnity:userID]];

    [[CASUPluginUtil sharedInstance] saveObject:builder withKey:@"lastBuilder"];

    return (__bridge CASUTypeManagerRef)builder;
}

void CASUSetMediationExtras(CASUTypeManagerBuilderRef builderRef,
                            const char                **extraKeys,
                            const char                **extraValues,
                            NSInteger                 extrasCount)
{
    CASManagerBuilder *builder = (__bridge CASManagerBuilder *)builderRef;
    for (int i = 0; i < extrasCount; i++) {
        [builder withMediationExtras:[CASUPluginUtil stringFromUnity:extraValues[i]]
                              forKey:[CASUPluginUtil stringFromUnity:extraKeys[i]]];
    }
}

CASUTypeManagerRef CASUInitializeManager(CASUTypeManagerBuilderRef          builderRef,
                                         CASUTypeManagerClientRef           *client,
                                         CASUInitializationCompleteCallback onInit,
                                         const char                         *identifier)
{
    NSString *nsIdentifier = [CASUPluginUtil stringFromUnity:identifier];

    CASUPluginUtil *cache = [CASUPluginUtil sharedInstance];
    [CASAnalytics setDelegate:cache]; // Require before create manager

    CASManagerBuilder *builder = (__bridge CASManagerBuilder *)builderRef;
    if (onInit) {
        [builder withCompletionHandler:^(id<CASInitialConfig> _Nonnull config) {
            onInit(client, [CASUPluginUtil stringToUnity:config.error], config.isShouldBeShownConsentDialog);
        }];
    }

    CASMediationManager *manager = [builder createWithCasId:nsIdentifier];
    CASUManager *wrapper = [[CASUManager alloc] initWithManager:manager forClient:client];
    [cache removeObjectWithKey:@"lastBuilder"];
    [cache saveObject:wrapper withKey:nsIdentifier];
    return (__bridge CASUTypeManagerRef)wrapper;
}

void CASUFreeManager(CASUTypeManagerRef managerRef)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    manager.casManager.adLoadDelegate = nil;
    manager.interCallback = nil;
    manager.rewardCallback = nil;
    manager.appReturnDelegate = nil;
    [manager disableReturnAds];
    CASUPluginUtil *cache = [CASUPluginUtil sharedInstance];
    [cache removeObjectWithKey:manager.casManager.managerID];
}

#pragma mark - General Ads functions
BOOL CASUIsAdEnabledType(CASUTypeManagerRef managerRef, int adType)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    return [manager.casManager isEnabledWithType:(CASType)adType];
}

void CASUEnableAdType(CASUTypeManagerRef managerRef, int adType, BOOL enable)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    [manager.casManager setEnabled:enable type:(CASType)adType];
}

void CASUSetLastPageAdContent(CASUTypeManagerRef managerRef, const char *contentJson)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    [manager setLastPageAdFor:[CASUPluginUtil stringFromUnity:contentJson]];
}

#pragma mark - Interstitial Ads

void CASUSetInterstitialDelegate(CASUTypeManagerRef                   managerRef,
                                 CASUDidLoadedAdCallback              didLoaded,
                                 CASUDidFailedAdCallback              didFailed,
                                 CASUWillOpeningWithMetaCallback      willOpen,
                                 CASUDidShowAdFailedWithErrorCallback didShowWithError,
                                 CASUDidClickedAdCallback             didClick,
                                 CASUDidClosedAdCallback              didClosed)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    manager.interCallback.didLoadedCallback = didLoaded;
    manager.interCallback.didFailedCallback = didFailed;
    manager.interCallback.willOpeningCallback = willOpen;
    manager.interCallback.didShowFailedCallback = didShowWithError;
    manager.interCallback.didClickCallback = didClick;
    manager.interCallback.didClosedCallback = didClosed;
}

void CASULoadInterstitial(CASUTypeManagerRef managerRef)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    [manager.casManager loadInterstitial];
}

BOOL CASUIsInterstitialReady(CASUTypeManagerRef managerRef)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    return manager.casManager.isInterstitialReady;
}

void CASUPresentInterstitial(CASUTypeManagerRef managerRef)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    [manager presentInter];
}

#pragma mark - Rewarded Ads

void CASUSetRewardedDelegate(CASUTypeManagerRef                   managerRef,
                             CASUDidLoadedAdCallback              didLoaded,
                             CASUDidFailedAdCallback              didFailed,
                             CASUWillOpeningWithMetaCallback      willOpen,
                             CASUDidShowAdFailedWithErrorCallback didShowWithError,
                             CASUDidClickedAdCallback             didClick,
                             CASUDidCompletedAdCallback           didComplete,
                             CASUDidClosedAdCallback              didClosed)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    manager.rewardCallback.didLoadedCallback = didLoaded;
    manager.rewardCallback.didFailedCallback = didFailed;
    manager.rewardCallback.willOpeningCallback = willOpen;
    manager.rewardCallback.didShowFailedCallback = didShowWithError;
    manager.rewardCallback.didClickCallback = didClick;
    manager.rewardCallback.didCompleteCallback = didComplete;
    manager.rewardCallback.didClosedCallback = didClosed;
}

void CASULoadReward(CASUTypeManagerRef managerRef)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    [manager.casManager loadRewardedAd];
}

BOOL CASUIsRewardedReady(CASUTypeManagerRef managerRef)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    return manager.casManager.isRewardedAdReady;
}

void CASUPresentRewarded(CASUTypeManagerRef managerRef)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    [manager presentReward];
}

#pragma mark - AdView

CASUTypeViewRef CASUCreateAdView(CASUTypeManagerRef    managerRef,
                                 CASUTypeViewClientRef *client,
                                 int                   adSizeCode)
{
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    CASUView *view = [[CASUView alloc] initWithManager:manager.casManager forClient:client size:adSizeCode];
    CASUPluginUtil *cache = [CASUPluginUtil sharedInstance];
    [cache saveObject:view withKey:[NSString stringWithFormat:@"%@_%d", manager.casManager.managerID, adSizeCode]];
    return (__bridge CASUTypeViewRef)view;
}

void CASUDestroyAdView(CASUTypeViewRef    viewRef,
                       CASUTypeManagerRef managerRef,
                       int                adSizeCode)
{
    CASUView *view = (__bridge CASUView *)viewRef;
    [view destroy];
    CASUManager *manager = (__bridge CASUManager *)managerRef;
    CASUPluginUtil *cache = [CASUPluginUtil sharedInstance];
    [cache removeObjectWithKey:[NSString stringWithFormat:@"%@_%d", manager.casManager.managerID, adSizeCode]];
}

void CASUAttachAdViewDelegate(CASUTypeViewRef                 viewRef,
                              CASUDidLoadedAdCallback         didLoad,
                              CASUDidFailedAdCallback         didFailed,
                              CASUWillOpeningWithMetaCallback willPresent,
                              CASUDidClickedAdCallback        didClicked)
{
    CASUView *view = (__bridge CASUView *)viewRef;
    view.adLoadedCallback = didLoad;
    view.adFailedCallback = didFailed;
    view.adPresentedCallback = willPresent;
    view.adClickedCallback = didClicked;
    [view attach];
}

void CASUPresentAdView(CASUTypeViewRef viewRef)
{
    CASUView *view = (__bridge CASUView *)viewRef;
    [view present];
}

void CASUHideAdView(CASUTypeViewRef viewRef)
{
    CASUView *view = (__bridge CASUView *)viewRef;
    [view hide];
}

void CASUSetAdViewPosition(CASUTypeViewRef viewRef, int posCode, int x, int y)
{
    CASUView *view = (__bridge CASUView *)viewRef;
    [view setPositionCode:posCode withX:x withY:y];
}

void CASUSetAdViewRefreshInterval(CASUTypeViewRef viewRef, int interval)
{
    CASUView *view = (__bridge CASUView *)viewRef;
    [view setRefreshInterval:interval];
}

void CASULoadAdView(CASUTypeViewRef viewRef)
{
    CASUView *view = (__bridge CASUView *)viewRef;
    [view load];
}

BOOL CASUIsAdViewReady(CASUTypeViewRef viewRef)
{
    CASUView *view = (__bridge CASUView *)viewRef;
    return [view isReady];
}

int CASUGetAdViewHeightInPixels(CASUTypeViewRef viewRef)
{
    CASUView *view = (__bridge CASUView *)viewRef;
    return view.heightInPixels;
}

int CASUGetAdViewWidthInPixels(CASUTypeViewRef viewRef)
{
    CASUView *view = (__bridge CASUView *)viewRef;
    return view.widthInPixels;
}

int CASUGetAdViewXOffsetInPixels(CASUTypeViewRef viewRef)
{
    CASUView *view = (__bridge CASUView *)viewRef;
    return view.xOffsetInPixels;
}

int CASUGetAdViewYOffsetInPixels(CASUTypeViewRef viewRef)
{
    CASUView *view = (__bridge CASUView *)viewRef;
    return view.yOffsetInPixels;
}

#pragma mark - App Return Ads

void CASUSetAppReturnDelegate(CASUTypeManagerRef                   manager,
                              CASUWillOpeningWithMetaCallback      willOpen,
                              CASUDidShowAdFailedWithErrorCallback didShowWithError,
                              CASUDidClickedAdCallback             didClick,
                              CASUDidClosedAdCallback              didClosed)
{
    CASUManager *internalManager = (__bridge CASUManager *)manager;
    internalManager.appReturnDelegate.willOpeningCallback = willOpen;
    internalManager.appReturnDelegate.didShowFailedCallback = didShowWithError;
    internalManager.appReturnDelegate.didClickCallback = didClick;
    internalManager.appReturnDelegate.didClosedCallback = didClosed;
}

void CASUEnableAppReturnAds(CASUTypeManagerRef manager)
{
    CASUManager *internalManager = (__bridge CASUManager *)manager;
    [internalManager enableReturnAds];
}

void CASUDisableAppReturnAds(CASUTypeManagerRef manager)
{
    CASUManager *internalManager = (__bridge CASUManager *)manager;
    [internalManager disableReturnAds];
}

void CASUSkipNextAppReturnAds(CASUTypeManagerRef manager)
{
    CASUManager *internalManager = (__bridge CASUManager *)manager;
    [internalManager skipNextAppReturnAd];
}

#pragma mark - ATT API

void CASURequestATT(CASUATTCompletion completion)
{
    [CASUATTManager trackingAuthorizationRequest:completion];
}

NSUInteger CASUGetATTStatus(void)
{
    return [CASUATTManager getTrackingAuthorizationStatus];
}
