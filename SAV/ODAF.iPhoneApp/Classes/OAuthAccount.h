//
//  OAuthAccount.h
//  VanGuide
//
//  Created by shazron on 10-03-11.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface OAuthAccount : NSObject {
	NSUInteger Id;
	NSDate* LastAccessedOn;
	NSDate* TokenExpiry;
	NSUInteger UserAccess;
	NSUInteger UserRole;
	NSUInteger oauth_service_id;
	NSString* oauth_token;
	NSString* oauth_token_secret;
	NSString* profile_image_url;
	NSString* screen_name;
	NSUInteger user_id;
}

@property (nonatomic, assign) NSUInteger Id; // our db id
@property (nonatomic, retain) NSDate* LastAccessedOn;
@property (nonatomic, retain) NSDate* TokenExpiry;
@property (nonatomic, assign) NSUInteger UserAccess;
@property (nonatomic, assign) NSUInteger UserRole;
@property (nonatomic, assign) NSUInteger oauth_service_id;
@property (nonatomic, copy)   NSString* oauth_token;
@property (nonatomic, copy)   NSString* oauth_token_secret;
@property (nonatomic, copy)   NSString* profile_image_url;
@property (nonatomic, copy)   NSString* screen_name;
@property (nonatomic, assign) NSUInteger user_id; // Twitter user id


@end
