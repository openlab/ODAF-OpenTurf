//
//  MKMapView+CoordsDisplay.m
//  GoogleLocalSearchMap
//
//  Created by shazron on 09-11-12.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//
#import "MKMapView+CoordsDisplay.h"

@implementation MKMapView(CoordsDisplay)
- (BOOL)coordinatesInRegion:(CLLocationCoordinate2D)coords 
{
    CLLocationDegrees leftDegrees = self.region.center.longitude - (self.region.span.longitudeDelta / 2.0);
    CLLocationDegrees rightDegrees = self.region.center.longitude + (self.region.span.longitudeDelta / 2.0);
    CLLocationDegrees bottomDegrees = self.region.center.latitude - (self.region.span.latitudeDelta / 2.0);
    CLLocationDegrees topDegrees = self.region.center.latitude + (self.region.span.latitudeDelta / 2.0);
    
    return leftDegrees <= coords.longitude && coords.longitude <= rightDegrees && bottomDegrees <= coords.latitude && coords.latitude <= topDegrees;
}
@end