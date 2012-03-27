//
//  ODHome.m
//  VanGuide
//
//  Created by shazron on 10-02-04.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "ODHome.h"
#import "RESTInfo.h"

static ODHome* sharedInstance = nil;

@implementation ODHome

- (id) init
{
	if (self = [super init]) {
		self.appController = @"Home";
	}
	
	return self;
}

- (RESTInfo*) Credits
{
	NSString* url = [self buildUrl:@"Credits"];
	NSDictionary* dict = nil; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) GetLinkForSummary:(NSUInteger)summaryId
{
	NSString* url = [self buildUrl:@"GetLinkForSummary" withId:summaryId];
	NSDictionary* dict = nil; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];	
}

- (RESTInfo*) GetLinkForComment:(NSUInteger)commentId
{
	NSString* url = [self buildUrl:@"GetLinkForComment" withId:commentId];
	NSDictionary* dict = nil; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];	
}

#pragma mark -
#pragma mark Singleton methods

+ (ODHome*) sharedInstance
{
	@synchronized(self)
	{
		if (sharedInstance == nil)
			sharedInstance = [[ODHome alloc] init];
	}
	return sharedInstance;
}

+ (id)allocWithZone:(NSZone *)zone 
{
	@synchronized(self) {
		if (sharedInstance == nil) {
			sharedInstance = [super allocWithZone:zone];
			return sharedInstance;  // assignment and return on first allocation
		}
	}
	return nil; // on subsequent allocation attempts return nil
}

@end
