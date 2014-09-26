//
//  xAuth.h
//  VanGuide
//
//  Created by shazron on 10-03-05.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface xAuth : NSObject {

}

- (void) xAuthAccessTokenForUsername:(NSString*)username andPassword:(NSString*)password;

@end
