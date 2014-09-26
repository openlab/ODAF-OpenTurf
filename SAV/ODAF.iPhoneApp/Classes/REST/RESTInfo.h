//
//  RESTInfo.h
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "RESTWrapperDelegate.h"

#define kHttpVerbGET	@"GET"
#define kHttpVerbPOST	@"POST"
#define kHttpVerbDELETE @"DELETE"
#define kHttpVerbPUT	@"PUT"

#define kFormatJson		@"json"


@interface RESTInfo : NSObject {
	@private
	NSURL* url;
	NSString* verb;
	NSDictionary* parameters;
	NSObject<RESTWrapperDelegate>* delegate;
	NSString* tag;
}

@property (nonatomic, retain) NSURL* url;
@property (nonatomic, copy)   NSString* verb;
@property (nonatomic, retain) NSDictionary* parameters;
@property (nonatomic, assign) NSObject<RESTWrapperDelegate>* delegate;
@property (nonatomic, copy)	  NSString* tag;

- (id) init:(NSURL*)url usingVerb:(NSString*)verb withParameters:(NSDictionary*)parameters;

@end
