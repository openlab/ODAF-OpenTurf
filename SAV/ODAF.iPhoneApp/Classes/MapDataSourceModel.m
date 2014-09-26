//
//  MapDataSourceModel.m
//  VanGuide
//
//  Created by shazron on 09-12-07.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import "MapDataSourceModel.h"
#import "MapDataSource.h"
#import "KmlPlacemark.h"
#import "RegionPickerDataSource.h"
#import "RESTInfo.h"
#import "ODSummaries.h"
#import "ImageDownloader.h"
#import "MapAnnotation.h"
#import "OAuthAccount.h"

static MapDataSourceModel* sharedInstance = nil;


@implementation MapDataSourceChangeSet

@synthesize additions, deletions;

- (id) init
{
	if (self == [super init]) {
		self.additions = [NSMutableArray arrayWithCapacity:3];
		self.deletions = [NSMutableArray arrayWithCapacity:3];
	}
	
	return self;
}

- (void) dealloc
{
	self.additions = nil;
	self.deletions = nil;
	
	[super dealloc];
}

@end


@implementation MapDataSourceModel

@synthesize mapPointDataSources, mapSocialDataSources, regionPickerDataSource;
@synthesize iconMapSettings, filterSettings, changeSet, imageDownloadsInProgress;

- (id) init
{
	if (self == [super init]) {
		[self reinitialize];
	}
	
	return self;
}

- (void) reinitialize
{
	[self reset];
	
	self.mapPointDataSources = [NSMutableArray arrayWithCapacity:10];
	self.mapSocialDataSources = [NSMutableArray arrayWithCapacity:10];
	self.regionPickerDataSource = [[[RegionPickerDataSource alloc] init] autorelease];
	self.iconMapSettings = [[Utils sharedInstance] getBundlePlist:kPlistIconMap];
	self.filterSettings = [[Utils sharedInstance] getCachePlist:kPlistFilter];
	self.imageDownloadsInProgress = [NSMutableDictionary dictionaryWithCapacity:10];
	
	// add 'Your Landmarks' social data option
	[self.mapSocialDataSources addObject:[MapDataSource yourLandmarksDataSource]];
}

- (void) reset
{
	if (self.changeSet != nil) {
		[self.changeSet.additions removeAllObjects];
		[self.changeSet.deletions removeAllObjects];
	} else {
		self.changeSet = [[MapDataSourceChangeSet alloc] init];
	}
}

- (void) startImageDownload:(NSString*)url forTableView:(UITableView*) tableView atIndexPath:(NSIndexPath*)indexPath
{
    ImageDownloader* downloader = [self.imageDownloadsInProgress objectForKey:indexPath];
    if (downloader == nil) 
    {
        downloader = [[ImageDownloader alloc] init];
        downloader.tableView = tableView;
        downloader.indexPath = indexPath;
        downloader.delegate = self;
        [self.imageDownloadsInProgress setObject:downloader forKey:indexPath];
        [downloader start:url];
        [downloader release];   
    }
}


#pragma mark -
#pragma mark MapSourceDelegate protocol

- (void) mapSourceParsed:(MapDataSource*)dataSource forDataType:(MapDataType)dataType
{
	dataSource.dataType = dataType;
	switch (dataType) {
		case RegionData:
			[self.regionPickerDataSource.mapRegionDataSources addObject:dataSource];
			break;
		case PointData:
			[self.mapPointDataSources addObject:dataSource];
			break;
		case SocialData:
			[self.mapSocialDataSources addObject:dataSource];
			break;
	}
}

- (void) mapSourcesWillLoad:(MapDataType)dataType
{
	[Utils sharedInstance].loadingView.textLabel.text = NSLocalizedString(kLoadingSourcesText, kLoadingSourcesText);

	switch (dataType) {
		case RegionData:
			DebugNSLog(@"Loading RegionData map sources…");
			break;
		case PointData:
			DebugNSLog(@"Loading PointData map sources…");
			break;
		case SocialData:
			DebugNSLog(@"Loading SocialData map sources…");
			break;
	}
}

