//
//  MainViewController.m
//  VanGuide
//
//  Created by shazron on 09-11-27.
//  Copyright Nitobi Software Inc 2009. All rights reserved.
//

#import "GoogleMapViewController.h"
#import "GoogleMapViewDelegate.h"
#import "KmlPlacemark.h"
#import "MapDataSourceModel.h"
#import "MapDataSource.h"
#import "AtomParser.h"
#import "MapAnnotation.h"

@implementation GoogleMapViewController


@synthesize mapView;

 - (void)viewDidLoad 
{
	[super viewDidLoad];

	GoogleMapViewDelegate* delg = [[GoogleMapViewDelegate alloc] init];
	self.mapView.delegate = delg;
	[self goToHomeLocation];
	
	// send any pending notifications
	if (self.pendingNotification != nil) {
		[[NSNotificationQueue defaultQueue] enqueueNotification:self.pendingNotification postingStyle: NSPostASAP];
		self.pendingNotification = nil;
	}
}

- (void)didReceiveMemoryWarning {
	// Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
	
	// Release any cached data, images, etc that aren't in use.
}

- (void)viewDidUnload {
	// Release any retained subviews of the main view.
	// e.g. self.myOutlet = nil;
}

- (void)dealloc {
	self.mapView = nil;
	
    [super dealloc];
}

#pragma mark -
#pragma mark MapViewController overrides

- (void) plotMapAnnotation:(NSNotification*)notification
{
	MapAnnotation* annotation = [notification object];
	[self.mapView addAnnotation:annotation.placemark];
}

- (void) plotRegionAnnotation:(NSNotification*)notification
{
	//MapAnnotation* annotation = [notification object];
	// TODO: plot region here
	// See: http://pixelfehler.nicolas-neubauer.de/2009/09/04/mapkit-google-maps-iphone-and-drawing-routes-or-polylines/
	// See: http://spitzkoff.com/craig/?p=108
	//DebugNSLog(@"Is Polygon, skipping for now…(%d points)", [annotation.placemark.coordinatesArray count]);
}

- (NSArray*) allMapAnnotations
{
	return self.mapView.annotations;
}

- (NSString*) categoryTypeForAnnotation:(id)annotation
{
	if ([annotation isKindOfClass:[KmlPlacemark class]]) {
		KmlPlacemark* placeMark = (KmlPlacemark*)annotation;
		return placeMark.sourceUUID;
	}
	return nil;
}

- (void) removeMapAnnotations:(NSArray*)annotations
{
	[self.mapView removeAnnotations:annotations];
}

- (void) goToHomeLocation
{
	[(GoogleMapViewDelegate*)self.mapView.delegate goToInitialLocation:self.mapView];
}

- (void) goToMyLocation
{
	self.mapView.showsUserLocation = YES;
	// TODO: go to user's current location using CLLocation
}

#pragma mark -

@end
