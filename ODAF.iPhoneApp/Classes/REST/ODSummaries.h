//
//  ODSummaries.h
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h> 
#import "RESTBase.h"

@class RESTInfo;
@class PointDataSummary;

@interface ODSummaries : RESTBase  {
	
}

- (RESTInfo*) List:(NSUInteger)page page_size:(NSUInteger)page_size;
- (RESTInfo*) ListByRegion:(CLLocationDegrees)lat longitude:(CLLocationDegrees)lng layerId:(NSString*)layerId
					 latDelta:(CLLocationDegrees)latDelta longDelta:(CLLocationDegrees)lngDelta page:(NSInteger)page page_size:(NSInteger)page_size;

- (RESTInfo*) Show:(CLLocationDegrees)lat longitude:(CLLocationDegrees)lng layerId:(NSString*)layerId;
- (RESTInfo*) ShowById:(NSUInteger)summaryId;
- (RESTInfo*) ShowByUserId:(NSUInteger)userId;
- (RESTInfo*) ShowByGuid:(NSString*)guid;

- (RESTInfo*) Add:(PointDataSummary*)summary;
- (RESTInfo*) Edit:(NSDictionary*)edits;
- (RESTInfo*) Remove:(NSUInteger)summaryId;

- (RESTInfo*) AddTag:(NSString*)tag forSummary:(NSUInteger)summaryId;
- (RESTInfo*) AddRating:(int)rating forSummary:(NSUInteger)summaryId;


+ (ODSummaries*) sharedInstance;

@end
