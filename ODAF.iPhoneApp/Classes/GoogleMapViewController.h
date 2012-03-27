//
//  MainViewController.h
//  VanGuide
//
//  Created by shazron on 09-11-27.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <MapKit/MapKit.h>
#import "MapViewController.h"

@class KmlParser;

@interface GoogleMapViewController : MapViewController {
	IBOutlet MKMapView* mapView;
}

@property (nonatomic, retain) MKMapView* mapView;

@end
