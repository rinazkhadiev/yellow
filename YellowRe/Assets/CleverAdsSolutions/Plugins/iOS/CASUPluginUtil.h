//
//  CASUPluginUtil.h
//  CASUnityPlugin
//
//  Copyright © 2020 Clever Ads Solutions. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <CleverAdsSolutions/CleverAdsSolutions.h>

NS_ASSUME_NONNULL_BEGIN

@interface CASUPluginUtil : NSObject<CASAnalyticsDelegate>
+ (nonnull instancetype)sharedInstance;

- (void)saveObject:(nullable id)obj withKey:(nonnull NSString *)key;
- (void)removeObjectWithKey:(nonnull NSString *)key;

/// Returns an NSString copying the characters from |bytes|, a C array of UTF8-encoded bytes.
/// Returns nil if |bytes| is NULL.
+ (NSString *)stringFromUnity:(const char *_Nullable)bytes;
/// Returns a C string from a C array of UTF8-encoded bytes.
+ (const char *)stringToUnity:(NSString *)str;
+ (NSString *)adMetaDataToStringPointer:(id<CASStatusHandler>)ad;
+ (UIViewController *)unityGLViewController;
+ (void)onAdsWillPressent;
+ (void)onAdsDidClosed;

@property (class) BOOL pauseOnBackground;
@end

NS_ASSUME_NONNULL_END