- (void) mapSourcesDidLoad:(MapDataType)dataType
{
	[Utils sharedInstance].loadingView.textLabel.text = NSLocalizedString(kSourcesLoadedText, kSourcesLoadedText);
	
	static BOOL firstFilterSetup = NO;
	BOOL nothingToLoad = YES;
	
	// set up the filter settings
	if (self.filterSettings == nil) {
		self.filterSettings = [[NSMutableDictionary alloc] initWithCapacity:10];
		firstFilterSetup = YES;
	}
	
	// check if no filters are set
	if (![[self.filterSettings allValues] containsObject:[NSNumber numberWithInt:YES]] || firstFilterSetup) {
		NSNotification* notif = [NSNotification notificationWithName:ODNoFiltersSelected object:nil];
		[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
	}
	
	NSUInteger section = 0;
	NSEnumerator *enumerator = nil;
	switch (dataType) {
		case RegionData:
			enumerator = [self.regionPickerDataSource.mapRegionDataSources objectEnumerator];
			section = REGION_DATA_SECTION;
			break;
		case PointData:
			enumerator = [self.mapPointDataSources objectEnumerator];
			section = POINT_DATA_SECTION;
			break;
		case SocialData:
			enumerator = [self.mapSocialDataSources objectEnumerator];
			section = SOCIAL_DATA_SECTION;
			break;
		default:
			return;
	}
	
	MapDataSource* anObject;
	
	while (anObject = [enumerator nextObject]) {
		anObject.delegate = self;
		
		if (firstFilterSetup) {
			[self.filterSettings setObject:[NSNumber numberWithInt:NO] forKey:anObject.uuid];
		}
		
		NSNumber* doLoadObj = [self.filterSettings objectForKey:anObject.uuid];
		BOOL doLoad = (doLoadObj == nil) ? NO : [doLoadObj boolValue];

		if (doLoad) {
			DebugNSLog(@"Loading mapsource %@", anObject.title);
			if (dataType == SocialData) {
				[anObject performSelector:@selector(startLoadingForSourceType:) withObject:kSourceTypeJson afterDelay:0.1];
			} else {
				[anObject performSelector:@selector(startLoadingForSourceType:) withObject:kSourceTypeKml afterDelay:0.1];
			}
			nothingToLoad = NO;
		} else {
			DebugNSLog(@"Skipping mapsource %@, filtered out.", anObject.title);
		}
	}
	
	if (nothingToLoad){
		[[Utils sharedInstance] loadingStop];
	} else {
		NSNotification* notif = [NSNotification notificationWithName:ODRefreshFilterList object:[NSNumber numberWithInt:section]];
		[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
	}
}

#pragma mark -
#pragma mark MapDataDelegate protocol

- (void) mapDataParsed:(MapDataSource*)dataSource placeMark:(KmlPlacemark*)placeMark 
{
	MapAnnotation* annotation = [[[MapAnnotation alloc] init] autorelease];
	annotation.dataSource = dataSource;
	annotation.placemark = placeMark;
	
	if (placeMark.isPolygon) {
		[[NSNotificationCenter defaultCenter] postNotificationName:ODAddRegionAnnotation object:annotation];
	} else {
		[[NSNotificationCenter defaultCenter] postNotificationName:ODAddMapAnnotation object:annotation];
	}
}

- (void) mapDataWillLoad:(MapDataSource*)dataSource
{
}

- (void) mapDataDidLoad:(MapDataSource*)dataSource
{
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
}

#pragma mark -
#pragma mark Singleton methods

+ (MapDataSourceModel*) sharedInstance
{
    @synchronized(self)
    {
        if (sharedInstance == nil) {
			sharedInstance = [[MapDataSourceModel alloc] init];
		}
    }
    return sharedInstance;
}

+ (id)allocWithZone:(NSZone *)zone {
    @synchronized(self) {
        if (sharedInstance == nil) {
            sharedInstance = [super allocWithZone:zone];
            return sharedInstance;  // assignment and return on first allocation
        }
    }
    return nil; // on subsequent allocation attempts return nil
}

- (id)copyWithZone:(NSZone *)zone
{
    return self;
}

- (id)retain {
    return self;
}

- (unsigned)retainCount {
    return UINT_MAX;  // denotes an object that cannot be released
}

- (void)release {
    //do nothing
}

- (id)autorelease {
    return self;
}

- (void) dealloc
{
	self.mapPointDataSources = nil;
	self.regionPickerDataSource = nil;
	self.iconMapSettings = nil;
	self.filterSettings = nil;
	
	[super dealloc];
}

#pragma mark -
#pragma mark UITableViewDataSource protocol


- (UITableViewCell*) cellForSocialData:(UITableView*)tableView atIndexPath:(NSIndexPath*)indexPath
{
	static NSString *CellIdentifier = @"map_social_sources";
	MapDataSource* mapSource = nil;
	
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) 
	{
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier] autorelease];
		mapSource = [self.mapSocialDataSources objectAtIndex:indexPath.row];
		
		if (!mapSource.iconUrl) { // no remote image, so we try to get the local one
			NSString* icon = [self.iconMapSettings objectForKey:mapSource.uuid];
			if (icon != nil) {
				mapSource.icon = [UIImage imageNamed:icon];
			}
		} 
		
		if (!mapSource.icon) { // no icon set, we lazy load
            if (tableView.dragging == NO && tableView.decelerating == NO)
            {
				NSString* url = [NSString stringWithFormat:@"%@/%@", 
												   [[Utils sharedInstance] webAppUrl],
												   mapSource.iconUrl];
				[self startImageDownload:url forTableView:tableView atIndexPath:indexPath];
			}			
            // if a download is deferred or in progress, return a placeholder image
            cell.imageView.image = [UIImage imageNamed:kPinCellImagePlaceholder];                
        }
        else
        {
			cell.imageView.image = mapSource.icon;
        }
		
		NSNumber* doShowObj = [self.filterSettings objectForKey:mapSource.uuid];
		BOOL doShow = doShowObj == nil? NO : [doShowObj boolValue];
		
		if (doShow) {
			cell.accessoryType = UITableViewCellAccessoryCheckmark;
		} else {
			cell.accessoryType = UITableViewCellAccessoryNone;
		}
	}
	
	if (mapSource != nil) {
		cell.textLabel.text = mapSource.title;
	}
	
    return cell;
}

