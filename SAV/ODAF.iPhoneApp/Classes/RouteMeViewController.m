//
//  RouteMeViewController.m
//  VanGuide
//
//  Created by shazron on 09-12-10.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import "RouteMeViewController.h"
#import "RMVirtualEarthSource.h"
#import "MapDataSource.h"
#import "MapDataSourceModel.h"
#import "KmlPlacemark.h"
#import "FlipsideViewController.h"
#import "RMMarker.h"
#import "RMMapView.h"
#import "RMMarkerManager.h"
#import "AtomParser.h"
#import "RMMarker+Additions.h"
#import "MapDetailsViewController.h"
#import "VanGuideAppDelegate.h"
#import "CalloutViewController.h"
#import "NewMarkerViewController.h"
#import "PointDataSummary.h"
#import "InfoViewController.h"
#import "MapAnnotation.h"

@implementation RouteMeViewController

@synthesize mapView, toolbar, currentMarker, newMarker, callout, currentLocationButton, homeButton;
@synthesize zPosition, currentLocationMarker, locationManager, imageDownloadsInProgress, undoManager, undoMarker;


#pragma mark -

- (void) viewDidLoad 
{
	[super viewDidLoad];
	
	[RMMapView class]; // added so the view can be used in Interface Builder
	
	self.undoManager = [[NSUndoManager alloc] init];
	
	self.currentMarker = self.newMarker = nil;
	self.toolbar.tintColor = [UIColor colorWithHexString:[[Utils sharedInstance] getConfigSetting:kConfigKeyTintColourRGB]];
	
	[self.mapView setDelegate:self];
	self.mapView.contents.tileSource = [[RMVirtualEarthSource alloc] initWithRoadThemeUsingAccessKey:BING_MAPS_DEV_KEY];

	[self goToHomeLocation];
	self.zPosition = 100000;// so that the current marker is on top
	
	// Register for UserSummaryAdded notif
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(userSummaryAdded:) 
												 name:ODRESTUserSummaryAddSuccess object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(addPointForViewComment:) 
												 name:ODAddPointForViewComment object:nil];
	
	// send any pending notifications
	if (self.pendingNotification != nil) {
		[[NSNotificationQueue defaultQueue] enqueueNotification:self.pendingNotification postingStyle: NSPostASAP];
		self.pendingNotification = nil;
	}
}

- (void) didReceiveMemoryWarning 
{
	// Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
	
	// Release any cached data, images, etc that aren't in use.
    NSArray* allDownloads = [self.imageDownloadsInProgress allValues];
    [allDownloads performSelector:@selector(cancel)];
}

- (void) viewDidUnload 
{
	// Release any retained subviews of the main view.
	// e.g. self.myOutlet = nil;
}

- (void) dealloc 
{
	self.mapView = nil;
	self.toolbar = nil;
	self.undoManager = nil;
	
    [super dealloc];
}

- (void) startImageDownload:(NSString*)url forAnnotation:(MapAnnotation*)annotation
{
    ImageDownloader* downloader = [self.imageDownloadsInProgress objectForKey:annotation.placemark.guid];
    if (downloader == nil) 
    {
        downloader = [[ImageDownloader alloc] init];
        downloader.annotation = annotation;
        downloader.delegate = self;
        [self.imageDownloadsInProgress setObject:downloader forKey:annotation.placemark.guid];
        [downloader start:url];
        [downloader release];   
    }
}

- (IBAction) info
{
	InfoViewController* viewController = [[InfoViewController alloc] init];
	viewController.modalTransitionStyle = UIModalTransitionStyleCoverVertical;
	[self presentModalViewController:viewController animated:YES];	
	[viewController release];
}

- (RMMarker*) userSummaryAdded:(NSNotification*)notification
{
	RMMarkerManager *markerManager = [self.mapView markerManager];
	if (self.newMarker != nil) {
		[markerManager removeMarker:self.newMarker];
		self.newMarker = nil;
	}
	
	MapAnnotation* annotation = (MapAnnotation*)notification.object;
	
	UIImage* image = [UIImage imageNamed:kPinAddedPoint];
	RMMarker *marker = [[RMMarker alloc]initWithUIImage:image
											// anchor point is at the pin convergence point.
											anchorPoint:CGPointMake(0.5, 1.0)];
	marker.data = annotation;
	[marker changeLabelUsingText:annotation.placemark.name];
	[marker hideLabel];
	[markerManager addMarker:marker AtLatLong:annotation.placemark.coordinate];
	
	return [marker autorelease];
}

