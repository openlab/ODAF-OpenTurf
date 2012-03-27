//
//  PointDataSummary.m
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import "PointDataSummary.h"
#import "KmlPlacemark.h"

@implementation PointDataSummary

@synthesize Id, Name, Description, LayerId, Latitude, Longitude, Tag, Guid, CreatedById, CreatedOn, ModifiedOn;
@synthesize CommentCount, RatingCount, RatingTotal;


- (void) setCreatedOn:(id)value
{
	if ([value isKindOfClass:[NSDate class]]) 
	{
		CreatedOn = [value copy]; 
	} 
	else if ([value isKindOfClass:[NSString class]]) 
	{
		NSString* str = (NSString*)value;
		NSDate* date = [[Utils sharedInstance] dateFromJsDate:str];
		if (date != nil) {
			CreatedOn = [date retain];
		}
	}
}

- (void) setModifiedOn:(id)value
{
	if ([value isKindOfClass:[NSDate class]]) 
	{
		ModifiedOn = [value retain]; 
	} 
	else if ([value isKindOfClass:[NSString class]]) 
	{
		NSString* str = (NSString*)value;
		NSDate* date = [[Utils sharedInstance] dateFromJsDate:str];
		if (date != nil) {
			ModifiedOn = [date retain];
		}
	}
}

- (BOOL) isCreated
{
	return (self.Id > 0);
}

- (CGFloat) calculatedRatingFromMin:(CGFloat)min andMax:(CGFloat)max
{
	CGFloat percentage = ((CGFloat)self.RatingTotal / (CGFloat)self.RatingCount) / 100.0f;
	CGFloat range = (max - min) + 1;
	return  (percentage * range); // value between 1 and 5
}

+ (int) calculateRatingFromMin:(CGFloat)min andMax:(CGFloat)max withValue:(CGFloat)value
{
	CGFloat percentage = MAX( min/max, 
							  MIN(value, max)/max 
							 ); 
	int result = percentage * 100;
	return result;
}

+ (PointDataSummary*) newFromPlacemark:(KmlPlacemark*)placemark
{
	PointDataSummary* newSummary = [[PointDataSummary alloc] init];
	newSummary.Id = 0;
	newSummary.Name = placemark.name;
	newSummary.Description = placemark.description;
	newSummary.LayerId = placemark.sourceUUID;
	newSummary.Latitude = placemark.coordinate.latitude;
	newSummary.Longitude = placemark.coordinate.longitude;
	newSummary.Tag = @"";
	newSummary.Guid = placemark.guid;
	
	return newSummary;
}

- (KmlPlacemark*) convertToPlacemark
{
	KmlPlacemark* placemark = [[KmlPlacemark alloc] init];
	placemark.isOGDI = NO;
	placemark.name = self.Name;
	placemark.description = self.Description;
	placemark.sourceUUID = self.LayerId;
	placemark.guid = self.Guid;
	
	CLLocationCoordinate2D coordinate;
	coordinate.latitude = self.Latitude;
	coordinate.longitude = self.Longitude;
	[placemark resetToCoordinate:coordinate];
		
	return [placemark autorelease];
}

@end
