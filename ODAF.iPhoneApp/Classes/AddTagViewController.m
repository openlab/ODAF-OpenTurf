//
//  AddTagViewController.m
//  VanGuide
//
//  Created by shazron on 10-02-01.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "AddTagViewController.h"
#import <QuartzCore/QuartzCore.h>
#import "PointDataSummary.h"
#import "ODSummaries.h"
#import "RESTWrapper.h"
#import "VanGuideAppDelegate.h"

@implementation AddTagViewController

@synthesize summary, tagToAdd, currentTags, tagButton;

- (void)viewDidLoad 
{
    [super viewDidLoad];
	
	self.tagButton = [[UIBarButtonItem alloc] initWithTitle:NSLocalizedString(kTagText, kTagText) style:UIBarButtonItemStyleDone 
													 target:self action:@selector(addTag)];
	self.tagButton.enabled = NO;
	self.title = NSLocalizedString(kAddTagsText, kAddTagsText);
	self.navigationItem.rightBarButtonItem = self.tagButton;

	[self.currentTags setDefaultCornerRadius];
	
	// show current tags
	if ([self.summary.Tag length] > 0) {
		self.currentTags.text = [self.summary.Tag stringByReplacingOccurrencesOfString:@"," withString:@"  "];
	}

	[self.tagToAdd becomeFirstResponder];
}

- (void)viewDidDisappear:(BOOL)animated
{
	[[Utils sharedInstance] loadingStop];
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
	self.summary = nil;
	self.currentTags = nil;
	self.tagToAdd = nil;
	
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTAuthenticateSuccess object: nil];
}

- (void)dealloc 
{
    [super dealloc];
}

- (IBAction) close
{
	VanGuideAppDelegate* delg = (VanGuideAppDelegate*)[UIApplication sharedApplication].delegate;
	[delg.navigationController popViewControllerAnimated:YES];
}

- (void) lazyCreateSummary
{
	DebugNSLog(@"No summary. posting notif to create...");
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false withText:NSLocalizedString(kInitializingText, kInitializingText)] 
								withView:[[Utils sharedInstance] keyboardSuperview]];
	
	// register for summary created notifs
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(summaryCreatedSuccess)  name:ODRESTSummaryAddSuccess object:nil];		
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(summaryCreatedFail)  name:ODRESTSummaryAddFail object:nil];		
	
	NSNotification* notif = [NSNotification notificationWithName:ODRESTSummaryRequired object: self];
	[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
}

- (IBAction) addTag
{
	ODSummaries* summariesAPI = [ODSummaries sharedInstance];
	RESTInfo* call = [summariesAPI AddTag:tagToAdd.text forSummary:self.summary.Id];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false withText:NSLocalizedString(kTaggingText, kTaggingText)] 
								withView:[[Utils sharedInstance] keyboardSuperview]];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	[summariesAPI.webservice sendRequest:call];
}

- (void) authSuccess:(NSNotification*)notification
{
	// remove notif registration for authenticate
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTAuthenticateSuccess object:nil];
	
	//NSDictionary* userDict = (NSDictionary*)notification.object;

	// now since we are authenticated, we check whether the summary has been created
	// if not, we send out the notif to create it, then wait for another notif
	if (!self.summary.isCreated) 
	{
		[self lazyCreateSummary];
	} 
	else 
	{
		[self addTag];
	}
}

- (void) summaryCreatedSuccess
{
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTSummaryAddSuccess object:nil];
	
	DebugNSLog(@"Created summary! Adding comment...");
	
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
	
	[self addTag];
}

- (void) summaryCreatedFail
{
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTSummaryAddFail object:nil];
	
	DebugNSLog(@"Failed to create summary!");
	
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
}

- (IBAction) textFieldDidUpdate:(id) sender
{
	UITextField* textField = (UITextField*)sender;
	self.tagButton.enabled = textField.text.length > 0;
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
			NSNotification* notif = [NSNotification notificationWithName:ODRESTTagAdded object:nil];
			[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
			[self close];
		}
			break;
		case 401:
		{
			// register for authenticate notif
			[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(authSuccess:) 
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
