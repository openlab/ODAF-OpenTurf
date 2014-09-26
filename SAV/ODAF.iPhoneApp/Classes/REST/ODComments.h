//
//  ODComments.h
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h> 
#import "RESTBase.h"

@class RESTInfo;

@interface ODComments : RESTBase {

}

- (RESTInfo*) List:(NSUInteger)summaryId;
- (RESTInfo*) List:(NSUInteger)summaryId  page:(NSUInteger)page page_size:(NSUInteger)page_size;
- (RESTInfo*) Show:(NSUInteger)commentId;
- (RESTInfo*) Add:(NSUInteger)summaryId text:(NSString*) text;
- (RESTInfo*) Edit:(NSUInteger)commentId text:(NSString*) text;
- (RESTInfo*) Remove:(NSUInteger) commentId;

+ (ODComments*) sharedInstance;

@end
