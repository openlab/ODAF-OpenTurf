//
//  KmlPlacemark.m
//  VanGuide
//
//  Created by shazron on 09-12-03.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <CoreLocation/CoreLocation.h>
#import "KmlPlacemark.h"
#import "Utils.h"

@implementation NSMutableArray(NSMutableArray_Extension)

- (id) removeAndGetLastObject
{
	id last = [self lastObject];
	if (last != nil) {
		[self removeLastObject];
	}
	return last;
}

@end

@implementation KmlPlacemark

@synthesize name, coordinates, description, sourceUUID, coordinatesArray, isOGDI, guid;

- (id) init
{
    if (self = [super init]) {
		self.coordinatesArray = [[NSMutableArray alloc] initWithCapacity:3];
		__coordinatesParsed = NO;
		self.isOGDI = YES;
    }
    return self;
}

- (BOOL) isPolygon
{
	return [self.coordinatesArray count] > 1;
}

- (NSString*) guid
{
	return self.isOGDI ? self.description : guid;
}

- (CLLocationCoordinate2D) coordinate
{
	if (!__coordinatesParsed) {
		[self parseCoordinates];
	}
	
	CLLocationCoordinate2D coord; // might want to init with error location
	CLLocation* loc = [self.coordinatesArray count] > 0 ? [self.coordinatesArray objectAtIndex:0] : nil;
	if (loc) {
		coord = loc.coordinate;
	} 
	
	return coord;
}

- (void) resetToCoordinate:(CLLocationCoordinate2D)aCoordinate
{
	__coordinatesParsed = NO;
	[self.coordinatesArray removeAllObjects];
	self.coordinates = [NSString stringWithFormat:@"%f,%f", aCoordinate.longitude, aCoordinate.latitude];
}

- (void) parseCoordinates
{
	if (__coordinatesParsed) {
		return;
	}
	
	CLLocation* coord;
	
	// parse self.coordinates into lat and long, altitude
	NSMutableArray* items = [[[[self.coordinates componentsSeparatedByString:@","] reverseObjectEnumerator] allObjects] mutableCopy];
	NSNumber* lng, *lat, *alt;
	
	while ([items count] > 0) 
	{
		lng = [items removeAndGetLastObject]; 
		lat = [items removeAndGetLastObject];
		alt = [items removeAndGetLastObject];
#pragma unused(alt)
		
		if (lat == nil && lng == nil) {
			break;
		}
		
		coord = [[CLLocation alloc] initWithLatitude:[lat floatValue] longitude:[lng floatValue]];
		[self.coordinatesArray addObject:coord];
		[coord release];
	}
	
	__coordinatesParsed = YES;
	[items release];
}

- (NSString*) title;
{
	return self.name;
}

- (NSString*) subtitle;
{
	return self.description;
}

- (void) dealloc
{
	[self.coordinatesArray removeAllObjects];
	self.coordinatesArray = nil;
	
	[super dealloc];
}

@end
