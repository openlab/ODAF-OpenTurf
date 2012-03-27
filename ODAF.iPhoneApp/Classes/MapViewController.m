//
//  MapViewController.m
//  VanGuide
//
//  Created by shazron on 09-12-14.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import "MapViewController.h"
#import "MapDataSource.h"
#import "MapDataSourceModel.h"
#import "AtomParser.h"
#import "KmlPlacemark.h"
#import "ODComments.h"
#import "ODSummaries.h"
#import "RESTWrapper.h"
#import "OAuthBrowserViewController.h"
#import "SignInViewController.h"
#import "VanGuideAppDelegate.h"

@implementation MapViewController

@synthesize segmentedControl, filterThread, pointDataAtomParser, pendingNotification;
@synthesize regionDataAtomParser, communityDataAtomParser, authenticateParentViewController;
@synthesize filterButton, popoverController;

#pragma mark -
#pragma mark UINavigationController delegate

- (void)navigationController:(UINavigationController *)navController 
	  willShowViewController:(UIViewController *)viewController animated:(BOOL)animated
{
	if (self == viewController) {
		navController.navigationBarHidden = YES;
	} else {
		navController.navigationBarHidden = NO;
	}
}

#pragma mark -


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

- (void) dealloc 
{
	self.pointDataAtomParser = nil;
	self.regionDataAtomParser = nil;
	self.communityDataAtomParser = nil;
	[self.filterThread cancel];
	self.filterThread = nil;
	
    [super dealloc];
}

- (BOOL) reachableWithFailureAlert:(BOOL)showFailureAlert
{
	NSString* reachableUrl = [[Utils sharedInstance] getConfigSetting:kConfigKeyReachableUrl];
	NSNumber* devMode = [[Utils sharedInstance] getConfigSetting:kConfigKeyDevMode];
	if ([devMode boolValue]) {
		NSDictionary* devSettings = [[Utils sharedInstance] getConfigSetting:kConfigKeyDevSettings];
		reachableUrl = [devSettings objectForKey:kConfigKeyReachableUrl];
	}
	
	BOOL reachable = [[Utils sharedInstance] isNetworkReachable:reachableUrl];
	if (!reachable && showFailureAlert) {
		
		NSString* caption = @"You need a Wi-Fi or data connection to the Internet to see the map data.";
		NSString* aTitle = @"No Internet Connection";
		
		[[Utils sharedInstance] showAlert:NSLocalizedString(caption, caption)
								withTitle:NSLocalizedString(aTitle, aTitle)];
		
		[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
	}
	
	return reachable;
}

- (void) viewDidLoad 
{
    [super viewDidLoad];
	
	// Register for AuthorizationRequired notif
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(authorizationNeeded:) 
												 name:ODRESTAuthorizationRequired object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(reachabilityChanged:) 
												 name:ODNetworkReachabilityChanged object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(noFiltersSelected:) 
												 name:ODNoFiltersSelected object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(viewComment:) 
												 name:ODViewComment object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(closePopover:) 
												 name:ODClosePopover object:nil];
	
	//[self.segmentedControl addTarget:self action:@selector(mapTypeChanged:) forControlEvents:UIControlEventValueChanged];
	
	BOOL reachable = [self reachableWithFailureAlert:YES];
	if (reachable) 
	{
		[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false withText:NSLocalizedString(kDownloadingText, kDownloadingText)] 
									withView:self.view];
		[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
		
		[self performSelector:@selector(loadMapDataSources) withObject:self afterDelay:0.5];
	}
}

- (void) reachabilityChanged:(NSNotification*)notification
{
	BOOL reachable = [self reachableWithFailureAlert:YES];
	if (reachable) 
	{
		NSString* caption = @"Your Internet connection has been restored. Do you want to reload the map data?";
		NSString* aTitle  = @"Internet Connection Restored";
		NSString* cancel  = @"No";
		NSString* proceed = @"Yes";
		
		UIAlertView* alert = [[UIAlertView alloc]
								   initWithTitle:NSLocalizedString(aTitle, aTitle)
								   message:NSLocalizedString(caption, caption) delegate:self 
								   cancelButtonTitle:NSLocalizedString(cancel, cancel)
								   otherButtonTitles:NSLocalizedString(proceed, proceed)];
		[alert show];
		[alert release];
	}
}

