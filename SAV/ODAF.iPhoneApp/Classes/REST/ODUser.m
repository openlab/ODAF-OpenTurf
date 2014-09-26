//
//  ODUser.m
//  VanGuide
//
//  Created by shazron on 10-02-04.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "ODUser.h"
#import "RESTInfo.h"

static ODUser* sharedInstance = nil;

@implementation ODUser

- (id) init
{
	if (self = [super init]) {
		self.appController = @"User";
	}
	
	return self;
}

- (RESTInfo*) RequestAuthToken
{
	NSString* url = [self buildUrl:@"RequestAuthToken"];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:
						  self.appId, @"appId", 
						  nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) GetAccessToken:(NSString*)oauth_token
{
	NSString* url = [self buildUrl:@"GetAccessToken"];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:
						  self.appId, @"appId",
						  oauth_token, @"oauth_token",
						  nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) Authenticate:(NSString*)oauth_token oauthTokenSecret:(NSString*)oauth_token_secret
{
	NSString* url = [self buildUrl:@"Authenticate"];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:
						  self.appId, @"appId",
						  oauth_token, @"oauth_token",
						  oauth_token_secret, @"oauth_token_secret",
						  nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) Current
{
	NSString* url = [self buildUrl:@"Current"];
	NSDictionary* dict = nil; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}

- (RESTInfo*) Image:(NSUInteger)userId
{
	NSString* url = [self buildUrl:@"Image" withId:userId];
	NSDictionary* dict = nil; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbGET withParameters:dict] autorelease];
}


- (RESTInfo*) UpdateTwitterStatus:(NSString*)status withLat:(CGFloat)lat andLong:(CGFloat)lng
{
	NSString* url = [self buildUrl:@"UpdateTwitterStatus"];
	NSDictionary* dict = [NSDictionary dictionaryWithObjectsAndKeys:
						  status, @"status",
						  [NSString stringWithFormat:@"%f", lat], @"lat", 
						  [NSString stringWithFormat:@"%f", lng], @"lng", 
						  nil]; 
	
	return [[[RESTInfo alloc] init:[NSURL URLWithString:url] usingVerb:kHttpVerbPOST withParameters:dict] autorelease];
}

#pragma mark -
#pragma mark Singleton methods

+ (ODUser*) sharedInstance
{
	@synchronized(self)
	{
		if (sharedInstance == nil)
			sharedInstance = [[ODUser alloc] init];
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
