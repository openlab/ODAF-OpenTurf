//
//  TweetThisViewController.m
//  VanGuide
//
//  Created by shazron on 10-02-22.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "TweetThisViewController.h"
#import "PointDataSummary.h"
#import "RESTWrapper.h"
#import "VanGuideAppDelegate.h"
#import "ODUser.h"
#import "ODHome.h"

@implementation TweetThisViewController

@synthesize tweetTextView, summary, tweetButton, tweetCharCountLabel, currentMode;

- (void)viewDidLoad 
{
    [super viewDidLoad];
	
	self.tweetButton = [[UIBarButtonItem alloc] initWithTitle:NSLocalizedString(kTweetText, kTweetText) 
														style:UIBarButtonItemStyleDone 
													  target:self action:@selector(postTweet)];
	self.tweetButton.enabled = NO;
	self.title = NSLocalizedString(kTweetThisText, kTweetThisText);
	self.navigationItem.rightBarButtonItem = self.tweetButton;
	
	self.tweetCharCountLabel.text = [[NSNumber numberWithInt:kTweetMessageMaxChars] stringValue];
	
	[self.tweetTextView setDefaultCornerRadius];
	[self.tweetTextView becomeFirstResponder];
	
	[self performSelector:@selector(getShortenedLink) withObject:nil afterDelay:0.5];
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
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTAuthenticateFail object:nil];
}

- (void)dealloc 
{
    [super dealloc];
}

- (void) getShortenedLink
{
	ODHome* homeAPI = [ODHome sharedInstance];
	RESTInfo* call = [homeAPI GetLinkForSummary:self.summary.Id];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] 
							createLoadingViewOptionsFullScreen:false 
							withText:NSLocalizedString(kShorteningUrlText, kShorteningUrlText)] 
							withView:[[Utils sharedInstance] keyboardSuperview]];
	
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	
	self.currentMode = GetShortenedLink;
	[homeAPI.webservice sendRequest:call];
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

- (void) summaryCreatedSuccess
{
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTSummaryAddSuccess object:nil];
	
	DebugNSLog(@"Created summary! Adding shortened link...");
	
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
	
	[self getShortenedLink];
}

- (void) summaryCreatedFail
{
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTSummaryAddFail object:nil];
	
	DebugNSLog(@"Failed to create summary!");
	
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
}

- (void) postTweet
{
	ODUser* userAPI = [ODUser sharedInstance];
	RESTInfo* call = [userAPI UpdateTwitterStatus:tweetTextView.text 
										  withLat:self.summary.Latitude 
										  andLong:self.summary.Longitude];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false withText:NSLocalizedString(kTweetingText, kTweetingText)] 
								withView:[[Utils sharedInstance] keyboardSuperview]];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = PostTweet;
	[userAPI.webservice sendRequest:call];
}

- (void) authSuccess:(NSNotification*)notification
{
	// remove notif registration for authenticate
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTAuthenticateSuccess object:nil];
	
	[self postTweet];
}

- (IBAction) close
{
	// remove notif registration for authenticate
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTAuthenticateFail object:nil];

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
	
	// Ignore error data
	if (wrapper.lastStatusCode < 200 || wrapper.lastStatusCode >= 300) {
		return;
	}
	
	switch (self.currentMode) 
	{
		case GetShortenedLink:
		{
			NSDictionary* dict = (NSDictionary*)[wrapper responseAsJson];
			NSString* short_url = [dict objectForKey:@"short_url"];
			NSString* short_url_intro = NSLocalizedString(kCheckOutThisVanGuideLocationText, kCheckOutThisVanGuideLocationText); 
			self.tweetTextView.text = [NSString stringWithFormat:@"%@ %@ ", short_url_intro, short_url];
		}
			break;
		case PostTweet:
			// no need to do anything
			break;
		default:
			break;
	}
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
	switch(statusCode) {
		case 200:
		case 201:
		{
			switch (self.currentMode) 
			{
				case PostTweet:
				{
					NSNotification* notif = [NSNotification notificationWithName:ODRESTTweetPosted object: self];
					[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
					[self close];
				}
					break;
				default:
					break;
			}
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
		case 404: // Summary not yet been created, we need to create it
		{
			[self lazyCreateSummary];
			
			// register for authenticate fail notif
			[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(close) 
														 name:ODRESTAuthenticateFail object:nil];
		}
			
			break;
		default:
			[[Utils sharedInstance] showAlert:[NSString stringWithFormat:@"Code: %d", statusCode] withTitle:@"Error"];
			break;
	}
}

#pragma mark -
#pragma mark UITextViewDelegate

- (void)textViewDidChange:(UITextView *)aTextView 
{
	NSInteger characterCount = (kTweetMessageMaxChars - aTextView.text.length);
	self.tweetButton.enabled = (aTextView.text.length > 0 && characterCount > 0);
	
	if (characterCount > 0) {
		self.tweetCharCountLabel.textColor = [UIColor blackColor];
	} else {
		self.tweetCharCountLabel.textColor = [UIColor redColor];
	}

	self.tweetCharCountLabel.text = [[NSNumber numberWithInt:characterCount] stringValue];
}



@end
