//
//  MainMapViewDelegate.m
//  VanGuide
//
//  Created by shazron on 09-11-30.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import "GoogleMapViewDelegate.h"
#import "MKMapView+CoordsDisplay.h"
#import "KmlPlacemark.h"
#import "Utils.h"
#import "MapDataSourceModel.h"

@implementation GoogleMapViewDelegate

@synthesize searchThread, disclosureDelegate;


- (id) init
{
	if (self = [super init]) {
	}
	
	return self;
}

- (void) goToInitialLocation:(MKMapView*)theMapView
{
	[theMapView setRegion:[self initialLocationRegion:theMapView] animated:YES];
}

- (MKCoordinateRegion) initialLocationRegion:(MKMapView*)theMapView
{
	NSDictionary* mapSettings = [[Utils sharedInstance] getConfigSetting:kConfigKeyMapSettings];
	CLLocationCoordinate2D c2d;
	c2d.latitude = [[mapSettings objectForKey:kConfigKeyMapInitialLatitude] floatValue];
	c2d.longitude = [[mapSettings objectForKey:kConfigKeyMapInitialLongitude] floatValue];
	
	return MKCoordinateRegionMakeWithDistance(c2d, kMapInitialLatitudeSpanMetres, kMapInitialLongitudeSpanMetres);
}

- (MKCoordinateRegion) clusterRegion:(MKMapView*)theMapView
{	
	CLLocationDistance distance = 500000;
	return MKCoordinateRegionMakeWithDistance(theMapView.centerCoordinate, distance, distance);
}

- (void) cullAnnotations:(MKMapView*)theMapView
{
	NSEnumerator* enumerator = theMapView.annotations.objectEnumerator;
	id<MKAnnotation> annotation = nil;
	NSMutableArray* toRemove = [NSMutableArray arrayWithCapacity:5];
	NSThread* currentThread = [NSThread currentThread];
	
	while (annotation = [enumerator nextObject]) {
		
		if ([currentThread isCancelled]) { break; }
		
		if ([theMapView viewForAnnotation:annotation] == nil && annotation != theMapView.userLocation) {
			//if (![theMapView coordinatesInRegion:annotation.coordinate]) {
			// annotation not visible in region anymore, so remove it
			[toRemove addObject:annotation];
			continue;
		}
	}
	
	[theMapView removeAnnotations:toRemove];
	[toRemove removeAllObjects];
}

- (void) clusterAnnotations:(MKMapView*)theMapView
{
	MKCoordinateRegion clusterRegion = [self clusterRegion:theMapView];
	if (theMapView.region.span.latitudeDelta < clusterRegion.span.latitudeDelta ||
		theMapView.region.span.longitudeDelta < clusterRegion.span.longitudeDelta) 
	{
		return;
	}
	
	NSThread* currentThread = [NSThread currentThread];
	NSMutableArray* toRemove = [NSMutableArray arrayWithCapacity:5];
	NSArray* existingAnnotations = [theMapView.annotations copy];
	NSUInteger count = [existingAnnotations count];
	id<MKAnnotation> left = nil, right = nil;
	MKAnnotationView* leftView, *rightView;
	CGRect leftRect, rightRect;
	
	for (NSUInteger i=0; i < count; i++) 
	{
		left = [existingAnnotations objectAtIndex:i];
		
		for (NSUInteger j=i+1; j < count; j++) 
		{
			if ([currentThread isCancelled]) { break; }
			
			right = [existingAnnotations objectAtIndex:j];
			
			leftView = [theMapView viewForAnnotation:left];
			leftRect = [leftView convertRect:leftView.bounds toView:theMapView];
			
			rightView = [theMapView viewForAnnotation:right];
			rightRect = [rightView convertRect:rightView.bounds toView:theMapView];
			
			if (CGRectIntersectsRect(leftRect, rightRect)) {
				[toRemove addObject:left];
				continue;
			}
		}
		
		if ([currentThread isCancelled]) { break; }
	}
	
	[theMapView removeAnnotations:toRemove];
	[existingAnnotations release];
	[toRemove removeAllObjects];
}

- (void) addAnnotations:(NSArray*)annotations toMap:(MKMapView*)theMapView 
{
	NSEnumerator* enumerator = annotations.objectEnumerator;
	id<MKAnnotation> annotation = nil;
	
	while (annotation = [enumerator nextObject]) 
	{
		if ([[NSThread currentThread] isCancelled]) { break; }
		
		if ([theMapView coordinatesInRegion:annotation.coordinate]) {
			if (![theMapView.annotations containsObject:annotation]) {
				// annotation not in region, so add it
				[theMapView addAnnotation:annotation];
			} else {
				// annotation already in region, skip it
			}
		}
	}
}

- (void)mapView:(MKMapView*)theMapView regionDidChangeAnimated:(BOOL)animated
{
	[[NSNotificationCenter defaultCenter] postNotificationName:ODMapViewRegionChanged object:self];
}

- (MKAnnotationView *)mapView:(MKMapView*)theMapView viewForAnnotation:(id <MKAnnotation>)annotation
{
	// if its the user location, return the default view (blue dot)
	if (annotation == theMapView.userLocation) {
		return nil;
	}
	
	KmlPlacemark* placeMark = (KmlPlacemark*)annotation;
	
	// re-use views if possible
	MKPinAnnotationView* view = (MKPinAnnotationView*)[theMapView dequeueReusableAnnotationViewWithIdentifier:placeMark.sourceUUID];
	if (view != nil) {
		return view;
	}
	
	NSString* icon = [[MapDataSourceModel sharedInstance].iconMapSettings objectForKey:placeMark.sourceUUID];
	
	if (icon == nil) {
		// no view to re-use, create new
		view = [[MKPinAnnotationView alloc] initWithAnnotation:annotation reuseIdentifier:placeMark.name];
		view.animatesDrop = YES;
		view.canShowCallout = YES;
		view.rightCalloutAccessoryView = [UIButton buttonWithType:UIButtonTypeDetailDisclosure];
	}
	else {
		// no view to re-use, create new
		view = [[MKAnnotationView alloc] initWithAnnotation:annotation reuseIdentifier:placeMark.name];
		view.image = [UIImage imageNamed:icon];
		view.canShowCallout = YES;
		view.rightCalloutAccessoryView = [UIButton buttonWithType:UIButtonTypeDetailDisclosure];
	}

	return [view autorelease];
}

- (void)mapView:(MKMapView*)theMapView annotationView:(MKAnnotationView *)view 
	calloutAccessoryControlTapped:(UIControl *)control
{
	if (self.disclosureDelegate != nil) {
		[self.disclosureDelegate mapInfoDisclosure:theMapView annotationView:view calloutAccessoryControlTapped:control];
	}
}


@end
