//
//  OAuthBrowserViewController.m
//
//

#import "OAuthBrowserViewController.h"
#import "ODUser.h"
#import "RESTWrapper.h"


@implementation GetAccessTokenResponse

@synthesize oauth_token, oauth_token_secret, oauth_token_expiry;

- (void) dealloc
{
	self.oauth_token = nil;
	self.oauth_token_secret = nil;
	self.oauth_token_expiry = nil;
	
	[super dealloc];
}

@end

@implementation RequestAuthTokenResponse

@synthesize oauth_token, oauth_token_secret, link;

- (void) dealloc
{
	self.oauth_token = nil;
	self.oauth_token_secret = nil;
	self.link = nil;

	[super dealloc];
}

@end

@implementation OAuthBrowserViewController

@synthesize webView, spinner, toolbar;
@synthesize requestAuthTokenResponse, getAccessTokenResponse, currentMode;

- (void) requestAuthToken
{
	ODUser* userAPI = [ODUser sharedInstance];
	RESTInfo* call = [userAPI RequestAuthToken];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:true 
																						   withText:NSLocalizedString(kInitializingText, kInitializingText)] 
								withView:self.view];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = RequestAuthToken;
	[userAPI.webservice sendRequest:call];
}

- (void) getAccessToken
{
	ODUser* userAPI = [ODUser sharedInstance];
	RESTInfo* call = [userAPI GetAccessToken:self.requestAuthTokenResponse.oauth_token];
	call.delegate = self;
		
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:true 
																						   withText:NSLocalizedString(kVerifyingText, kVerifyingText)] 
								withView:self.view];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = GetAccessToken;
	[userAPI.webservice sendRequest:call];
}

- (void) authenticateUser
{
	ODUser* userAPI = [ODUser sharedInstance];
	RESTInfo* call = [userAPI Authenticate:self.getAccessTokenResponse.oauth_token 
						   oauthTokenSecret:self.getAccessTokenResponse.oauth_token_secret];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:true 
																						   withText:NSLocalizedString(kAuthenticatingText, kAuthenticatingText)] 
								withView:self.view];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = Authenticate;
	[userAPI.webservice sendRequest:call];
}

- (void) viewDidLoad 
{
    [super viewDidLoad];

	self.webView.scalesPageToFit = YES;
	self.webView.backgroundColor = [UIColor whiteColor];
	self.currentMode = OAuthModeNone;
	
	self.toolbar.tintColor = [UIColor colorWithHexString:[[Utils sharedInstance] getConfigSetting:kConfigKeyTintColourRGB]];
	
	// Request auth token, then load the url for Twitter OAuth
	[self performSelector:@selector(requestAuthToken) withObject:nil afterDelay:0.5];
}

- (BOOL) shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation 
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void) didReceiveMemoryWarning 
{
	// Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
	
	// Release any cached data, images, etc that aren't in use.
}

- (void) viewDidUnload 
{
	// Release any retained subviews of the main view.
	// e.g. self.myOutlet = nil;

	self.webView.delegate = nil;
	self.webView = nil;

	self.spinner = nil;

	self.requestAuthTokenResponse = nil;
	self.getAccessTokenResponse = nil;
}

- (void) dealloc 
{
	self.webView.delegate = nil;
	self.webView = nil;
	
	self.spinner = nil;
	
	self.requestAuthTokenResponse = nil;
	self.getAccessTokenResponse = nil;
	
	[super dealloc];
}

- (IBAction) onDoneButtonPress:(id)sender
{
	[[super parentViewController] dismissModalViewControllerAnimated:YES];
}

- (IBAction) onNewButtonPress:(id)sender
{
	[self loadURL:@"https://twitter.com/signup"];
}

- (void) loadURL:(NSString*)url
{
	NSURLRequest *request = [NSURLRequest requestWithURL:[NSURL URLWithString:url]];
	
    if ([webView isLoading]) {
        [webView stopLoading];
	}

	self.webView.hidden = NO;
	[self.webView loadRequest:request];
}

- (void) close
{
	[[super parentViewController] dismissModalViewControllerAnimated:YES];
}

- (void)viewWillDisappear
{
    if ([webView isLoading]) {
        [webView stopLoading];
	}
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
	
	// at this point, Status Code is 200 (OK)
	switch (self.currentMode) 
	{
		case RequestAuthToken:
		{
			self.requestAuthTokenResponse = [[RequestAuthTokenResponse alloc] init];
			NSDictionary* dict = (NSDictionary*)[wrapper responseAsJson];
			[self.requestAuthTokenResponse setValuesForKeysWithDictionary:dict];

			// load the OAuth authorization link
			[self loadURL:self.requestAuthTokenResponse.link];
		}
			break;
		case GetAccessToken:
		{
			self.getAccessTokenResponse = [[GetAccessTokenResponse alloc] init];
			NSDictionary* dict = (NSDictionary*)[wrapper responseAsJson];
			[self.getAccessTokenResponse setValuesForKeysWithDictionary:dict];
			
			// Got the access tokens, persist them to /Library/Caches, 
			[[Utils sharedInstance] savePlistToCache:dict withName:kPlistUserCredentialsCache];
			//now authenticate with our server
			[self authenticateUser];
		}
			break;
		case Authenticate:
		{
			// Persist the User object (temp file in /tmp)
			NSDictionary* dict = (NSDictionary*)[wrapper responseAsJson];
			[[Utils sharedInstance] savePlistToTemp:dict withName:kPlistUserObjectTemp];
			// close
			[self close];
			// send notification
			NSNotification* notif = [NSNotification notificationWithName:ODRESTAuthenticateSuccess object:dict];
			[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
		}
			break;
		default:
		{
		}
			break;
	}
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
		{
			DebugNSLog(@"Success!");
		}
			break;
		default:
			switch (self.currentMode)
			{
				case RequestAuthToken:
					break;
				case GetAccessToken:
					break;
				case Authenticate:
				{
					NSNotification* notif = [NSNotification notificationWithName:ODRESTAuthenticateFail object:nil];
					[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
				}
				break;
			}
			break;
	}
}

#pragma mark -
#pragma mark UIWebViewDelegate

- (BOOL) webView:(UIWebView*)webView shouldStartLoadWithRequest:(NSURLRequest*)request 
	navigationType:(UIWebViewNavigationType)navigationType
{
	DebugNSLog(@"Loading: %@", [[request URL] description]); 
	
	NSURL* url = [request URL];
	
	// its a Twitter callback to us for authorization
	if ([[[url path] lowercaseString] isEqualToString:@"/user/authorizereturn"]) {
		
		DebugNSLog(@"Authorize return.");
		[[Utils sharedInstance] loadingStop];
		[self getAccessToken];
		return NO;
	}
	
	// its Twitter trying to authenticate here…
	if (self.currentMode == RequestAuthToken && [[request HTTPBody] length] > 0) {
		
		// Removed loading screen, because if Auth fails we will wait forever…
	}
		
	return YES;
}

- (void) webView:(UIWebView*)webView didFailLoadWithError:(NSError*)error
{
	[spinner stopAnimating];
	
	NSLog(@"Webview did fail load with error: %@", [error localizedDescription]);
	//[[Utils sharedInstance] showAlert:[error localizedDescription] withTitle:NSLocalizedString(kWebBrowserErrorText, kWebBrowserErrorText)];
}


- (void) webViewDidStartLoad:(UIWebView*)sender 
{
	[spinner startAnimating];
}

- (void) webViewDidFinishLoad:(UIWebView*)sender 
{
	[spinner stopAnimating];
}


@end
