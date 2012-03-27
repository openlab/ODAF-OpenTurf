//
//  ODHome.h
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h> 
#import "RESTBase.h"

@class RESTInfo;

@interface ODHome : RESTBase {
	
}

- (RESTInfo*) Credits;
- (RESTInfo*) GetLinkForSummary:(NSUInteger)summaryId;
- (RESTInfo*) GetLinkForComment:(NSUInteger)commentId;

+ (ODHome*) sharedInstance;

@end