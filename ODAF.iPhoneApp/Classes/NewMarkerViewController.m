//
//  NewMarkerViewController.m
//  VanGuide
//
//  Created by shazron on 10-02-16.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "NewMarkerViewController.h"
#import "RESTWrapper.h"
#import "ODSummaries.h"
#import "PointDataSummary.h"
#import "KmlPlacemark.h"
#import "VanGuideAppDelegate.h"
#import "MapAnnotation.h"
#import "MapDataSource.h"

@implementation NewMarkerViewController

@synthesize placeName, placeDescription, addButton, summary;

- (CGSize) contentSizeForViewInPopoverView {
	return CGSizeMake(kDefaultPopoverWidth, kDefaultPopoverHeight);
}

- (void)viewDidLoad 
{
    [super viewDidLoad];
	
	[self.placeDescription setDefaultCornerRadius];
	
	self.addButton = [[UIBarButtonItem alloc] initWithTitle:NSLocalizedString(kAddText, kAddText) style:UIBarButtonItemStyleDone 
													 target:self action:@selector(addLandmarkButtonTouched)];
	self.addButton.enabled = NO;
	self.title = NSLocalizedString(kAddLandmarkText, kAddLandmarkText);
	self.navigationItem.rightBarButtonItem = self.addButton;
	
	[self.placeName becomeFirstResponder];
}

- (void)viewDidDisappear:(BOOL)animated
{
	[[Utils sharedInstance] loadingStop];
	[ODSummaries sharedInstance].webservice.delegate = nil;
	[super viewDidDisappear:animated];
}

- (void)didReceiveMemoryWarning 
{
	// Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
	
	// Release any cached data, images, etc that aren't in use.
}

- (void)viewDidUnload 
{
	// Release any retained subviews of the main view.
	// e.g. self.myOutlet = nil;

	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTAuthenticateSuccess object: nil];
}

- (void)dealloc 
{
    [super dealloc];
}

// TODO: factor into Utils
- (NSString*) getLayerId
{
	// try to get the current user object in tmp (if authenticated before)
	NSDictionary* userDict = [[Utils sharedInstance] getTempPlist:kPlistUserObjectTemp];
	NSString* layerId = @"";
	
	if (userDict != nil) {
		NSNumber* num = [userDict objectForKey:kServiceUserId];
		if (num != nil) {
			layerId = [num stringValue];
		}
	}
	
	return layerId;
}

- (void) addLandmark
{
	DebugNSLog(@"Creating summary...");
	
	ODSummaries* summariesAPI = [ODSummaries sharedInstance];
	
	self.summary.LayerId = [self getLayerId];
	self.summary.Name = self.placeName.text;
	self.summary.Description = self.placeDescription.text;
	self.summary.Tag = @"";
	
	RESTInfo* call = [summariesAPI Add:self.summary];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false 
																						   withText:NSLocalizedString(kAddingText, kAddingText)] 
								withView:[[Utils sharedInstance] keyboardSuperview]];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	[summariesAPI.webservice sendRequest:call];
}

- (void) addLandmarkButtonTouched
{
	[self addLandmark];
}
	 
- (void) close
{
	if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
	{
		NSNotification* notif = [NSNotification notificationWithName:ODClosePopover object:nil];
		[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
	}
	else
	{
		VanGuideAppDelegate* delg = (VanGuideAppDelegate*)[UIApplication sharedApplication].delegate;
		[delg.navigationController popViewControllerAnimated:YES];
	}		
}

- (IBAction) textFieldDidUpdate:(id) sender
{
	UITextField* textField = (UITextField*)sender;
	self.addButton.enabled = textField.text.length >= kSummaryNameMinChars;
}

#pragma mark -
#pragma mark RESTWrapperDelegate

- (void) wrapper:(RESTWrapper*)wrapper didRetrieveData:(NSData *)data
{
	DebugNSLog(@"Retrieved data: %@", [wrapper responseAsText]);
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
}

- (void) wrapper:(RESTWrapper*)wrapper didFailWithError:(NSError *)error
{
	NSLog(@"REST call error: %@", [error description]);
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
}

- (void)wrapper:(RESTWrapper*)wrapper didReceiveStatusCode:(int)statusCode
{
	DebugNSLog(@"Status code: %d", statusCode);
	switch(statusCode) 
	{
		case 200:
		case 201:
		{
			MapAnnotation* annotation = [[[MapAnnotation alloc] init] autorelease];
			annotation.dataSource = [MapDataSource yourLandmarksDataSource];
			annotation.placemark = [self.summary convertToPlacemark];
			
			NSNotification* notif = [NSNotification notificationWithName:ODRESTUserSummaryAddSuccess object:annotation];
			[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
			[self close];
		}
			break;
		case 401:
		{
			// register for authenticate notif
			[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(addLandmark) 
														 name:ODRESTAuthenticateSuccess object:nil];
			
			// show authentication dialog
			[[NSNotificationCenter defaultCenter] postNotificationName:ODRESTAuthorizationRequired object:self];
		}
			break;
		default:
			[[Utils sharedInstance] showAlert:[NSString stringWithFormat:@"Code: %d", statusCode] 
									withTitle:NSLocalizedString(kErrorText, kErrorText)];
			break;
	}
}

@end
