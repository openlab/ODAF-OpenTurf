//
//  Utils.m
//  VanGuide
//
//  Created by shazron on 09-12-03.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import "Utils.h"
#import "Reachability.h"
#import "LoadingView.h"
#import "UIApplication+KeyboardView.h"

static Utils* sharedInstance = nil;

@implementation Utils

@synthesize loadingView;


- (BOOL) preferencesBoolForKey:(NSString*)key
{
	NSUserDefaults *userDefaults = [NSUserDefaults standardUserDefaults];
	return [userDefaults boolForKey:key];
}

- (void) setPreferencesBool:(BOOL)value forKey:(NSString*)key
{
	NSUserDefaults *userDefaults = [NSUserDefaults standardUserDefaults];  
	[userDefaults setBool:value forKey:key]; 
}

- (NSString*) webAppUrl
{
	NSString* url = [[Utils sharedInstance] getConfigSetting:kConfigKeyWebAppUrl];
	
	NSNumber* devMode = [[Utils sharedInstance] getConfigSetting:kConfigKeyDevMode];
	if ([devMode boolValue]) {
		NSDictionary* devSettings = [[Utils sharedInstance] getConfigSetting:kConfigKeyDevSettings];
		url = [devSettings objectForKey:kConfigKeyWebAppUrl];
	}
	
	return url;
}

- (NSDictionary*) createLoadingViewOptionsFullScreen:(BOOL)fullScreen withText:(NSString*)text
{
	NSMutableDictionary* options = [NSMutableDictionary dictionaryWithCapacity:4];
	[options setObject:@"1"	forKey:@"minDuration"];
	[options setObject:@"0.7" forKey:@"backgroundOpacity"];
	[options setObject:(fullScreen ? @"true" : @"false") forKey:@"fullScreen"];
	[options setObject:text forKey:@"labelText"];
	
	return options;
}

- (id) getConfigSetting:(NSString*)key
{
	return [[self getBundlePlist:kPlistConfiguration] objectForKey:key];
}

- (NSString*) generateUUID
{
	CFUUIDRef uuidObj = CFUUIDCreate(nil); //create a new UUID

	//get the string representation of the UUID
	NSString* uuidString = (NSString*)CFUUIDCreateString(nil, uuidObj);
	
	CFRelease(uuidObj);
	
	return [uuidString autorelease];
}

- (NSDate*) dateFromJsDate:(NSString*)jsDate
{
	NSString* prefix = @"/Date(";
	NSString* suffix = @")/";
	
	if ([jsDate hasPrefix:prefix] && [jsDate hasSuffix:suffix]) {
		NSUInteger numLen = [jsDate length] - ([prefix length] + [suffix length]);
		NSString* millisecsFromEpoch = [jsDate substringWithRange:NSMakeRange([prefix length], numLen)];
		NSTimeInterval secsFromEpoch = [millisecsFromEpoch longLongValue] / 1000.0f; 
		
		return [NSDate dateWithTimeIntervalSince1970:secsFromEpoch];
	}
	
	return nil;
}

- (NSString*) secondsToDateString:(NSUInteger)ti withThreshold:(BOOL)withThreshold
{
	NSUInteger diff;
	
	const NSUInteger SEC_PER_MIN = 60;
	const NSUInteger SEC_PER_HR = SEC_PER_MIN * 60;
	const NSUInteger SEC_PER_DAY = SEC_PER_HR * 24;
	const NSUInteger SEC_PER_WEEK = SEC_PER_DAY * 7;
	const NSUInteger SEC_PER_MONTH = SEC_PER_WEEK * 4;
	const NSUInteger SEC_PER_YEAR = SEC_PER_DAY * 365.25f;
	
	if (ti < 1) {
		return @"now";
	}
    else if (ti < SEC_PER_MIN) { // seconds ago
        return [NSString stringWithFormat:@"%ds", ti];
    } else if (ti < SEC_PER_HR) { // minutes ago, less than 1 hr
        diff = round(ti / SEC_PER_MIN);
        return [NSString stringWithFormat:@"%dm", diff];
    } else if (ti < SEC_PER_DAY) { // hours ago, less than 1 day
        diff = round(ti / SEC_PER_HR);
        return[NSString stringWithFormat:@"%dh", diff];
    } else if (ti < SEC_PER_WEEK) { // days ago, less than 1 week
        diff = round(ti / SEC_PER_DAY);
        return [NSString stringWithFormat:@"%dd", diff];
    } else if (ti < SEC_PER_MONTH) { // weeks ago, less than 1 month
        diff = round(ti / SEC_PER_WEEK);
        return [NSString stringWithFormat:@"%dw", diff];
	} else if (ti < SEC_PER_YEAR) { // months ago, less than 1 year
		
		if (withThreshold) {
			return nil;
		} else {
			diff = round(ti / SEC_PER_MONTH);
			return [NSString stringWithFormat:@"%d %@", diff, (diff != 1 ? NSLocalizedString(kMonthsText, kMonthsText) : 
																		  NSLocalizedString(kMonthText, kMonthText))];
		}
	} else { // more than a year ago
		
		if (withThreshold) {
			return nil;
		} else {
			diff = round(ti / SEC_PER_YEAR); // years
			NSUInteger diff2 = ti % SEC_PER_YEAR; // remainder seconds
			return [NSString stringWithFormat:@"%d %@ %@", 
				   diff, 
				   (diff != 1 ? NSLocalizedString(kYearsText, kYearsText) : 
								NSLocalizedString(kYearText, kYearText)),
					[self secondsToDateString:diff2 withThreshold:withThreshold]
				   ];
		}
	}	
}

