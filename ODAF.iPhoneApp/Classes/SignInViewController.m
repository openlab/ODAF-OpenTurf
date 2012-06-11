//
//  SignInViewController.m
//  VanGuide
//
//  Created by shazron on 10-03-05.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import "SignInViewController.h"
#import "ColorfulButton.h"
#import "xAuth.h"
#import "ODUser.h"
#import "OADataFetcher.h"
#import "RESTWrapper.h"
#import "VanGuideAppDelegate.h"
#import "OAuthAccount.h"


@interface SignInViewController(hidden)

- (void) authenticateWithKey:(NSString*)key andSecret:(NSString*)secret;

@end

@implementation SignInViewController

@synthesize username, password, signInButton, twitterAuth, toolbar, oauthAccount, spinner, noReauth;

- (void)viewDidLoad 
{
    [super viewDidLoad];
	
	[self.spinner stopAnimating];
	
	UIColor* gradientHigh = [UIColor colorWithHexString:[[Utils sharedInstance] getConfigSetting:kConfigKeyButtonGradientHighRGB]];
	UIColor* gradientLow  = [UIColor colorWithHexString:[[Utils sharedInstance] getConfigSetting:kConfigKeyButtonGradientLowRGB]];
	
	[self.signInButton setHighColor:gradientHigh];
	[self.signInButton setLowColor:gradientLow];
	[self.signInButton setTitle:kSignInText forState:UIControlStateNormal];

	self.toolbar.tintColor = [UIColor colorWithHexString:[[Utils sharedInstance] getConfigSetting:kConfigKeyTintColourRGB]];
	
	self.twitterAuth = [[xAuth alloc] init];
	
	NSDictionary* userDict = [[Utils sharedInstance] getTempPlist:kPlistUserObjectTemp];
	if (userDict != nil) {
		self.oauthAccount = [[[OAuthAccount alloc] init] autorelease];
		[self.oauthAccount setValuesForKeysWithDictionary:userDict];
	}
	
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(xAuthSuccess:) name:ODRESTxAuthSuccess object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(xAuthFail:)	 name:ODRESTxAuthFail object:nil];
	
	[self.username becomeFirstResponder];
	
	// pre-existing credentials
	if (self.oauthAccount != nil) 
	{
		// ask for permission to use existing credentials
		NSString* aTitle = kSavedTwitterLoginText;
		NSString* aMessage = [NSString stringWithFormat:kUseSavedCredentialsText, self.oauthAccount.screen_name];
		
		UIAlertView* confirm = [[UIAlertView alloc]
								   initWithTitle:NSLocalizedString(aTitle, aTitle)
								   message:NSLocalizedString(aMessage, aMessage) delegate:self 
								   cancelButtonTitle:NSLocalizedString(kNoText, kNoText)
								   otherButtonTitles:NSLocalizedString(kYesText, kYesText), nil];

		confirm.tag = kActionSheetUseSavedCredentials;
		[confirm show];
		[confirm release];
	}
}

- (void)didReceiveMemoryWarning 
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

- (void)viewDidUnload 
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (void)dealloc 
{
    [super dealloc];
}

- (IBAction) signIn
{
	self.signInButton.titleLabel.alpha = 0.0;
	self.spinner.hidden = NO;
	[self.spinner startAnimating];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;

	[self.twitterAuth xAuthAccessTokenForUsername:self.username.text andPassword:self.password.text];
}

- (IBAction) close
{
	[[super parentViewController] dismissModalViewControllerAnimated:YES];
}

- (void) authenticateWithKey:(NSString*)key andSecret:(NSString*)secret
{
	ODUser* userAPI = [ODUser sharedInstance];
	RESTInfo* call = [userAPI Authenticate:key 
						  oauthTokenSecret:secret];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:true 
																						   withText:NSLocalizedString(kAuthenticatingText, kAuthenticatingText)] 
								withView:[[Utils sharedInstance] keyboardSuperview]];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	[userAPI.webservice sendRequest:call];
}

- (void) xAuthSuccess:(NSNotification*)notification
{
	self.signInButton.titleLabel.alpha = 1.0;
	[self.spinner stopAnimating];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
	
	OAToken* token = (OAToken*)notification.object;
	[self authenticateWithKey:token.key andSecret:token.secret];
}	

- (void) xAuthFail:(NSNotification*)notification
{
#ifdef _DEBUG	
	NSError* error = (NSError*)notification.object;
	DebugNSLog(@"Could not login using Twitter xAuth: %@", [error localizedDescription]);
#endif

	self.signInButton.titleLabel.alpha = 1.0;
	[self.spinner stopAnimating];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
	
	[[Utils sharedInstance] showAlert:NSLocalizedString(kCouldntSignYouInText, kCouldntSignYouInText) 
							withTitle:NSLocalizedString(kTwitterText, kTwitterText)];
}

#pragma mark -
#pragma mark RESTWrapperDelegate

- (void) wrapper:(RESTWrapper*)wrapper didRetrieveData:(NSData*)data
{
	DebugNSLog(@"Retrieved data: %@", [wrapper responseAsText]);
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
	
	if (wrapper.lastStatusCode != 200) 
	{
		[[Utils sharedInstance] showAlert:[NSString stringWithFormat:@"Code: %d Data: %@", 
										   wrapper.lastStatusCode, 
										   [wrapper responseAsText]] 
								withTitle:NSLocalizedString(kErrorText, kErrorText)];
		return;
	}
	
	// Persist the User object (temp file in /tmp)
	NSDictionary* dict = (NSDictionary*)[wrapper responseAsJson];
	[[Utils sharedInstance] savePlistToTemp:dict withName:kPlistUserObjectTemp];
	// close
	[self close];
	// send notification
	NSNotification* notif = [NSNotification notificationWithName:ODRESTAuthenticateSuccess object:dict];
	[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
}

- (void) wrapper:(RESTWrapper*)wrapper didFailWithError:(NSError*)error
{
	NSLog(@"REST call error: %@", [error description]);
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
}

- (void) wrapper:(RESTWrapper*)wrapper didReceiveStatusCode:(int)statusCode
{
	DebugNSLog(@"Status code: %d", statusCode);
	switch(statusCode) 
	{
		case 200:
			DebugNSLog(@"Success!");
			break;
		default:
		{
			NSNotification* notif = [NSNotification notificationWithName:ODRESTAuthenticateFail object:nil];
			[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
		}
			break;
	}
}

#pragma mark -
#pragma mark UIAlertViewDelegate

- (void)alertView:(UIAlertView*)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
	if (alertView.tag != kActionSheetUseSavedCredentials) {
		return;
	}
	
	switch(buttonIndex)
	{
		case 1: // Yes
			if (self.noReauth) {
				NSDictionary* userDict = [[Utils sharedInstance] getTempPlist:kPlistUserObjectTemp];
				// send notification
				NSNotification* notif = [NSNotification notificationWithName:ODRESTAuthenticateSuccess object:userDict];
				[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
				// close the view
				[self close];
			} else {
				[self authenticateWithKey:self.oauthAccount.oauth_token andSecret:self.oauthAccount.oauth_token_secret];
			}
			break;
		case 0: // No
		default:
			break;
	}
}

@end
