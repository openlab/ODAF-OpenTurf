//
//  RESTInfo.m
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import "RESTInfo.h"


@implementation RESTInfo

@synthesize url;
@synthesize verb;
@synthesize parameters, delegate, tag;

- (id) init:(NSURL *)aUrl usingVerb:(NSString *)aVerb withParameters:(NSDictionary *)aParameters
{
	if (self = [super init]) 
	{
		self.url = aUrl;
		self.verb = aVerb;
		self.parameters = aParameters;
		self.delegate = nil;
		self.tag = nil;
	}
	
	return self;
}

- (void) dealloc
{
	self.url = nil;
	self.verb = nil;
	self.parameters = nil;
	self.delegate = nil;
	self.tag = nil;
	
	[super dealloc];
}


@end
