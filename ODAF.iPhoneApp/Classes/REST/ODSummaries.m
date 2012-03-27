//
//  ODSummaries.m
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import "ODSummaries.h"
#import "RESTInfo.h"
#import "PointDataSummary.h"

static ODSummaries* sharedInstance = nil;

@implementation ODSummaries

- (id) init
{
	if (self = [super init]) {
		self.appController = @"Summaries";
	}
	
	return self;
}

- (RESTInfo*) List:(NSUInteger)page page_size:(NSUInteger)page_size
{
	NSString* url = [self buildUrl:@"List"];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:
						  [NSString stringWithFormat:@"%ld", page], @"page", 
						  [NSString stringWithFormat:@"%ld", page_size], @"page_size", nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) ListByRegion:(CLLocationDegrees)lat longitude:(CLLocationDegrees)lng layerId:(NSString*)layerId
					 latDelta:(CLLocationDegrees)latDelta longDelta:(CLLocationDegrees)lngDelta page:(NSInteger)page page_size:(NSInteger)page_size
{
	NSString* url = [self buildUrl:@"ListByRegion"];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:
						  [NSString stringWithFormat:@"%f", lat], @"lat", 
						  [NSString stringWithFormat:@"%f", lng], @"lng", 
						  [NSString stringWithFormat:@"%f", latDelta], @"latDelta", 
						  [NSString stringWithFormat:@"%f", lngDelta], @"lngDelta", 
						  layerId, @"layerId", 
						  [NSString stringWithFormat:@"%ld", page], @"page", 
						  [NSString stringWithFormat:@"%ld", page_size], @"page_size", 
						  nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) Show:(CLLocationDegrees)lat longitude:(CLLocationDegrees)lng layerId:(NSString*)layerId
{
	NSString* url = [self buildUrl:@"Show"];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:
						  [NSString stringWithFormat:@"%f", lat], @"lat", 
						  [NSString stringWithFormat:@"%f", lng], @"lng", 
						  layerId, @"layerId", 
						  nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) ShowById:(NSUInteger)summaryId
{
	NSString* url = [self buildUrl:@"ShowById" withId:summaryId];
	NSDictionary* dict = nil; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) ShowByUserId:(NSUInteger)userId
{
	NSString* url = [self buildUrl:@"ShowByUserId" withId:userId];
	NSDictionary* dict = nil; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) ShowByGuid:(NSString*)guid
{
	NSString* url = [self buildUrl:@"ShowByGuid"];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:
						  guid, @"guid", 
						  nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) Add:(PointDataSummary*)summary
{
	NSString* url = [self buildUrl:@"Add"];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:
						  summary.Name, @"Name", 
						  summary.Description, @"Description", 
						  summary.LayerId, @"LayerId", 
						  [NSString stringWithFormat:@"%f", summary.Latitude], @"Latitude", 
						  [NSString stringWithFormat:@"%f", summary.Longitude], @"Longitude", 
						  summary.Tag, @"Tag", 
						  summary.Guid, @"Guid", 
						  nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbPOST withParameters:dict] autorelease];
}

- (RESTInfo*) AddTag:(NSString*)tag forSummary:(NSUInteger)summaryId
{
	NSString* url = [self buildUrl:@"AddTag" withId:summaryId];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:
						  tag, @"Tag",
						  nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbPOST withParameters:dict] autorelease];
}

- (RESTInfo*) AddRating:(int)rating forSummary:(NSUInteger)summaryId
{
	NSString* url = [self buildUrl:@"AddRating" withId:summaryId];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:
						  [NSString stringWithFormat:@"%d", rating], @"rating",
						  nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbPOST withParameters:dict] autorelease];
}

- (RESTInfo*) Edit:(NSDictionary*)edits
{
	NSString* url = [self buildUrl:@"Edit"];
	NSDictionary* dict = edits; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbPOST withParameters:dict] autorelease];
}

- (RESTInfo*) Remove:(NSUInteger)summaryId
{
	NSString* url = [self buildUrl:@"Remove" withId:summaryId];
	NSDictionary* dict = nil; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbPOST withParameters:dict] autorelease];
}

#pragma mark -
#pragma mark Singleton methods

+ (ODSummaries*) sharedInstance
{
	@synchronized(self)
	{
		if (sharedInstance == nil)
			sharedInstance = [[ODSummaries alloc] init];
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