- (void) addPointForViewComment:(NSNotification*)notification
{
	RMMarker* marker = [self userSummaryAdded:notification];
	
	// go to map location
	NSDictionary* mapSettings = [[Utils sharedInstance] getConfigSetting:kConfigKeyMapSettings];
	
	[self.mapView moveToLatLong:[[self.mapView markerManager] latitudeLongitudeForMarker:marker]];
	self.mapView.contents.zoom = [[mapSettings objectForKey:kConfigKeyMapViewCommentZoom] floatValue];
}

- (void) undoRemoveMapPin
{
	self.newMarker = self.undoMarker;
	self.undoMarker = nil;
	
	RMMarkerManager *markerManager = [self.mapView markerManager];
	[markerManager addMarker:self.newMarker AtLatLong:[markerManager latitudeLongitudeForMarker:self.newMarker]];
	
	[[self.undoManager prepareWithInvocationTarget:self] removeNewMapPin];
	[self.undoManager setActionName:NSLocalizedString(kAddNewLandmarkText, kAddNewLandmarkText)];
}

- (void) removeNewMapPin
{
	RMMarkerManager *markerManager = [self.mapView markerManager];
	if (self.newMarker != nil) {
		self.undoMarker = self.newMarker;
		[markerManager removeMarker:self.newMarker];
		[[self.undoManager prepareWithInvocationTarget:self] undoRemoveMapPin];
	}
	
	self.newMarker = nil;
}

- (IBAction) newMapPin
{
	[self.undoManager undo];
	
	UIImage* image = [UIImage imageNamed:kPinNewPoint];
	RMMarker *marker = [[RMMarker alloc]initWithUIImage:image
											// anchor point is at the pin convergence point.
											anchorPoint:CGPointMake(0.5, 1.0)];
	[marker setTextForegroundColor:[UIColor blueColor]];
	
	self.newMarker = marker;
	
	RMMarkerManager *markerManager = [self.mapView markerManager];
	[markerManager addMarker:marker AtLatLong:self.mapView.contents.mapCenter];
	marker.zPosition = self.zPosition++;
	[marker release];
	
	[[self.undoManager prepareWithInvocationTarget:self] removeNewMapPin];
	[self.undoManager setActionName:NSLocalizedString(kAddNewLandmarkText, kAddNewLandmarkText)];
}

#pragma mark -
#pragma mark MapViewController overrides

- (void) plotMapAnnotation:(NSNotification*)notification
{
	MapAnnotation* annotation = [notification object];
	
	NSArray* uuids_to_filter = [MapDataSourceModel sharedInstance].changeSet.deletions;
	if ([uuids_to_filter containsObject:annotation.dataSource.uuid]) { // don't add - delete object
		return;
	}
	
	RMMarkerManager *markerManager = [self.mapView markerManager];
	UIImage* icon = nil;
	
	if (!annotation.dataSource.iconUrl) { // no remote image, so we try to get the local one
		MapDataSourceModel* dataModel = [MapDataSourceModel sharedInstance];
		NSString* icon = [dataModel.iconMapSettings objectForKey:annotation.dataSource.uuid];
		if (icon != nil) {
			annotation.dataSource.icon = [UIImage imageNamed:icon];
		}
	} 
	
	if (!annotation.dataSource.icon) { // no icon set, we lazy load
		NSString* url = [NSString stringWithFormat:@"%@/%@", 
						 [[Utils sharedInstance] webAppUrl],
						 annotation.dataSource.iconUrl];
		[self startImageDownload:url forAnnotation:annotation];
		
		// if a download is deferred or in progress, don't plot the pin essentially
	}
	else
	{
		icon = annotation.dataSource.icon;
	}
	
	if (icon != nil) 
	{
		RMMarker *marker = [[RMMarker alloc]initWithUIImage:icon
												// anchor point is at the pin middle point.
												anchorPoint:CGPointMake(0.5, 0.5)];
		marker.data = annotation;
		[marker changeLabelUsingText:annotation.placemark.name];
		[marker hideLabel];
		[markerManager addMarker:marker AtLatLong:annotation.placemark.coordinate];
		[marker release];
	}
}

- (void) plotRegionAnnotation:(NSNotification*)notification
{
	//MapAnnotation* annotation = [notification object];
	// TODO: plot region here
	//DebugNSLog(@"Is Polygon, skipping for now…(%d points)", [annotation.placemark.coordinatesArray count]);
}

- (NSArray*) allMapAnnotations
{
	return [[self.mapView markerManager] markers];
}

- (NSString*) categoryTypeForAnnotation:(id)annotation
{
	if ([annotation isKindOfClass:[RMMarker class]]) {
		RMMarker* marker = (RMMarker*)annotation;
		return marker.sourceUUID;
	}
	return nil;
}

- (void) removeMapAnnotations:(NSArray*)annotations
{
	[[self.mapView markerManager] removeMarkers:annotations]; 
}

