//
//  ODUser.h
//  VanGuide
//
//  Created by shazron on 10-02-04.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "RESTBase.h"

@class RESTInfo;

@interface ODUser : RESTBase {

}

- (RESTInfo*) RequestAuthToken;
- (RESTInfo*) GetAccessToken:(NSString*)oauth_token;
- (RESTInfo*) Authenticate:(NSString*)oauth_token oauthTokenSecret:(NSString*)oauth_token_secret;
- (RESTInfo*) Current;
- (RESTInfo*) Image:(NSUInteger)userId;
- (RESTInfo*) UpdateTwitterStatus:(NSString*)status withLat:(CGFloat)lat andLong:(CGFloat)lng;

+ (ODUser*) sharedInstance;

@end
