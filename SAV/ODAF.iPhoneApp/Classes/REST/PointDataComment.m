//
//  PointDataComment.m
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import "PointDataComment.h"


@implementation PointDataComment

@synthesize Id, CreatedOn, Text, CreatedById, SummaryId;

- (void) setCreatedOn:(id)value
{
	if ([value isKindOfClass:[NSDate class]]) 
	{
		CreatedOn = [value retain]; 
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

- (void) dealloc
{
	self.CreatedOn = nil;
	
	[super dealloc];
}

@end


@implementation AggregateComment

@synthesize Comment, CommentAuthor, ServiceName;

- (id) init
{
	if ((self = [super init]) != nil)
	{
		Comment = nil;
	}
	return self;
}

- (void) setComment:(id)object
{
	[Comment release];
	if ([object isKindOfClass:[NSDictionary class]]) {
		Comment = [[PointDataComment alloc] init];
		[Comment setValuesForKeysWithDictionary:(NSDictionary*)object];
	} else {
		Comment = [object retain];
	}
}

- (id) initWithComment:(PointDataComment*)comment;
{
	if ((self = [super init])!= nil) {
		Comment = [comment retain];
	}
	return self;
}

- (BOOL) isValid
{
	if (self.Comment != nil) {
		return self.Comment.CreatedById != 0;
	} else {
		return NO;
	}
}

- (void) dealloc
{
	[Comment release];
	[super dealloc];
}

@end