- (void) zoomToCurrentLocation
{
	if (self.currentLocationMarker != nil) 
	{
		NSDictionary* mapSettings = [[Utils sharedInstance] getConfigSetting:kConfigKeyMapSettings];
		
		[self.mapView moveToLatLong:[[self.mapView markerManager] latitudeLongitudeForMarker:self.currentLocationMarker]];
		self.mapView.contents.zoom = [[mapSettings objectForKey:kConfigKeyMapCurrentLocationZoom] floatValue];
		self.currentLocationButton.style = UIBarButtonItemStyleDone;
		
		DebugNSLog(@"Zooming to current location.");
	}
}

- (void) goToHomeLocation
{
	NSDictionary* mapSettings = [[Utils sharedInstance] getConfigSetting:kConfigKeyMapSettings];
	CLLocationCoordinate2D c2d;
	c2d.latitude = [[mapSettings objectForKey:kConfigKeyMapInitialLatitude] floatValue];
	c2d.longitude = [[mapSettings objectForKey:kConfigKeyMapInitialLongitude] floatValue];

	[self.mapView moveToLatLong:c2d];
	self.mapView.contents.zoom = [[mapSettings objectForKey:kConfigKeyMapInitialZoom] floatValue];
	self.homeButton.style = UIBarButtonItemStyleDone;
}

- (void) goToMyLocation
{
	DebugNSLog(@"Goto my location");
	if (self.locationManager == nil) 
	{
		self.locationManager = [[CLLocationManager alloc] init];
		self.locationManager.delegate = self;
		[self.locationManager startUpdatingLocation];
	} 
	else 
	{
		[self zoomToCurrentLocation];
	}
}

- (void) viewComment:(NSNotification*)notification
{
	NSDictionary* options = (NSDictionary*)notification.object;
	ViewData* viewData = [[ViewData alloc] init];
	viewData.SummaryId = [[options objectForKey:kUrlParamSummaryId] intValue];
	viewData.CommentId = [[options objectForKey:kUrlParamCommentId] intValue];
	
	// need to remove this observer, else the actionsheet will pop over if no filters are selected
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODNoFiltersSelected object:nil];
	
	MapDetailsViewController* viewController = [[MapDetailsViewController alloc] init];
	viewController.annotation = [[MapAnnotation alloc] init];
	viewController.viewData = viewData;
	[viewData release];
	
	viewController.title = NSLocalizedString(kDetailsText, kDetailsText);
	[[VanGuideAppDelegate sharedApplicationDelegate] pushViewControllerToStack:viewController];
	[viewController release];
}

#pragma mark -
#pragma mark CLLocationManagerDelegate implementation

- (void)locationManager:(CLLocationManager*)manager didUpdateToLocation:(CLLocation*)newLocation 
		   fromLocation:(CLLocation*)oldLocation
{
	if ([[Utils sharedInstance] locationEquals:oldLocation.coordinate withLocation:newLocation.coordinate]) {
		return;
	}
	
	RMMarkerManager *markerManager = [self.mapView markerManager];
	
	if (self.currentLocationMarker == nil)
	{
		self.currentLocationMarker = [[RMMarker alloc]initWithUIImage:[UIImage imageNamed:kPinCurrentLocation]
												anchorPoint:CGPointMake(0.0, 0.0)];
		
		[markerManager addMarker:self.currentLocationMarker AtLatLong:newLocation.coordinate];
		[self zoomToCurrentLocation];
		__initialCurrentLocation = YES;
	} 
	else 
	{
		[markerManager moveMarker:self.currentLocationMarker AtLatLon:newLocation.coordinate];
		
		NSDictionary* mapSettings = [[Utils sharedInstance] getConfigSetting:kConfigKeyMapSettings];
		CGFloat currentZoom = self.mapView.contents.zoom;

		if (([markerManager isMarkerWithinScreenBounds:self.currentLocationMarker] && 
			 currentZoom >= [[mapSettings objectForKey:kConfigKeyMapCurrentLocationZoom] floatValue]) || __initialCurrentLocation) {  
			[self zoomToCurrentLocation];
		}
	}
}

#pragma mark -
#pragma mark RMMapViewDelegate implementation

- (BOOL)mapView:(RMMapView *)map shouldDragMarker:(RMMarker *)marker withEvent:(UIEvent *)event
{
	//If you do not implement this function, then all drags on markers will be sent to the didDragMarker function.
	//If you always return YES you will get the same result
	//If you always return NO you will never get a call to the didDragMarker function
	
	return (marker == self.newMarker);
}

