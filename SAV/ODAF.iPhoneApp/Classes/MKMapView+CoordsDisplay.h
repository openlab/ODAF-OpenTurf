//
//  MKMapView+CoordsDisplay.h
//  GoogleLocalSearchMap
//
//  Created by shazron on 09-11-12.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <MapKit/MapKit.h>

@interface MKMapView(CoordsDisplay)

- (BOOL)coordinatesInRegion:(CLLocationCoordinate2D)coords;

@end