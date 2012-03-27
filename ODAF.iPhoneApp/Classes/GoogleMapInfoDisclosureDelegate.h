//
//  MainMapInfoDisclosureDelegate.h
//  VanGuide
//
//  Created by shazron on 09-11-30.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <MapKit/MapKit.h>

@protocol MainMapInfoDisclosureDelegate  

- (void)mapInfoDisclosure:(MKMapView*) mapView annotationView:(MKAnnotationView *)view 
calloutAccessoryControlTapped:(UIControl *)control;


@end