- (void) alertView:(UIAlertView*)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
	if (buttonIndex == 1) // Yes selected
	{ 
		[[MapDataSourceModel sharedInstance] reinitialize];
		NSArray* all = [[self allMapAnnotations] copy];
		[self removeMapAnnotations:all];
		[all release];
		
		[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false 
																							   withText:NSLocalizedString(kDownloadingText, kDownloadingText)] 
									withView:self.view];
		[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
		[self performSelector:@selector(loadMapDataSources) withObject:self afterDelay:0.5];
	}
}

-(BOOL)canBecomeFirstResponder 
{
	return YES;
}

-(void)viewDidAppear:(BOOL)animated 
{
	[super viewDidAppear:animated];
	[self becomeFirstResponder];
}

- (void)viewWillDisappear:(BOOL)animated 
{
	[self resignFirstResponder];
	[super viewWillDisappear:animated];
}

- (void) loadMapDataSources
{
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(plotMapAnnotation:) name:ODAddMapAnnotation object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(plotRegionAnnotation:) name:ODAddRegionAnnotation object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(mapDataFiltered:) name:ODMapDataFiltered object:nil];
	
	NSNumber* loadRemoteSources = [[Utils sharedInstance] getConfigSetting:kConfigKeyLoadRemoteSources];
	NSString* webAppUrl = [[Utils sharedInstance] webAppUrl];
	
	NSString* sourcesXmlFile = nil;
	NSString* sourcePath = nil;
	NSURL* url = nil;
	
	sourcesXmlFile = [[Utils sharedInstance] getConfigSetting:kConfigKeyPointDataSourcesFile];
	if ([loadRemoteSources boolValue]) {
		sourcePath = [NSString stringWithFormat:@"%@/%@", webAppUrl, sourcesXmlFile];
		url = [NSURL URLWithString:sourcePath];
	} else {
		sourcePath = [[NSBundle mainBundle] pathForResource:sourcesXmlFile ofType:@"" inDirectory:@""];
		url = [NSURL fileURLWithPath:sourcePath];
	}
	
	self.pointDataAtomParser = [[AtomParser alloc] initWithContentsOfURL:url andDataType:PointData];
	[[MapDataSourceModel sharedInstance] mapSourcesWillLoad:PointData];
	[self.pointDataAtomParser parse];
	
	sourcesXmlFile = [[Utils sharedInstance] getConfigSetting:kConfigKeyRegionDataSourcesFile];
	if ([loadRemoteSources boolValue]) {
		sourcePath = [NSString stringWithFormat:@"%@/%@", webAppUrl, sourcesXmlFile];
		url = [NSURL URLWithString:sourcePath];
	} else {
		sourcePath = [[NSBundle mainBundle] pathForResource:sourcesXmlFile ofType:@"" inDirectory:@""];
		url = [NSURL fileURLWithPath:sourcePath];
	}
			  
	self.regionDataAtomParser = [[AtomParser alloc] initWithContentsOfURL:url andDataType:RegionData];
	[[MapDataSourceModel sharedInstance] mapSourcesWillLoad:RegionData];
	[self.regionDataAtomParser parse];
	
	sourcesXmlFile = [[Utils sharedInstance] getConfigSetting:kConfigKeyCommunityDataSourcesFile];
	if ([loadRemoteSources boolValue]) {
		sourcePath = [NSString stringWithFormat:@"%@/%@", webAppUrl, sourcesXmlFile];
		url = [NSURL URLWithString:sourcePath];
	} else {
		sourcePath = [[NSBundle mainBundle] pathForResource:sourcesXmlFile ofType:@"" inDirectory:@""];
		url = [NSURL fileURLWithPath:sourcePath];
	}
				   
	self.communityDataAtomParser = [[AtomParser alloc] initWithContentsOfURL:url andDataType:SocialData];
	[[MapDataSourceModel sharedInstance] mapSourcesWillLoad:SocialData];
	[self.communityDataAtomParser parse];
}