- (void)mapView:(RMMapView *)map didDragMarker:(RMMarker*)marker withEvent:(UIEvent *)event 
{
	CGPoint position = [[[event allTouches] anyObject] locationInView:mapView];
	
	RMMarkerManager *markerManager = [mapView markerManager];
	
	DebugNSLog(@"New location: X:%lf Y:%lf", [marker projectedLocation].easting, [marker projectedLocation].northing);
	CGRect rect = [marker bounds];
	
	// the New Map Pin has its anchor point as the pin convergence point (0.5, 1.0)
	// we compensate for this when dragging the marker, shift the drag point to the center of the pin
	// x does not have to be compensated because its already in the middle. We shift y up
	[markerManager moveMarker:marker AtXY:CGPointMake(position.x, position.y + (rect.size.height/2))];
}

- (void) singleTapOnMap: (RMMapView*) map At: (CGPoint) point
{
	DebugNSLog(@"Clicked on Map - New location: X:%lf Y:%lf", point.x, point.y);
	
	[self.currentMarker hideLabel];
	self.currentMarker.label = nil;
}

- (void) tapOnMarker: (RMMarker*) marker onMap: (RMMapView*) map
{
	if (marker == self.currentLocationMarker) {
		return;
	}
	
	self.currentMarker.label = nil;
	self.currentMarker = marker;

	if (!self.callout) {
		self.callout = [[CalloutViewController alloc] init];
		self.callout.delegate = self;
	}
	
	MapAnnotation* annotation = (MapAnnotation*)marker.data;

	CGRect frame = self.callout.view.frame;
	CGSize offset = CGSizeMake(-10, -(frame.size.height));
	
	if (marker == self.newMarker) { // further x-offset
		offset.width += 15;
	}
	
	frame.origin = CGPointMake(offset.width, offset.height);
	
	self.callout.view.frame = frame;
	
	marker.label = self.callout.view;
	marker.zPosition = self.zPosition++;
	[marker showLabel];
	
	if (marker == self.newMarker) {
		self.callout.textLabel.text = NSLocalizedString(kAddLandmarkText, kAddLandmarkText);
		self.callout.subtitleLabel.text = NSLocalizedString(kShakeToRemoveText, kShakeToRemoveText);
	} else {
		self.callout.textLabel.text = annotation.placemark.name;
		self.callout.subtitleLabel.text = annotation.dataSource.title;
	}
}

- (void) tapOnLabelForMarker:(RMMarker*) marker onMap:(RMMapView*) map
{
	if (self.newMarker == marker) 
	{
		NewMarkerViewController* viewController = [[NewMarkerViewController alloc] init];
		
		RMMarkerManager *markerManager = [mapView markerManager];
		CLLocationCoordinate2D coordinate = [markerManager latitudeLongitudeForMarker:marker];
		
		viewController.summary  = [[PointDataSummary alloc] init];
		viewController.summary.Latitude = coordinate.latitude;
		viewController.summary.Longitude = coordinate.longitude;
		viewController.summary.Guid = [[Utils sharedInstance] generateUUID];
		viewController.summary.Tag = @"";
		
		[[VanGuideAppDelegate sharedApplicationDelegate] pushViewControllerToStack:viewController];
		[viewController release];
	} 
	else 
	{
		MapDetailsViewController* viewController = [[MapDetailsViewController alloc] init];
		viewController.annotation = (MapAnnotation*)self.currentMarker.data;
		
		viewController.title = NSLocalizedString(kDetailsText, kDetailsText);
		[[VanGuideAppDelegate sharedApplicationDelegate] pushViewControllerToStack:viewController];
		[viewController release];
	}
}

- (void) afterMapMove: (RMMapView*)map
{
	self.homeButton.style = UIBarButtonItemStyleBordered;
	self.currentLocationButton.style = UIBarButtonItemStyleBordered;
	__initialCurrentLocation = NO;
}

- (void) afterMapZoom: (RMMapView*)map byFactor:(float)zoomFactor near:(CGPoint)center
{
	self.homeButton.style = UIBarButtonItemStyleBordered;
	self.currentLocationButton.style = UIBarButtonItemStyleBordered;
	__initialCurrentLocation = NO;
}

#pragma mark -

- (void) calloutDetails:(CalloutViewController*)controller forPlacemark:(KmlPlacemark*)placemark
{
	// TODO: not called. handled in tapOnLabelForMarker:onMap:
}

#pragma mark -
#pragma mark ImageDownloaderDelegate

- (void) imageDidLoad:(UIImage*)image forAnnotation:(MapAnnotation*)annotation
{
	annotation.dataSource.icon = image;
	
	NSNotification* notification = [NSNotification notificationWithName:@"" object:annotation];
	[self plotMapAnnotation:notification];
}

@end
