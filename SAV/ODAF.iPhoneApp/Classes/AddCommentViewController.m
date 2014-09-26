//
//  AddCommentViewController.m
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import "AddCommentViewController.h"
#import "S7RatingView.h"
#import <QuartzCore/QuartzCore.h>
#import "ODComments.h"
#import "RESTWrapper.h"
#import "UIApplication+KeyboardView.h"
#import "PointDataSummary.h"
#import "VanGuideAppDelegate.h"
#import "PointDataComment.h"
#import "ODHome.h"
#import "ODUser.h"

@implementation AddCommentViewController

@synthesize textView, summary, aggregateComment, currentMode, postButton, tweetThisSwitch;

- (void)viewDidLoad 
{
    [super viewDidLoad];
	
	self.currentMode = AddCommentModeNone;
	
	self.postButton = [[UIBarButtonItem alloc] initWithTitle:NSLocalizedString(kPostText, kPostText) style:UIBarButtonItemStyleDone 
													  target:self action:@selector(addComment)];
	self.postButton.enabled = NO;
	self.title = NSLocalizedString(kPostCommentText, kPostCommentText);
	self.navigationItem.rightBarButtonItem = self.postButton;
	
	self.tweetThisSwitch.on = [[Utils sharedInstance] preferencesBoolForKey:kPreferencesKeyTweetThis];
	
	[self.textView setDefaultCornerRadius];
	[self.textView becomeFirstResponder];
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

- (IBAction) close
{
	VanGuideAppDelegate* delg = (VanGuideAppDelegate*)[UIApplication sharedApplication].delegate;
	[delg.navigationController popViewControllerAnimated:YES];
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

- (void) addComment
{
	ODComments* commentsAPI = [ODComments sharedInstance];
	RESTInfo* call = [commentsAPI Add:self.summary.Id text:self.textView.text];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false 
																						   withText:NSLocalizedString(kPostingText, kPostingText)] 
								withView:[[Utils sharedInstance] keyboardSuperview]];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = AddComment;
	[commentsAPI.webservice sendRequest:call];
}

- (void) getCommentLink
{
	ODHome* homeAPI = [ODHome sharedInstance];
	RESTInfo* call = [homeAPI GetLinkForComment:self.aggregateComment.Comment.Id];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false 
																						   withText:NSLocalizedString(kShorteningUrlText, kShorteningUrlText)] 
								withView:[[Utils sharedInstance] keyboardSuperview]];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = GetLinkForComment;
	[homeAPI.webservice sendRequest:call];
}

- (void) postTweet:(NSString*)tweetText
{
	ODUser* userAPI = [ODUser sharedInstance];
	RESTInfo* call = [userAPI UpdateTwitterStatus:tweetText 
										  withLat:self.summary.Latitude 
										  andLong:self.summary.Longitude];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false 
																						   withText:NSLocalizedString(kTweetingText, kTweetingText)] 
								withView:[[Utils sharedInstance] keyboardSuperview]];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = TweetThis;
	[userAPI.webservice sendRequest:call];
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
		[self addComment];
	}
}

- (void) summaryCreatedSuccess
{
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTSummaryAddSuccess object:nil];
	
	DebugNSLog(@"Created summary! Adding comment...");
	
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
	
	[self addComment];
}

- (void) summaryCreatedFail
{
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTSummaryAddFail object:nil];
	
	DebugNSLog(@"Failed to create summary!");
	
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
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
		case AddComment:
		{
			self.aggregateComment = [[AggregateComment alloc] init];
			NSDictionary* dict = (NSDictionary*)[wrapper responseAsJson];
			[self.aggregateComment setValuesForKeysWithDictionary:dict];

			if (self.tweetThisSwitch.on) {
				[self getCommentLink];
			}
		}
			break;
		case GetLinkForComment:
		{
			NSDictionary* dict = (NSDictionary*)[wrapper responseAsJson];
			NSString* short_url = [dict objectForKey:@"short_url"];
			
			// truncate comment, add short url at the end, then tweet
			NSString* ellipsis = @"…";
			NSUInteger text_to_add_len = 1 + [short_url length]; // 1 is for the extra space between text and the url
			BOOL need_to_trim = ([self.textView.text length] + text_to_add_len) > kTweetMessageMaxChars;
			
			if (need_to_trim) 
			{
				text_to_add_len += [ellipsis length]; // add ellipsis
				NSUInteger truncated_len = kTweetMessageMaxChars - text_to_add_len;
				NSString* truncated_text = [self.textView.text substringToIndex:truncated_len];
				
				[self postTweet:[NSString stringWithFormat:@"%@%@ %@", truncated_text, ellipsis, short_url]];
			} 
			else 
			{
				[self postTweet:[NSString stringWithFormat:@"%@ %@", self.textView.text, short_url]];
			}
		}
			break;
		case TweetThis:
			// do nothing, already handled
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
				case AddComment:
				{
					NSNotification* notif = [NSNotification notificationWithName:ODRESTCommentAdded object:self];
					[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
					if (!self.tweetThisSwitch.on) {
						[self close];
					} 
				}
					break;
				case GetLinkForComment:
					break;
				case TweetThis:
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
			switch (self.currentMode) 
			{
				case TweetThis:
				{
					// problem with updating twitter.
					[[Utils sharedInstance] showAlert:[NSString stringWithFormat:@"Status code: %d", statusCode] 
											withTitle:NSLocalizedString(kTwitterErrorText, kTwitterErrorText)];
					[self close];
				}
					break;
				default:
				{
					// register for authenticate notif
					[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(authSuccess:) 
																 name:ODRESTAuthenticateSuccess object:nil];
					
					// show authentication dialog
					[[NSNotificationCenter defaultCenter] postNotificationName:ODRESTAuthorizationRequired object:self];
				}
					break;
			}
		}
			break;
		default:
			[[Utils sharedInstance] showAlert:[NSString stringWithFormat:@"Code: %d", statusCode] 
									withTitle:NSLocalizedString(kErrorText, kErrorText)];
			break;
	}
}

#pragma mark -
#pragma mark UITextViewDelegate

- (void)textViewDidChange:(UITextView *)aTextView 
{
	self.postButton.enabled = (aTextView.text.length > 0);
}

@end