- (void) noFiltersSelected:(NSNotification*)notification
{
	if ([self.modalViewController isKindOfClass:[FlipsideViewController class]]) {
		// Filters page already shown, skip
		DebugNSLog(@"Already on filters page, skipping action sheet.");
		return;
	}
	
	NSString* caption = @"There are no filters selected. Select some filters?";
	NSString* cancel  = @"Cancel";
	NSString* action  = @"Select Filters";
	
	UIActionSheet* sheet = [[UIActionSheet alloc]
								 initWithTitle:NSLocalizedString(caption, caption)
								 delegate:self
								 cancelButtonTitle:NSLocalizedString(cancel, cancel)
								 destructiveButtonTitle:nil
								 otherButtonTitles:NSLocalizedString(action, action),nil];

	sheet.tag = kActionSheetNoFiltersSelected;
	sheet.actionSheetStyle = UIActionSheetStyleAutomatic;
	[sheet showInView:self.view];
	[sheet release];
}

- (void) mapTypeChanged:(id)sender
{
//	NSUInteger index = self.segmentedControl.selectedSegmentIndex;
}

- (void) viewComment:(NSNotification*)notification
{
	NSLog(@"MapViewController.viewComment: Override in your subclass.");
	[self doesNotRecognizeSelector:_cmd];
}

- (void) plotMapAnnotation:(NSNotification*)notification
{
	NSLog(@"MapViewController.plotMapAnnotation: Override in your subclass.");
	[self doesNotRecognizeSelector:_cmd];
}

- (void) plotRegionAnnotation:(NSNotification*)notification
{
	NSLog(@"MapViewController.plotRegionAnnotation: Override in your subclass.");
	[self doesNotRecognizeSelector:_cmd];
}

- (NSArray*) allMapAnnotations
{
	NSLog(@"MapViewController.allMapAnnotations: Override in your subclass.");
	[self doesNotRecognizeSelector:_cmd];
	return nil;
}

- (NSString*) categoryTypeForAnnotation:(id)annotation
{
	NSLog(@"MapViewController.categoryTypeForAnnotation: Override in your subclass.");
	[self doesNotRecognizeSelector:_cmd];
	return nil;
}

- (void) removeMapAnnotations:(NSArray*)annotations
{
	NSLog(@"MapViewController.removeMapAnnotations: Override in your subclass.");
	[self doesNotRecognizeSelector:_cmd];
}

- (void) goToHomeLocation
{
	NSLog(@"MapViewController.goToHomeLocation: Override in your subclass.");
	[self doesNotRecognizeSelector:_cmd];
}

- (void) goToMyLocation
{
	NSLog(@"MapViewController.goToMyLocation: Override in your subclass.");
	[self doesNotRecognizeSelector:_cmd];
}

- (void) mapDataFiltered:(NSNotification*)notification
{
	NSArray* uuids_to_add = [MapDataSourceModel sharedInstance].changeSet.additions;
	
	NSEnumerator *enumerator = [[MapDataSourceModel sharedInstance].mapPointDataSources objectEnumerator];
	MapDataSource* dataSource;
	
	while (dataSource = [enumerator nextObject]) {
		if ([uuids_to_add containsObject:dataSource.uuid]) {
			DebugNSLog(@"Loading annotations for MapDataSource (%@)", dataSource.title);
			//dataSource.delegate = self;
			
			if (dataSource.dataType == SocialData) {
				[dataSource performSelectorOnMainThread:@selector(startLoadingForSourceType:) withObject:kSourceTypeJson waitUntilDone:NO];
			} else {
				[dataSource performSelectorOnMainThread:@selector(startLoadingForSourceType:) withObject:kSourceTypeKml waitUntilDone:NO];
			}
		}
	}

	enumerator = [[MapDataSourceModel sharedInstance].mapSocialDataSources objectEnumerator];
	while (dataSource = [enumerator nextObject]) {
		if ([uuids_to_add containsObject:dataSource.uuid]) {
			DebugNSLog(@"Loading annotations for MapDataSource (%@)", dataSource.title);
			//dataSource.delegate = self;
			
			if (dataSource.dataType == SocialData) {
				[dataSource performSelectorOnMainThread:@selector(startLoadingForSourceType:) withObject:kSourceTypeJson waitUntilDone:NO];
			} else {
				[dataSource performSelectorOnMainThread:@selector(startLoadingForSourceType:) withObject:kSourceTypeKml waitUntilDone:NO];
			}
		}
	}
}

