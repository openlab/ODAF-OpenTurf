//
//  RESTBase.h
//  VanGuide
//
//  Created by shazron on 10-02-04.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <Foundation/Foundation.h>

@class RESTWrapper;

@interface RESTBase : NSObject {
	NSString* appController;
	NSString* appUrl;
	NSString* appId;
	RESTWrapper* webservice;
}

@property (nonatomic, copy) NSString* appController;
@property (nonatomic, copy) NSString* appUrl;
@property (nonatomic, copy) NSString* appId;
@property (nonatomic, retain) RESTWrapper* webservice;

- (NSString*) buildUrl:(NSString*)appAction;
- (NSString*) buildUrl:(NSString*)appAction withId:(NSUInteger)anId;

@end
