//
//  OAuthAccount.m
//  VanGuide
//
//  Created by shazron on 10-03-11.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "OAuthAccount.h"


@implementation OAuthAccount

@synthesize Id, UserAccess, UserRole, oauth_service_id, oauth_token, oauth_token_secret;
@synthesize profile_image_url, screen_name, user_id, LastAccessedOn, TokenExpiry;


- (void) setLastAccessedOn:(id)value
{
	if ([value isKindOfClass:[NSDate class]]) 
	{
		LastAccessedOn = [value retain]; 
	} 
	else if ([value isKindOfClass:[NSString class]]) 
	{
		NSString* str = (NSString*)value;
		NSDate* date = [[Utils sharedInstance] dateFromJsDate:str];
		if (date != nil) {
			LastAccessedOn = [date retain];
		}
	}
}

- (void) setTokenExpiry:(id)value
{
	if ([value isKindOfClass:[NSDate class]]) 
	{
		TokenExpiry = [value retain]; 
	} 
	else if ([value isKindOfClass:[NSString class]]) 
	{
		NSString* str = (NSString*)value;
		NSDate* date = [[Utils sharedInstance] dateFromJsDate:str];
		if (date != nil) {
			TokenExpiry = [date retain];
		}
	}
}

- (void) dealloc
{
	self.LastAccessedOn = nil;
	self.TokenExpiry = nil;
	
	[super dealloc];
}


@end
