//
//  RouteMeViewController.h
//  VanGuide
//
//  Created by shazron on 09-12-10.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "FlipsideViewController.h"
#import "RMMapViewDelegate.h"
#import "MapViewController.h"
#import "CalloutViewDisclosureDelegate.h"
#import "ImageDownloader.h"

@class RMMapView;
@class RMMarker;
@class CalloutViewController;

@interface RouteMeViewController : MapViewController < ImageDownloaderDelegate, RMMapViewDelegate, CLLocationManagerDelegate, CalloutViewDisclosureDelegate > {
	IBOutlet RMMapView* mapView;
	IBOutlet UIToolbar* toolbar;
	IBOutlet UIBarButtonItem* currentLocationButton;
	IBOutlet UIBarButtonItem* homeButton;
	
@private
	RMMarker* currentMarker;
	RMMarker* newMarker;
	RMMarker* undoMarker;

	RMMarker* currentLocationMarker;
	CalloutViewController* callout;
	CGFloat zPosition;
	CLLocationManager* locationManager;
	NSMutableDictionary *imageDownloadsInProgress;
	NSUndoManager *undoManager;
	
	BOOL __initialCurrentLocation;
}

- (void) undoRemoveMapPin;
- (void) removeNewMapPin;
- (IBAction) newMapPin;
- (IBAction) info;

@property (nonatomic, retain) RMMapView* mapView;
@property (nonatomic, retain) UIToolbar* toolbar;
@property (nonatomic, retain) UIBarButtonItem* currentLocationButton;
@property (nonatomic, retain) UIBarButtonItem* homeButton;
@property (nonatomic, retain) NSUndoManager *undoManager;

@property (retain) RMMarker* currentMarker;
@property (retain) RMMarker* newMarker;
@property (retain) RMMarker* undoMarker;
@property (retain) RMMarker* currentLocationMarker;

@property (retain) CalloutViewController* callout;
@property (nonatomic, assign) CGFloat zPosition;
@property (nonatomic, retain) CLLocationManager* locationManager;
@property (nonatomic, retain) NSMutableDictionary* imageDownloadsInProgress;

@end