- (UITableViewCell*) cellForPointData:(UITableView*)tableView atIndexPath:(NSIndexPath*)indexPath
{
	static NSString *CellIdentifier = @"map_point_sources";
	MapDataSource* mapSource = nil;
	
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) 
	{
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier] autorelease];
		mapSource = [self.mapPointDataSources objectAtIndex:indexPath.row];
		
		if (!mapSource.iconUrl) { // no remote image, so we try to get the local one
			NSString* icon = [self.iconMapSettings objectForKey:mapSource.uuid];
			if (icon != nil) {
				mapSource.icon = [UIImage imageNamed:icon];
			}
		} 
		
		if (!mapSource.icon) { // no icon set, we lazy load
			
            if (tableView.dragging == NO && tableView.decelerating == NO)
            {
				NSString* url = [NSString stringWithFormat:@"%@/%@", 
								 [[Utils sharedInstance] webAppUrl],
								 mapSource.iconUrl];
				[self startImageDownload:url forTableView:tableView atIndexPath:indexPath];
			}			
            // if a download is deferred or in progress, return a placeholder image
            cell.imageView.image = [UIImage imageNamed:kPinCellImagePlaceholder];                
        }
        else
        {
			cell.imageView.image = mapSource.icon;
        }
		
		NSNumber* doShowObj = [self.filterSettings objectForKey:mapSource.uuid];
		BOOL doShow = doShowObj == nil? NO : [doShowObj boolValue];
		
		if (doShow) {
			cell.accessoryType = UITableViewCellAccessoryCheckmark;
		} else {
			cell.accessoryType = UITableViewCellAccessoryNone;
		}
	}
	
	if (mapSource != nil) {
		cell.textLabel.text = mapSource.title;
	}
	
    return cell;
}

- (UITableViewCell*) cellForRegionData:(UITableView*)tableView atIndexPath:(NSIndexPath*)indexPath
{
	static NSString *CellIdentifier = @"map_region_sources";
	NSString* currentRegionData = [self.filterSettings objectForKey:ODCurrentRegionData];
	
	//TODO: use NSPredicate instead
	NSEnumerator *enumerator = [self.regionPickerDataSource.mapRegionDataSources objectEnumerator];
	MapDataSource* mapSource = nil;
	
	while ((mapSource = [enumerator nextObject]) && currentRegionData != nil) {
		if ([mapSource.uuid isEqualToString:currentRegionData]) {
			break;
		}
	}
	
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) 
	{
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier] autorelease];
		
		if (!mapSource.iconUrl) { // no remote image, so we try to get the local one
			NSString* icon = [self.iconMapSettings objectForKey:mapSource.uuid];
			if (icon != nil) {
				mapSource.icon = [UIImage imageNamed:icon];
			}
		} 
		
		if (!mapSource.icon)
        {
            if (tableView.dragging == NO && tableView.decelerating == NO)
            {
				NSString* url = [NSString stringWithFormat:@"%@/%@", 
								 [[Utils sharedInstance] webAppUrl],
								 mapSource.iconUrl];
				[self startImageDownload:url forTableView:tableView atIndexPath:indexPath];
			}			
            // if a download is deferred or in progress, return a placeholder image
            cell.imageView.image = [UIImage imageNamed:kPinCellImagePlaceholder];                
        }
        else
        {
			cell.imageView.image = mapSource.icon;
        }
				
		cell.accessoryType = UITableViewCellAccessoryDetailDisclosureButton;
	} 

	cell.textLabel.text = (currentRegionData == nil) ? NSLocalizedString(kNoneText, kNoneText) : mapSource.title;
	
    return cell;
}

