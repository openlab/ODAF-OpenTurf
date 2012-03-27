//
//  PointDataComment.h
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface PointDataComment : NSObject {
	NSUInteger Id;
	NSString* Text;
	NSDate* CreatedOn;
	NSUInteger CreatedById;
	NSUInteger SummaryId;
}

@property (nonatomic, assign) NSUInteger Id;
@property (nonatomic, copy)	  NSString* Text;
@property (nonatomic, retain) NSDate* CreatedOn;
@property (nonatomic, assign) NSUInteger CreatedById;
@property (nonatomic, assign) NSUInteger SummaryId;

@end


@interface AggregateComment : NSObject {
	PointDataComment* Comment;
	NSString* CommentAuthor;
	NSString* ServiceName;
}

@property (nonatomic, retain)	PointDataComment* Comment;
@property (nonatomic, copy)		NSString* CommentAuthor;
@property (nonatomic, copy)		NSString* ServiceName;
@property (nonatomic, readonly, getter=isValid)	BOOL valid;

- (id) initWithComment:(PointDataComment*)comment;
- (BOOL) isValid;

@end