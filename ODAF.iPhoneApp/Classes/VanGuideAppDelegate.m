//
//  MSOpenDataAppDelegate.m
//  VanGuide
//
//  Created by shazron on 09-11-27.
//  Copyright Nitobi Software Inc 2009. All rights reserved.
//

#import "VanGuideAppDelegate.h"
#import "MapViewController.h"
#import "GoogleMapViewController.h"
#import "RouteMeViewController.h"
#import "MapDataSource.h"
#import "KmlPlacemark.h"
#import <QuartzCore/QuartzCore.h>

@implementation VanGuideAppDelegate

@synthesize window, splashScreen;
@synthesize viewController, navigationController;

+ (VanGuideAppDelegate*) sharedApplicationDelegate
{
	return (VanGuideAppDelegate*)[UIApplication sharedApplication].delegate;
}

- (void) pushViewControllerToStack:(UIViewController*)aViewController
{
	[[VanGuideAppDelegate sharedApplicationDelegate].navigationController pushViewController:aViewController animated:YES];
}

- (UIViewController*) topViewControllerOnStack
{
	return [[VanGuideAppDelegate sharedApplicationDelegate].navigationController topViewController];
}

- (void) applicationDidFinishLaunching:(UIApplication*)application 
{
	application.applicationSupportsShakeToEdit = YES;
	
	[[UIApplication sharedApplication] setStatusBarStyle:UIStatusBarStyleBlackOpaque animated:NO];	
	
	NSNumber* useMapKit = [[Utils sharedInstance] getConfigSetting:kConfigKeyUseMapKit];
	
	MapViewController* aController = nil;
	if ([useMapKit boolValue]) {
		aController = [[GoogleMapViewController alloc] initWithNibName:@"GoogleMapViewController" bundle:nil];
	} else {
		aController = [[RouteMeViewController alloc] initWithNibName:@"RouteMeViewController" bundle:nil];
	}
	
	self.viewController = aController;
	[aController release];
	
	NSString* splashImageName = @"Default";
	CGFloat splashDelay = 1.0;
	if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
	{
		splashImageName = @"Default-Portrait";
	} 

	UIImage* image = [[UIImage alloc] initWithContentsOfFile:[[NSBundle mainBundle] pathForResource:splashImageName ofType:@"png"]];
	self.splashScreen = [[UIImageView alloc] initWithImage:image];
	[image release];
	[window addSubview:splashScreen];
	
	[self performSelector:@selector(prereqCheck) withObject:nil afterDelay:0.1];
	[self performSelector:@selector(tempCleanup) withObject:nil afterDelay:0.3];
	[self performSelector:@selector(afterSplash) withObject:nil afterDelay:splashDelay];
}

- (void) prereqCheck
{
	if ([kTwitterAppConsumerKey length] == 0) {
		NSLog(@"WARNING: Twitter app consumer key is missing. Add in Constants.h");
	}

	if ([kTwitterAppConsumerSecret length] == 0) {
		NSLog(@"WARNING: Twitter app consumer secret is missing. Add in Constants.h");
	}
}

- (void) tempCleanup
{
	// Clear User Object file (if pref not set)
	if (![[Utils sharedInstance] preferencesBoolForKey:kPreferencesKeySaveTwitterLogin]) {
		[[Utils sharedInstance] deleteFileFromTemp:kPlistUserObjectTemp];
	}
	
	// Clear "Your Landmarks" checkmark
	NSMutableDictionary* filters = [[Utils sharedInstance] getCachePlist:kPlistFilter];
	[filters setObject:[NSNumber numberWithInt:NO] forKey:kYourLandmarksFilter];
	[[Utils sharedInstance] savePlistToCache:filters withName:kPlistFilter];
	
	// Clear Ratings cache file
	[[Utils sharedInstance] deleteFileFromTemp:kPlistRatingsCache];
}

- (void) afterSplash
{
	CATransition* animation = nil;
	
	if (UI_USER_INTERFACE_IDIOM() != UIUserInterfaceIdiomPad)
	{
		[self.splashScreen removeFromSuperview];
		
		[CATransaction begin];
		animation = [CATransition animation];
		animation.type = kCATransitionFade;
		animation.duration = 1.0;
	}
	
	self.navigationController = [[UINavigationController alloc] initWithRootViewController:self.viewController];	
	self.navigationController.navigationBar.tintColor = [UIColor colorWithHexString:[[Utils sharedInstance] getConfigSetting:kConfigKeyTintColourRGB]];
	self.navigationController.delegate  = (MapViewController*)self.viewController;	
	
	[window addSubview:[self.navigationController view]];
	[window bringSubviewToFront:self.navigationController.view];
	
	if (UI_USER_INTERFACE_IDIOM() != UIUserInterfaceIdiomPad)
	{
		// Commit the animation
		[[window layer] addAnimation:animation forKey:@"frame.size"];
		[CATransaction commit];
	}
	
    [window makeKeyAndVisible];
}

- (void) dealloc 
{
	self.viewController = nil;
	self.navigationController = nil;
	self.splashScreen = nil;
	
    [window release];
    [super dealloc];
}

- (NSString*) appURLScheme
{
	NSString* URLScheme = nil;
	
	NSArray* URLTypes = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleURLTypes"];
	if (URLTypes != nil) {
		NSDictionary* dict = [URLTypes objectAtIndex:0];
		if (dict != nil) 
		{
			NSArray* URLSchemes = [dict objectForKey:@"CFBundleURLSchemes"];
			if (URLSchemes != nil ) {    
				URLScheme = [URLSchemes objectAtIndex:0];
			}
		}
	}
	
	return URLScheme;
}	

- (BOOL) application:(UIApplication*)application handleOpenURL:(NSURL*)url
{
	NSString* command = [url host];

	if ([command isEqualToString:@"ViewComment"]) 
	{
		NSMutableDictionary* options = [NSMutableDictionary dictionaryWithCapacity:2];
		NSArray* options_parts = [NSArray arrayWithArray:[[url query] componentsSeparatedByString:@"&"]];
		int options_count = [options_parts count];
		
		for (int i = 0; i < options_count; i++) {
			NSArray* option_part = [[options_parts objectAtIndex:i] componentsSeparatedByString:@"="];
			NSString* name = [(NSString *)[option_part objectAtIndex:0] stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
			NSString* value = [(NSString *)[option_part objectAtIndex:1] stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
			[options setObject:value forKey:name];
		}
		
		self.viewController.pendingNotification = [NSNotification notificationWithName:ODViewComment object:options];
	}
	
	return ([[url scheme] isEqualToString:[self appURLScheme]]);
}

@end