// Originally from:
// http://stackoverflow.com/questions/902950/iphone-convert-date-string-to-a-relative-time-stamp/932130
//
- (NSString*) dateDiffFromNowToString:(NSDate*)originalDate withThreshold:(BOOL)withThreshold
{
    NSDate* todayDate = [NSDate date];
    double ti = [todayDate timeIntervalSinceDate:originalDate];
	

	NSString* dateString = [self secondsToDateString:ti withThreshold:withThreshold];
	if (dateString == nil) {
		NSDateFormatter *format = [[[NSDateFormatter alloc] init] autorelease];
		[format setDateFormat:@"d/M/y"];
		dateString = [format stringFromDate:originalDate];
	}
	
	return dateString;
}

- (BOOL) locationEquals:(CLLocationCoordinate2D)left withLocation:(CLLocationCoordinate2D)right
{
	BOOL latEq = fequal(left.latitude, right.latitude);
	BOOL lngEq = fequal(left.longitude, right.longitude);
	
	return (latEq && lngEq);
}

- (BOOL) isNetworkReachable:(NSString*) hostName
{
	Reachability* reachability = [Reachability sharedReachability];
	
	[reachability setHostName:hostName];
	
	// workaround for notifier to work: we initialize it once... (synchronous)
	([reachability remoteHostStatus] != NotReachable) &&
		([reachability internetConnectionStatus] != NotReachable); 

	// ... then enable the notifier ...
	[[Reachability sharedReachability] setNetworkStatusNotificationsEnabled:YES];

	// ... then call it async
	return ([reachability remoteHostStatus] != NotReachable) &&
		([reachability internetConnectionStatus] != NotReachable);
}

- (void) showAlert:(NSString*)message withTitle:(NSString*)title
{
	UIAlertView* errorAlert = [[UIAlertView alloc]
							   initWithTitle:NSLocalizedString(title, title)
							   message:NSLocalizedString(message, message) delegate:nil 
							   cancelButtonTitle:NSLocalizedString(kCloseText, kCloseText)
							   otherButtonTitles:nil];
	[errorAlert show];
	[errorAlert release];
}

- (NSMutableDictionary*) getPlist:(NSString *)plistPath
{
    NSString* errorDesc = nil;
    NSPropertyListFormat format;
    NSData* plistXML = [[NSFileManager defaultManager] contentsAtPath:plistPath];
    return (NSMutableDictionary*)[NSPropertyListSerialization
                                          propertyListFromData:plistXML
                                          mutabilityOption:NSPropertyListMutableContainersAndLeaves			  
                                          format:&format errorDescription:&errorDesc];
}

- (NSDictionary*) getBundlePlist:(NSString *)plistName
{
	return [self getPlist:[self bundleFilePath:plistName]];
}

- (NSDictionary*) getDataPlist:(NSString *)plistName
{
	return [self getPlist:[self userFilePath:plistName searchPath:NSDocumentDirectory]];
}

- (NSDictionary*) getTempPlist:(NSString *)plistName
{
	return [self getPlist:[NSTemporaryDirectory() stringByAppendingPathComponent:plistName]];
}

- (NSDictionary*) getCachePlist:(NSString *)plistName
{
	return [self getPlist:[self userFilePath:plistName searchPath:NSCachesDirectory]];
}

- (BOOL) deleteFileFromTemp:(NSString*)fileName
{
	NSError* error;
	NSString* path = [NSTemporaryDirectory() stringByAppendingPathComponent:fileName];
	[[NSFileManager defaultManager] removeItemAtPath:path error:&error];
	
	return (error == nil);
}

- (BOOL) savePlistToTemp:(NSDictionary*)plist withName:(NSString*) plistName
{
	NSString* path = [NSTemporaryDirectory() stringByAppendingPathComponent:plistName];
	return [plist writeToFile:path atomically:YES];
}

