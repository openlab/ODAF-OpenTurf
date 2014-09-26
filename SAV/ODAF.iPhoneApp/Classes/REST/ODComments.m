//
//  ODComments.m
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import "ODComments.h"
#import "RESTInfo.h"

static ODComments* sharedInstance = nil;

@implementation ODComments

- (id) init
{
	if (self = [super init]) {
		self.appController = @"Comments";
	}
	
	return self;
}

- (RESTInfo*) List:(NSUInteger)summaryId  page:(NSUInteger)page page_size:(NSUInteger)page_size
{
	NSString* url = [self buildUrl:@"List" withId:summaryId];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:
						  [NSString stringWithFormat:@"%ld", page], @"page", 
						  [NSString stringWithFormat:@"%ld", page_size], @"page_size", nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) List:(NSUInteger)summaryId
{
	NSString* url = [self buildUrl:@"List" withId:summaryId];
	NSDictionary* dict = nil;
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) Show:(NSUInteger)commentId
{
	NSString* url = [self buildUrl:@"Show" withId:commentId];
	NSDictionary* dict = nil;
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) Add:(NSUInteger)summaryId text:(NSString*) text
{
	NSString* url = [self buildUrl:@"Add" withId:summaryId];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:text, @"Text", nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbPOST withParameters:dict] autorelease];
}

- (RESTInfo*) Edit:(NSUInteger)commentId text:(NSString*) text
{
	NSString* url = [self buildUrl:@"Edit" withId:commentId];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:text, @"Text", nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbPOST withParameters:dict] autorelease];
}

- (RESTInfo*) Remove:(NSUInteger) commentId
{
	NSString* url = [self buildUrl:@"Remove" withId:commentId];
	NSDictionary* dict = nil; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbPOST withParameters:dict] autorelease];
}
	
#pragma mark -
#pragma mark Singleton methods
	
+ (ODComments*) sharedInstance
{
	@synchronized(self)
	{
		if (sharedInstance == nil)
			sharedInstance = [[ODComments alloc] init];
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
