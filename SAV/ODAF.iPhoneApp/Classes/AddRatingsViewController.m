//
//  AddRatingsViewController.m
//  VanGuide
//
//  Created by shazron on 10-02-01.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "AddRatingsViewController.h"
#import "S7RatingView.h"
#import "PointDataSummary.h"
#import "ODSummaries.h"
#import "RESTWrapper.h"
#import "VanGuideAppDelegate.h"

@implementation AddRatingsViewController


@synthesize ratingView, summary, ratingCaption, rateButton;

- (void) saveRatingToCache
{
	NSMutableDictionary* ratedItems = [[Utils sharedInstance] getTempPlist:kPlistRatingsCache];
	if (ratedItems == nil) {
		ratedItems = [NSMutableDictionary dictionaryWithCapacity:1];
	}

	[ratedItems setValue:@"" forKey:[[NSNumber numberWithUnsignedInt:self.summary.Id] stringValue]];
	[[Utils sharedInstance] savePlistToTemp:ratedItems withName:kPlistRatingsCache];
}

- (BOOL) isAlreadyRated
{
	NSMutableDictionary* ratedItems = [[Utils sharedInstance] getTempPlist:kPlistRatingsCache];
	id object = [ratedItems objectForKey:[[NSNumber numberWithUnsignedInt:self.summary.Id] stringValue]];
	
	return (object != nil);
}

- (void)viewDidLoad 
{
    [super viewDidLoad];
	
	self.ratingView.delegate = self;

	self.rateButton = [[UIBarButtonItem alloc] initWithTitle:NSLocalizedString(kRateText, kRateText) style:UIBarButtonItemStyleDone 
													 target:self action:@selector(addRating)];
	self.rateButton.enabled = NO;
	self.title = NSLocalizedString(kAddRatingText, kAddRatingText);
	
	if (![self isAlreadyRated]) {
		self.navigationItem.rightBarButtonItem = self.rateButton;
	}
	
	[self.ratingView setStarImage:[UIImage imageNamed:@"star-empty-lg.png"]   forState:STATE_NONSELECTED];
	[self.ratingView setStarImage:[UIImage imageNamed:@"star-full-lg.png"]      forState:STATE_SELECTED];
	[self.ratingView setStarImage:[UIImage imageNamed:@"star-half-lg.png"]  forState:STATE_HALFSELECTED];
	[self.ratingView setStarImage:[UIImage imageNamed:@"star-full-lg.png"]           forState:STATE_HOT];
	[self.ratingView setStarImage:[UIImage imageNamed:@"star-full-lg.png"]   forState:STATE_HIGHLIGHTED];
	
	[self.ratingView setDefaultCornerRadius];
	self.ratingView.rating = [self.summary calculatedRatingFromMin:1 andMax:5];

	self.ratingView.margin = CGPointMake(8, 12);
	self.ratingView.starSpacing = 10.0f;
	[self.ratingView setBackgroundColor:[UIColor whiteColor]];
	
	if ([self isAlreadyRated]) {
		self.ratingView.userInteractionEnabled = NO;
		self.ratingCaption.text = NSLocalizedString(kYouveAlreadyRatedThisLandmarkText, kYouveAlreadyReatedThisLandmarkText);
	} else {
		self.ratingView.userInteractionEnabled = YES;
		self.ratingCaption.text = [NSString stringWithFormat:self.ratingCaption.text, self.summary.Name];
		[self.ratingView performSelector:@selector(initializeComponent) withObject:nil];
	}
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
	// Release any retained subviews of the main view.
	// e.g. self.myOutlet = nil;
	
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTAuthenticateSuccess object: nil];
}

- (void)dealloc 
{
    [super dealloc];
}

- (void) lazyCreateSummary
{
	DebugNSLog(@"No summary. posting notif to create...");
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false 
																						   withText:NSLocalizedString(kInitializingText, kInitializingText)] 
								withView:[[Utils sharedInstance] keyboardSuperview]];
	
	// register for summary created notifs
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(summaryCreatedSuccess)  name:ODRESTSummaryAddSuccess object:nil];		
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(summaryCreatedFail)  name:ODRESTSummaryAddFail object:nil];		
	
	NSNotification* notif = [NSNotification notificationWithName:ODRESTSummaryRequired object: self];
	[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
}

- (IBAction) addRating
{
	ODSummaries* summariesAPI = [ODSummaries sharedInstance];
	RESTInfo* call = [summariesAPI 
					   AddRating:[PointDataSummary calculateRatingFromMin:1 andMax:5 withValue:self.ratingView.userRating] 
					   forSummary:self.summary.Id];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false 
																						   withText:NSLocalizedString(kRatingText, kRatingText)] 
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
		[self addRating];
	}
}

- (void) summaryCreatedSuccess
{
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTSummaryAddSuccess object:nil];
	
	DebugNSLog(@"Created summary! Adding comment...");
	
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
	
	[self addRating];
}

- (void) summaryCreatedFail
{
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTSummaryAddFail object:nil];
	
	DebugNSLog(@"Failed to create summary!");
	
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
}

- (IBAction) close
{
	VanGuideAppDelegate* delg = (VanGuideAppDelegate*)[UIApplication sharedApplication].delegate;
	[delg.navigationController popViewControllerAnimated:YES];
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
			[self saveRatingToCache];
			NSNotification* notif = [NSNotification notificationWithName:ODRESTRatingAdded object: self];
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

#pragma mark -
#pragma mark S7RatingDelegate

- (void)ratingView:(S7RatingView *)ratingView didChangeRatingFrom:(float)previousRating to:(float)rating
{
}

- (void)ratingView:(S7RatingView *)ratingView didChangeUserRatingFrom:(int)previousUserRating to:(int)userRating
{
	DebugNSLog(@"Previous - Current Rating : %f - %f", previousUserRating, userRating);
	self.rateButton.enabled = (userRating > 0);
}

@end
