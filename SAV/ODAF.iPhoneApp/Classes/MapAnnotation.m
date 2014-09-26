//
//  MapAnnotation.m
//  VanGuide
//
//  Created by shazron on 10-02-24.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "MapAnnotation.h"
#import "KmlPlacemark.h"
#import "MapDataSource.h"

@implementation MapAnnotation

@synthesize dataSource, placemark;

- (void) dealloc
{
	self.dataSource = nil;
	self.placemark = nil;
	
	[super dealloc];
}

@end
