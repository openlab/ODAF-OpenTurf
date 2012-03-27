//
//  xAuth.m
//  VanGuide
//
//  Created by shazron on 10-03-05.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//
//	Adapted from Norio Nomura's code at:
//	http://github.com/norio-nomura/ntlniph/commit/5ce25d68916cd45254c7ff2ba9b91de4f324899a#diff-4
//

#import "xAuth.h"
#import "OAMutableURLRequest.h"
#import "OADataFetcher.h"

@implementation xAuth

- (void) xAuthAccessTokenForUsername:(NSString*)username andPassword:(NSString*)password
{
    NSURL *url = [NSURL URLWithString:@"https://api.twitter.com/oauth/access_token"];
	
	OAConsumer* consumer = [[[OAConsumer alloc] initWithKey:kTwitterAppConsumerKey 
													secret:kTwitterAppConsumerSecret] autorelease];
	
    OAMutableURLRequest *request = [[[OAMutableURLRequest alloc] initWithURL:url
																	consumer:consumer 
																	   token:nil   // we don't have a Token yet
																	   realm:nil   // our service provider doesn't specify a realm
														   signatureProvider:nil]  // use the default method, HMAC-SHA1
									autorelease]; 
	
    [request setHTTPMethod:@"POST"];
	[request setParameters:[NSArray arrayWithObjects:
							[OARequestParameter requestParameterWithName:@"x_auth_mode" value:@"client_auth"],
							[OARequestParameter requestParameterWithName:@"x_auth_username" value:username],
							[OARequestParameter requestParameterWithName:@"x_auth_password" value:password],
							nil]];
	
    OADataFetcher *fetcher = [[[OADataFetcher alloc] init] autorelease];
	[fetcher fetchDataWithRequest:request
						 delegate:self
				didFinishSelector:@selector(accessTokenTicket:didFinishWithData:)
				  didFailSelector:@selector(accessTokenTicket:didFailWithError:)];
}


- (void) accessTokenTicket:(OAServiceTicket*)ticket didFinishWithData:(NSMutableData*)data 
{
    NSString *responseBody = [[[NSString alloc] initWithData:data
                                                   encoding:NSUTF8StringEncoding] autorelease];

    OAToken *token = [[[OAToken alloc] initWithHTTPResponseBody:responseBody] autorelease];
	
	NSNotification* notif = [NSNotification notificationWithName:ODRESTxAuthSuccess object:token];
	[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
}

- (void) accessTokenTicket:(OAServiceTicket*)ticket didFailWithError:(NSError*)error 
{
	NSNotification* notif = [NSNotification notificationWithName:ODRESTxAuthFail object:error];
	[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
}

@end