- (UITableViewCell*) tableView:(UITableView*)tableView cellForRowAtIndexPath:(NSIndexPath*)indexPath
{
	switch (indexPath.section) {
//		case REGION_DATA_SECTION:
//			return [self cellForRegionData:tableView atIndexPath:indexPath];
//			break;
		case SOCIAL_DATA_SECTION:
			return [self cellForSocialData:tableView atIndexPath:indexPath];
			break;
		case POINT_DATA_SECTION:
		default:
			return [self cellForPointData:tableView atIndexPath:indexPath];
			break;
	}
}

- (NSInteger) numberOfSectionsInTableView:(UITableView*)tableView
{
	return 2;
}

- (NSInteger) tableView:(UITableView*)tableView numberOfRowsInSection:(NSInteger)section
{
	switch (section) {
//		case REGION_DATA_SECTION:
//			return 1;
		case SOCIAL_DATA_SECTION:
			return [mapSocialDataSources count];
		case POINT_DATA_SECTION:
		default:
			return [mapPointDataSources count];
	}
}

- (NSArray*) sectionIndexTitlesForTableView:(UITableView*)tableView
{
	return nil;
}

- (NSInteger) tableView:(UITableView*)tableView sectionForSectionIndexTitle:(NSString*)title atIndex:(NSInteger)index
{
	return 0;
}

- (NSString*) tableView:(UITableView*)tableView titleForHeaderInSection:(NSInteger)section
{
	switch (section) {
//		case REGION_DATA_SECTION:
//			return NSLocalizedString(kRegionDataText, kRegionDataText);
		case SOCIAL_DATA_SECTION:
			return NSLocalizedString(kSocialLandmarksText, kSocialLandmarksText);
		case POINT_DATA_SECTION:
		default:
			return NSLocalizedString(kPointDataText, kPointDataText);
	}
}

- (NSString*) tableView:(UITableView*)tableView titleForFooterInSection:(NSInteger)section
{
	return nil;
}