- (void) stopLoadingAnnotations
{
	NSArray* uuids_to_remove = [MapDataSourceModel sharedInstance].changeSet.deletions;
	
	NSEnumerator *enumerator = [[MapDataSourceModel sharedInstance].mapPointDataSources objectEnumerator];
	MapDataSource* dataSource;
	
	while (dataSource = [enumerator nextObject]) {
		if ([uuids_to_remove containsObject:dataSource.uuid]) {
			DebugNSLog(@"Stop loading annotations for MapDataSource (%@)", dataSource.title);
			[dataSource cancelLoading];
		}
	}
	
	enumerator = [[MapDataSourceModel sharedInstance].mapSocialDataSources objectEnumerator];
	while (dataSource = [enumerator nextObject]) {
		if ([uuids_to_remove containsObject:dataSource.uuid]) {
			DebugNSLog(@"Stop loading annotations for MapDataSource (%@)", dataSource.title);
			[dataSource cancelLoading];
		}
	}
}

- (IBAction) showFilters 
{  
	if ([[VanGuideAppDelegate sharedApplicationDelegate] topViewControllerOnStack] != self) {	
		return;
	}

	[[MapDataSourceModel sharedInstance] reset];
	
	FlipsideViewController* viewController = [[FlipsideViewController alloc] initWithNibName:@"FlipsideViewController" bundle:nil];
	viewController.delegate = self;
	

	if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
	{
		id popover = [[NSClassFromString(@"UIPopoverController") alloc] initWithContentViewController:viewController];
		self.popoverController = popover;          // we retain a pointer so we can release later or re-use
		
		[popover presentPopoverFromBarButtonItem:self.filterButton permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
		[popover release];
	}
	else
	{
		viewController.modalTransitionStyle = UIModalTransitionStyleFlipHorizontal;
		[self presentModalViewController:viewController animated:YES];
	}		
	
	
	[viewController release];
}

- (IBAction) locateMe
{
	[self goToMyLocation];
}

- (IBAction) locateHome
{
	[self goToHomeLocation];
}

- (void) filterAnnotationsThreadFunc:(id)threadObj
{
	NSAutoreleasePool* pool = [[NSAutoreleasePool alloc] init];
	
	NSThread* currentThread = [NSThread currentThread];
	NSMutableArray* annotations_to_remove = [NSMutableArray arrayWithCapacity:20];
	NSArray* uuids_to_filter = [MapDataSourceModel sharedInstance].changeSet.deletions;
	
	@try 
	{
		NSAutoreleasePool* subPool = [[NSAutoreleasePool alloc] init];
		
		NSEnumerator *enumerator = [[self allMapAnnotations] objectEnumerator];
		id anObject;
		
		while (anObject = [enumerator nextObject]) {
			if ([currentThread isCancelled]) { break; }
			
			if ([uuids_to_filter containsObject:[self categoryTypeForAnnotation:anObject]]) {
				[annotations_to_remove addObject:anObject];
			}
		}
		
		[subPool release];
	}
	@catch (NSException *exception) {
		NSLog(@"MapView filterAnnotationsThreadFunc: Caught %@: %@", [exception name], [exception reason]);
	}
	
	[self removeMapAnnotations:annotations_to_remove];
	
	[[NSNotificationCenter defaultCenter] postNotificationName:ODMapDataFiltered object:self];
	
	[pool release];
}

- (void)filterAnnotations
{
	// stop currently running search thread
	if (self.filterThread != nil) {
		[self.filterThread cancel];
		self.filterThread = nil;
	}
	
	// start new thread
	self.filterThread = [[NSThread alloc] initWithTarget:self selector:@selector(filterAnnotationsThreadFunc:) object:nil];
	[self.filterThread start];
}

- (void) authorizationNeeded:(NSNotification*)notification
{
	UIViewController* authParent = (UIViewController*)notification.object;
	
#ifdef XAUTH
	SignInViewController* viewController = [[SignInViewController alloc] init];
	viewController.modalTransitionStyle = UIModalTransitionStyleCoverVertical;
	
	if (authParent.view.tag == 	kViewControllerFilters) {
		viewController.noReauth = YES;
	}
	
	if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
	{
		UINavigationController* navigationController = authParent.navigationController;
		viewController.modalPresentationStyle = UIModalPresentationCurrentContext;//UIModalPresentationFormSheet;
		if (navigationController != nil) {
			[navigationController presentModalViewController:viewController animated:YES];	
		} else {
			[authParent presentModalViewController:viewController animated:YES];	
		}
	}
	else
	{
		[authParent presentModalViewController:viewController animated:YES];	
	}		
	
	[viewController release];
#else	
	NSString* caption = 
		@"You need to sign in to use this feature. VanGuide uses Twitter for sign in, as well as the 'Tweet This' feature. Your Twitter password is not stored by VanGuide.";
	NSString* cancel = @"Cancel";
	NSString* action = @"Sign in with Twitter";
	
	UIActionSheet* sheet = [[UIActionSheet alloc]
							initWithTitle:NSLocalizedString(caption, caption)
							delegate:self
							cancelButtonTitle:NSLocalizedString(cancel, cancel)
							destructiveButtonTitle:nil
							otherButtonTitles:NSLocalizedString(action, action),nil];
	
	self.authenticateParentViewController = authParent;
	
	sheet.tag = kActionSheetUserWillAuthenticate;
	sheet.actionSheetStyle = UIActionSheetStyleAutomatic;
	[sheet showInView:self.authenticateParentViewController.view];
	[sheet release];
#endif
}

- (void) closePopover:(NSNotification*)notification
{
	if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
	{
		[self.popoverController dismissPopoverAnimated:YES];
		self.popoverController = nil;
	}
}

#pragma mark -
#pragma mark FlipsideViewControllerDelegate implementation

- (void) flipsideViewControllerDidFinish:(FlipsideViewController*)controller 
{
	if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
	{
		[self.popoverController dismissPopoverAnimated:YES];
	}
	else
	{
		[self dismissModalViewControllerAnimated:YES];
	}		
	
	
	[self stopLoadingAnnotations];
	[self filterAnnotations];
}

#pragma mark -
#pragma mark UIActionSheetDelegate implementation


- (void) actionSheet:(UIActionSheet*)actionSheet clickedButtonAtIndex:(NSInteger)buttonIndex
{
	switch(actionSheet.tag) {
		case kActionSheetNoFiltersSelected:
			if (buttonIndex == 0) { // Go to Filters
				[self showFilters];
			}
			break;
		case kActionSheetUserWillAuthenticate:
		{
			if (buttonIndex == 0) { // Sign in with Twitter
				OAuthBrowserViewController* viewController = [[OAuthBrowserViewController alloc] init];
				viewController.modalTransitionStyle = UIModalTransitionStyleFlipHorizontal;
				[self.authenticateParentViewController presentModalViewController:viewController animated:YES];	
				[viewController release];
			} else { // Cancel
				self.authenticateParentViewController = nil;
				NSNotification* notif = [NSNotification notificationWithName:ODRESTAuthenticateFail object:nil];
				[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
			}
		}
			break;
		default:
			break;
	}
}

@end
