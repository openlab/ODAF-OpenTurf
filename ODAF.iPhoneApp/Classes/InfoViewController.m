//
//  InfoViewController.m
//  VanGuide
//
//  Created by shazron on 10-02-22.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "InfoViewController.h"
#import "ODHome.h"
#import "RESTInfo.h"

@implementation InfoViewController

@synthesize webView, backButton, forwardButton, topToolbar, bottomToolbar;


- (BOOL) shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
	if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
	{
		return YES;
	}
	else
	{
		return NO;
	}
}

- (void)viewDidLoad 
{
    [super viewDidLoad];
	self.topToolbar.tintColor = [UIColor colorWithHexString:[[Utils sharedInstance] getConfigSetting:kConfigKeyTintColourRGB]];
	self.bottomToolbar.tintColor = self.topToolbar.tintColor;

	RESTInfo* call = [[ODHome sharedInstance] Credits];
	NSURLRequest *request = [NSURLRequest requestWithURL:call.url];
	[self.webView loadRequest:request];
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
}

- (void)dealloc 
{
	self.webView.delegate = nil;
	[super dealloc];
}

- (void)viewDidDisappear:(BOOL)animated
{
	[[Utils sharedInstance] loadingStop];
	[super viewDidDisappear:animated];
}

- (IBAction) close
{
	[[super parentViewController] dismissModalViewControllerAnimated:YES];
}

#pragma mark -
#pragma mark UIWebViewDelegate implementation

- (void)webView:(UIWebView*)webView didFailLoadWithError:(NSError *)error
{
	[[Utils sharedInstance] loadingStop];
	
	NSLog(@"Webview did fail load with error: %@", [error localizedDescription]);
	[[Utils sharedInstance] showAlert:[error localizedDescription] 
							withTitle:NSLocalizedString(kWebBrowserErrorText, kWebBrowserErrorText)];
}

- (BOOL)webView:(UIWebView*)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType
{
	return YES;
}

- (void)webViewDidFinishLoad:(UIWebView*)aWebView
{
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
	[[Utils sharedInstance] loadingStop];

	self.backButton.enabled = aWebView.canGoBack;
	self.forwardButton.enabled = aWebView.canGoForward;
}

- (void)webViewDidStartLoad:(UIWebView*)webView
{
	NSDictionary* options = [[Utils sharedInstance] createLoadingViewOptionsFullScreen:NO withText:NSLocalizedString(kLoadingText, kLoadingText)];

	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	[[Utils sharedInstance] loadingStart:options withView:self.view];
}

@end