- (void) socialDataSelected:(UITableView*)tableView forIndexPath:(NSIndexPath*)indexPath
{
	MapDataSource* mapSource = [self.mapSocialDataSources objectAtIndex:indexPath.row];
	
	NSNumber* doShowObj = [self.filterSettings objectForKey:mapSource.uuid];
	BOOL doShow = doShowObj == nil? NO : [doShowObj boolValue];
	[self.filterSettings setObject:[NSNumber numberWithInt:!doShow] forKey:mapSource.uuid];
	
	UITableViewCell* cell = [tableView cellForRowAtIndexPath:indexPath];
	if (!doShow) {
		
		if ([mapSource.uuid isEqualToString:kYourLandmarksFilter]) 
		{
			// set selection to NO in filter
			[self.filterSettings setObject:[NSNumber numberWithInt:NO] forKey:mapSource.uuid];

			// launch auth view indirectly, return
			NSNotification* notif = [NSNotification notificationWithName:ODYourLandmarksNeedsUpdate object:indexPath];
			[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
			return;
		} 

		cell.accessoryType = UITableViewCellAccessoryCheckmark;

		if ([self.changeSet.deletions containsObject:mapSource.uuid]) { // cancel each other out
			[self.changeSet.deletions removeObject:mapSource.uuid];
		} else {
			[self.changeSet.additions addObject:mapSource.uuid];
		}
		
	} else {
		cell.accessoryType = UITableViewCellAccessoryNone;
		
		if ([self.changeSet.additions containsObject:mapSource.uuid]) { // cancel each other out
			[self.changeSet.additions removeObject:mapSource.uuid];
		} else {
			[self.changeSet.deletions addObject:mapSource.uuid];
		}
	}
	
	[[Utils sharedInstance] savePlistToCache:self.filterSettings withName:kPlistFilter];
}

- (void) pointDataSelected:(UITableView*)tableView forIndexPath:(NSIndexPath*)indexPath
{
	MapDataSource* mapSource = [self.mapPointDataSources objectAtIndex:indexPath.row];
	
	NSNumber* doShowObj = [self.filterSettings objectForKey:mapSource.uuid];
	BOOL doShow = doShowObj == nil? NO : [doShowObj boolValue];
	[self.filterSettings setObject:[NSNumber numberWithInt:!doShow] forKey:mapSource.uuid];
	
	UITableViewCell* cell = [tableView cellForRowAtIndexPath:indexPath];
	if (!doShow) {
		cell.accessoryType = UITableViewCellAccessoryCheckmark;
		
		if ([self.changeSet.deletions containsObject:mapSource.uuid]) { // cancel each other out
			[self.changeSet.deletions removeObject:mapSource.uuid];
		} else {
			[self.changeSet.additions addObject:mapSource.uuid];
		}
		
	} else {
		cell.accessoryType = UITableViewCellAccessoryNone;
		
		if ([self.changeSet.additions containsObject:mapSource.uuid]) { // cancel each other out
			[self.changeSet.additions removeObject:mapSource.uuid];
		} else {
			[self.changeSet.deletions addObject:mapSource.uuid];
		}
	}
	
	[[Utils sharedInstance] savePlistToCache:self.filterSettings withName:kPlistFilter];
}

- (void) regionDataSelected:(UITableView*)tableView forIndexPath:(NSIndexPath*)indexPath
{
	NSString* currentRegionData = [self.filterSettings objectForKey:ODCurrentRegionData];
	
	//TODO: use NSPredicate instead
	NSEnumerator *enumerator = [self.regionPickerDataSource.mapRegionDataSources objectEnumerator];
	MapDataSource* mapSource = nil;
	NSUInteger index = 0;
	
	while ((mapSource = [enumerator nextObject]) && currentRegionData != nil) {
		if ([mapSource.uuid isEqualToString:currentRegionData]) {
			break;
		}
		++index;
	}
	
	[[NSNotificationCenter defaultCenter] postNotificationName:ODShowRegionPicker object:[NSNumber numberWithInt:index]];
}

- (void) tableView:(UITableView*)tableView didSelectRowAtIndexPath:(NSIndexPath*)indexPath 
{
	switch (indexPath.section) {
//		case REGION_DATA_SECTION:
//			[self regionDataSelected:tableView forIndexPath:indexPath];
//			break;
		case SOCIAL_DATA_SECTION:
			[self socialDataSelected:tableView forIndexPath:indexPath];
			break;
		case POINT_DATA_SECTION:
		default:
			[self pointDataSelected:tableView forIndexPath:indexPath];
			break;
	}
}

- (void)tableView:(UITableView *)tableView accessoryButtonTappedForRowWithIndexPath:(NSIndexPath *)indexPath
{
	switch (indexPath.section) {
//		case REGION_DATA_SECTION:
//			[self regionDataSelected:tableView forIndexPath:indexPath];
//			break;
		case SOCIAL_DATA_SECTION:
			[self socialDataSelected:tableView forIndexPath:indexPath];
			break;
		case POINT_DATA_SECTION:
		default:
			[self pointDataSelected:tableView forIndexPath:indexPath];
			break;
	}
}

#pragma mark -
#pragma mark ImageDownloaderDelegate

- (void) imageDidLoad:(UIImage*)image forTableView:(UITableView*)tableView atIndexPath:(NSIndexPath*)indexPath
{
	MapDataSource* mapSource = nil;
	
	switch (indexPath.section) 
	{
//		case REGION_DATA_SECTION:
//			break;
		case SOCIAL_DATA_SECTION:
			mapSource = [self.mapSocialDataSources objectAtIndex:indexPath.row];
			break;
		case POINT_DATA_SECTION:
		default:
			mapSource = [self.mapPointDataSources objectAtIndex:indexPath.row];
			break;
	}

    if (mapSource != nil)
    {
		mapSource.icon = image;
        UITableViewCell *cell = [tableView cellForRowAtIndexPath:indexPath];
		CGRect rect = cell.imageView.frame;
		rect.size = image.size;
		cell.imageView.frame = rect;
        cell.imageView.image = mapSource.icon;
    }
}

@end
