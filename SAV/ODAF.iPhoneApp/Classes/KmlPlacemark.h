//
//  KmlPlacemark.h
//  VanGuide
//
// The Placemark xml data is flattened.
//
//  Created by shazron on 09-12-03.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <MapKit/MapKit.h>


@interface KmlPlacemark : NSObject < MKAnnotation > {
	NSString* sourceUUID;
	NSString* coordinates;
	NSString* name;
	NSString* description;
	BOOL __coordinatesParsed;
	BOOL isOGDI;
	NSString* guid;
	
	NSMutableArray* coordinatesArray;
}

@property (nonatomic, copy) NSString* sourceUUID;
@property (nonatomic, retain) NSMutableArray* coordinatesArray;
@property (nonatomic, readonly) BOOL isPolygon;
@property (nonatomic, assign) BOOL isOGDI;

- (void) parseCoordinates;
- (void) resetToCoordinate:(CLLocationCoordinate2D)aCoordinate;

// Items below are for the parsing from XML
@property (nonatomic, copy) NSString* name;
@property (nonatomic, copy) NSString* coordinates;
@property (nonatomic, copy) NSString* description;
@property (nonatomic, copy) NSString* guid; // OGDI puts this value in the description

// Items below are for the MKAnnotation protocol
@property (nonatomic, readonly) CLLocationCoordinate2D coordinate;

- (NSString*) title;
- (NSString*) subtitle;

@end

@interface NSMutableArray(NSMutableArray_Extension)

- (id) removeAndGetLastObject;

@end
