//
//  PointDataSummary.h
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@class KmlPlacemark;

@interface PointDataSummary : NSObject {
	NSUInteger Id;
	NSString* Description;
	NSString* LayerId;
	NSString* Name;
	CLLocationDegrees Latitude;
	CLLocationDegrees Longitude;
	NSString* Tag;
	NSString* Guid;
	NSUInteger CreatedById;
	NSDate* CreatedOn;
	NSDate* ModifiedOn;
	NSUInteger CommentCount;
	NSUInteger RatingCount;
	NSUInteger RatingTotal;
}


@property (nonatomic, assign)	NSUInteger Id;
@property (nonatomic, copy)		NSString* Description;
@property (nonatomic, copy)		NSString* Name;
@property (nonatomic, copy)		NSString* LayerId;
@property (nonatomic, assign)	CLLocationDegrees Latitude;
@property (nonatomic, assign)	CLLocationDegrees Longitude;
@property (nonatomic, copy)		NSString* Tag;
@property (nonatomic, copy)		NSString* Guid;
@property (nonatomic, assign)	NSUInteger CreatedById;
@property (nonatomic, retain)	NSDate* CreatedOn;
@property (nonatomic, retain)	NSDate* ModifiedOn;
@property (nonatomic, assign)	NSUInteger CommentCount;
@property (nonatomic, assign)	NSUInteger RatingCount;
@property (nonatomic, assign)	NSUInteger RatingTotal;

@property (nonatomic, readonly)	BOOL isCreated;

- (CGFloat) calculatedRatingFromMin:(CGFloat)min andMax:(CGFloat)max;
- (KmlPlacemark*) convertToPlacemark;

+ (int) calculateRatingFromMin:(CGFloat)min andMax:(CGFloat)max withValue:(CGFloat)value;
+ (PointDataSummary*) newFromPlacemark:(KmlPlacemark*)placemark;


@end