- (BOOL) savePlistToCache:(NSDictionary*)plist withName:(NSString*) plistName
{
    NSArray* paths = NSSearchPathForDirectoriesInDomains(NSCachesDirectory, NSUserDomainMask, YES);
    NSString* path = [paths objectAtIndex:0];
	return [plist writeToFile:[path stringByAppendingPathComponent:plistName] atomically:YES];
}

- (NSString*) userFilePath:(NSString*)fileName searchPath:(NSUInteger)searchPath
{
    NSArray *paths = NSSearchPathForDirectoriesInDomains(searchPath, NSUserDomainMask, YES);
    NSString *dir = [paths objectAtIndex:0];
    return [dir stringByAppendingPathComponent:fileName];
}

- (NSString*) bundleFilePath:(NSString*)fileName 
{
    return [[NSBundle mainBundle] pathForResource:fileName ofType:@"plist"];
}

#pragma mark -
#pragma mark Loading View

- (void)loadingStart:(NSDictionary*)loadOptions withView:(UIView*)view
{
	if (self.loadingView != nil) {
		return;
	}
	
	NSMutableDictionary* options = [[loadOptions mutableCopy] autorelease];
	
	DebugNSLog(@"Loading start");
	
	CGFloat strokeOpacity, backgroundOpacity;
	BOOL fullScreen = NO;
	NSString* colorCSSString;
	NSString* labelText;
	
	strokeOpacity = [[options objectForKey:@"strokeOpacity"] floatValue];
	backgroundOpacity = [[options objectForKey:@"backgroundOpacity"] floatValue];
	fullScreen = [[options objectForKey:@"fullScreen"] boolValue];
	
	colorCSSString = [options objectForKey:@"strokeColor"];
	labelText = [options objectForKey:@"labelText"];
	
	if (!labelText) {
		labelText = [LoadingView defaultLabelText];
	}
	
	UIColor* strokeColor = [LoadingView defaultStrokeColor];
	
	if (strokeOpacity <= 0) {
		strokeOpacity = [LoadingView defaultStrokeOpacity];
	} 
	if (colorCSSString) {
		UIColor* tmp = [UIColor colorWithName:colorCSSString];
		if (tmp) {
			strokeColor = tmp;
		}
	} 
	
	self.loadingView = [LoadingView loadingViewInView:view strokeOpacity:strokeOpacity backgroundOpacity:backgroundOpacity 
										  strokeColor:strokeColor fullScreen:fullScreen labelText:labelText];
	
	NSRange minMaxDuration = NSMakeRange(2, 3600);// 1 hour max? :)
	NSString* durationKey = @"duration";
	// the view will be shown for a minimum of this value if durationKey is not set
	self.loadingView.minDuration = [options integerValueForKey:@"minDuration" defaultValue:minMaxDuration.location withRange:minMaxDuration];
	
	// if there's a duration set, we set a timer to close the view
	if ([options valueForKey:durationKey]) {
		NSTimeInterval duration = [options integerValueForKey:durationKey defaultValue:minMaxDuration.location withRange:minMaxDuration];
		[self performSelector:@selector(loadingStop) withObject:nil afterDelay:duration];
	}
}

- (void) loadingStop
{
	if (self.loadingView != nil) 
	{
		DebugNSLog(@"Loading stop");
		
		NSTimeInterval diff = 0;
		//if (self.loadingView.timestamp != nil) {
		diff = [[NSDate date] timeIntervalSinceDate:self.loadingView.timestamp] - self.loadingView.minDuration;
		//}
		
		if (diff >= 0) {
			[self.loadingView removeView]; // the superview will release (see removeView doc), so no worries for below
			self.loadingView = nil;
		} else {
			[self performSelector:@selector(loadingStop) withObject:nil afterDelay:-1*diff];
		}
	}
}

- (UIView*) keyboardSuperview
{
	return [[[UIApplication sharedApplication] keyboardView] superview];
}

#pragma mark -
#pragma mark Singleton methods

+ (Utils*) sharedInstance
{
    @synchronized(self)
    {
        if (sharedInstance == nil)
			sharedInstance = [[Utils alloc] init];
    }
    return sharedInstance;
}

+ (id) allocWithZone:(NSZone *)zone {
    @synchronized(self) {
        if (sharedInstance == nil) {
            sharedInstance = [super allocWithZone:zone];
            return sharedInstance;  // assignment and return on first allocation
        }
    }
    return nil; // on subsequent allocation attempts return nil
}

- (id) copyWithZone:(NSZone *)zone
{
    return self;
}

- (id) retain {
    return self;
}

- (unsigned) retainCount {
    return UINT_MAX;  // denotes an object that cannot be released
}

- (void) release {
    //do nothing
}

- (id) autorelease {
    return self;
}


@end
