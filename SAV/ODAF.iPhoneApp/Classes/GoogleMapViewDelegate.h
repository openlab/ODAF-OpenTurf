//
//  MainMapViewDelegate.h
//  VanGuide
//
//  Created by shazron on 09-11-30.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <MapKit/MapKit.h>
#import "GoogleMapInfoDisclosureDelegate.h"

@interface GoogleMapViewDelegate : NSObject< MKMapViewDelegate > {

	NSThread* searchThread;
	id<MainMapInfoDisclosureDelegate> disclosureDelegate;
}

@property (nonatomic, retain) NSThread* searchThread;
@property (nonatomic, retain) id<MainMapInfoDisclosureDelegate> disclosureDelegate;

- (void) goToInitialLocation:(MKMapView*)theMapView;
- (MKCoordinateRegion) initialLocationRegion:(MKMapView*)theMapView;

@end
