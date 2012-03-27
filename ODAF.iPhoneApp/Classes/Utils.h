//
//  Utils.h
//  VanGuide
//
//  Created by shazron on 09-12-03.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <MapKit/MapKit.h>

#define DEG_EPS 0.001
#define fequal(a,b) (fabs((a) - (b)) < DEG_EPS)
#define fequalzero(a) (fabs(a) < DEG_EPS)

@class LoadingView;

@interface Utils : NSObject {
	@private
	LoadingView* loadingView;
}

@property (retain) LoadingView* loadingView;

- (NSString*) webAppUrl;
- (NSString*) dateDiffFromNowToString:(NSDate*)originalDate withThreshold:(BOOL)withThreshold;
- (NSDate*) dateFromJsDate:(NSString*)jsDate;
- (NSString*) generateUUID;

- (id) getConfigSetting:(NSString*)key;
- (NSDictionary*) createLoadingViewOptionsFullScreen:(BOOL)fullScreen withText:(NSString*)text;

- (BOOL) locationEquals:(CLLocationCoordinate2D)left withLocation:(CLLocationCoordinate2D)right;

- (NSMutableDictionary*) getPlist:(NSString*)plistPath;

- (NSMutableDictionary*) getBundlePlist:(NSString*)plistName;
- (NSMutableDictionary*) getDataPlist:(NSString*)plistName;
- (NSMutableDictionary*) getCachePlist:(NSString*)plistName;
- (NSMutableDictionary*) getTempPlist:(NSString*)plistName;

- (NSString*) bundleFilePath:(NSString*)fileName;
- (NSString*) userFilePath:(NSString*)fileName searchPath:(NSUInteger)searchPath;
- (BOOL) savePlistToTemp:(NSDictionary*)plist withName:(NSString*) plistName;
- (BOOL) savePlistToCache:(NSDictionary*)plist withName:(NSString*) plistName;
- (BOOL) deleteFileFromTemp:(NSString*)fileName;

- (BOOL) isNetworkReachable:(NSString*) url;

- (void)loadingStart:(NSDictionary*)options withView:(UIView*)view;
- (void) loadingStop;	

- (UIView*) keyboardSuperview;
- (void) showAlert:(NSString*)message withTitle:(NSString*)title;

- (BOOL) preferencesBoolForKey:(NSString*)key;
- (void) setPreferencesBool:(BOOL)value forKey:(NSString*)key;

+ (Utils*) sharedInstance;

@end
