//
//  RESTBase.m
//  VanGuide
//
//  Created by shazron on 10-02-04.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "RESTBase.h"
#import "RESTInfo.h"
#import "RESTWrapper.h"

@implementation RESTBase

@synthesize appController, appUrl, appId, webservice;

- (id) init
{
	if (self = [super init]) {
		NSDictionary* config = [[Utils sharedInstance] getBundlePlist:kPlistConfiguration]; 

		NSNumber* devMode = [config objectForKey:kConfigKeyDevMode];
		if ([devMode boolValue]) {
			config = [config objectForKey:kConfigKeyDevSettings];
		}
		
		self.appUrl =  [config objectForKey:kConfigKeyWebAppUrl];
		self.appId =  [config objectForKey:kConfigKeyClientAppId];
		self.webservice = [[RESTWrapper alloc] init];
	}
	
	return self;
}

- (NSString*) buildUrl:(NSString*)appAction
{
	return [NSString stringWithFormat:@"%@/%@/%@.%@/", self.appUrl, self.appController, appAction, kFormatJson];
}

- (NSString*) buildUrl:(NSString*)appAction withId:(NSUInteger)myId
{
	return [NSString stringWithFormat:@"%@%ld", [self buildUrl:appAction], myId];
}

#pragma mark -
#pragma mark Singleton methods

- (id)copyWithZone:(NSZone *)zone
{
	return self;
}

- (id)retain {
	return self;
}

- (unsigned)retainCount {
	return UINT_MAX;  // denotes an object that cannot be released
}

- (void)release {
	//do nothing
}

- (id)autorelease {
	return self;
}


@end
