//
//  FlipsideViewController.m
//  VanGuide
//
//  Created by shazron on 09-11-27.
//  Copyright Nitobi Software Inc. 2009. All rights reserved.
//

#import "FlipsideViewController.h"
#import "MapDataSourceModel.h"
#import "MapDataSource.h"
#import <QuartzCore/QuartzCore.h>
#import "RESTInfo.h"
#import "ODSummaries.h"
#import "OAuthAccount.h"

@implementation FlipsideViewController

@synthesize delegate, tableView, regionPickerView, pickerView, navbar, dataSourceNeedsAuthIndexPath;

- (void)viewDidLoad 
{
    [super viewDidLoad];
    self.view.backgroundColor = [UIColor viewFlipsideBackgroundColor];
	self.navbar.tintColor = [UIColor colorWithHexString:[[Utils sharedInstance] getConfigSetting:kConfigKeyTintColourRGB]];
	
	MapDataSourceModel* dataModel = [MapDataSourceModel sharedInstance];

	self.tableView.editing = NO;
	self.tableView.delegate = dataModel;
	self.tableView.dataSource = dataModel;
	
	self.pickerView.dataSource = dataModel.regionPickerDataSource;
	self.pickerView.delegate = dataModel.regionPickerDataSource;
	self.pickerView.showsSelectionIndicator = YES;
	
//	// put off-screen
//	CGRect frame = self.regionPickerView.frame;
//	frame.origin.x = 0;
//	frame.origin.y = 480;
//	self.regionPickerView.frame = frame;
//	[self.view addSubview:self.regionPickerView];
	
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(showRegionPicker:) name:ODShowRegionPicker object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(regionDataChangedNotification:) name:ODRegionDataChanged object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(dataSourceNeedsAuth:) name:ODYourLandmarksNeedsUpdate object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(authUpdated:) name:ODRESTAuthenticateSuccess object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(refreshFilterList:) name:ODRefreshFilterList object:nil];
}

- (void)viewDidAppear:(BOOL)animated
{
	[super viewDidAppear:animated];
	[self.tableView reloadData];
}

- (void) dataSourceNeedsAuth:(NSNotification*)notification
{
	self.dataSourceNeedsAuthIndexPath = (NSIndexPath*)notification.object;
	[[NSNotificationCenter defaultCenter] addObserver:self 
											 selector:@selector(authUpdated:) name:ODRESTAuthenticateSuccess object:nil];
	
	// launch auth view
	self.view.tag = kViewControllerFilters;
	[[NSNotificationCenter defaultCenter] postNotificationName:ODRESTAuthorizationRequired object:self];
}

- (void) authUpdated:(NSNotification*)notification
{
	// remove notif registration for authenticate
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTAuthenticateSuccess object:nil];
	
	NSDictionary* userDict = (NSDictionary*)notification.object;
	OAuthAccount* oauthAccount = nil;
	
	if (userDict != nil) {
		oauthAccount = [[[OAuthAccount alloc] init] autorelease];
		[oauthAccount setValuesForKeysWithDictionary:userDict];
	}
	
	if (oauthAccount != nil) 
	{
		MapDataSourceModel* dataModel = [MapDataSourceModel sharedInstance];
		RESTInfo* call = [[ODSummaries sharedInstance] ShowByUserId:oauthAccount.Id];

		// set the datasource url
		MapDataSource* mapSource = [dataModel.mapSocialDataSources 
									objectAtIndex:self.dataSourceNeedsAuthIndexPath.row];
		[mapSource.sourceTypes setValue:[call.url absoluteString] forKey:kSourceTypeJson];
		[dataModel.changeSet.additions addObject:mapSource.uuid];
		
		// check the table cell
		UITableViewCell* cell = [self.tableView cellForRowAtIndexPath:self.dataSourceNeedsAuthIndexPath];
		cell.accessoryType = UITableViewCellAccessoryCheckmark;
		
		// save the filter setting
		[dataModel.filterSettings setObject:[NSNumber numberWithInt:YES] forKey:mapSource.uuid];
		[[Utils sharedInstance] savePlistToCache:dataModel.filterSettings withName:kPlistFilter];
	}
}

- (void) refreshFilterList:(NSNotification*)notification
{
	[self.tableView reloadData];
}

- (void) showRegionPicker:(NSNotification*)notification
{
	[CATransaction begin];
	CATransition *animation = [CATransition animation];
	animation.type = kCATransitionMoveIn;
	animation.subtype = kCATransitionFromTop;
	animation.duration = 0.30;
	
	CGRect frame = self.regionPickerView.frame;
	frame.origin.x = 0;
	frame.origin.y = 240;
	self.regionPickerView.frame = frame;
	
	[[self.regionPickerView layer] addAnimation:animation forKey:@"myanimationkey"];
	[CATransaction commit];
}

- (IBAction)buttonChooseFiltersDoneSelected 
{
	[self.delegate flipsideViewControllerDidFinish:self];	
}

- (IBAction) buttonRegionPickerDoneSelected
{
	NSInteger selectedRow = [self.pickerView selectedRowInComponent:0];
	NSNumber* num = (selectedRow <= 0)? nil :[NSNumber numberWithInt:selectedRow-1];
	
	[[NSNotificationCenter defaultCenter] postNotificationName:ODRegionDataChanged object:num];
	
	[CATransaction begin];
	CATransition *animation = [CATransition animation];
	animation.type = kCATransitionReveal;
	animation.subtype = kCATransitionFromBottom;
	animation.duration = 0.30;
	
	CGRect frame = self.regionPickerView.frame;
	frame.origin.x = 0;
	frame.origin.y = 480;
	self.regionPickerView.frame = frame;
	
	[[self.regionPickerView layer] addAnimation:animation forKey:@"myanimationkey"];
	[CATransaction commit];
}

- (void) regionDataChangedNotification:(NSNotification*)notification
{
	NSNumber* num = notification.object;
	MapDataSourceModel* dataModel = [MapDataSourceModel sharedInstance];
	
	if (num == nil) {
		[dataModel.filterSettings removeObjectForKey:ODCurrentRegionData];
	} 
	else {
		MapDataSource*  mapDataSource = [dataModel.regionPickerDataSource.mapRegionDataSources objectAtIndex:[num intValue]];
		if (mapDataSource != nil) {
			[dataModel.filterSettings setObject:mapDataSource.uuid forKey:ODCurrentRegionData];
		}
	}
	
	[[Utils sharedInstance] savePlistToCache:dataModel.filterSettings withName:kPlistFilter];
	[self.tableView reloadData];
}

- (void)didReceiveMemoryWarning 
{
	// Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
	
	// Release any cached data, images, etc that aren't in use.
	// terminate all pending download connections
	MapDataSourceModel* dataModel = [MapDataSourceModel sharedInstance];
    NSArray* allDownloads = [dataModel.imageDownloadsInProgress allValues];
    [allDownloads performSelector:@selector(cancel)];
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


@end
