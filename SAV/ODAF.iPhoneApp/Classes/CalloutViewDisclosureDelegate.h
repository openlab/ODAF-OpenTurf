//
//  CalloutViewDisclosureDelegate.h
//  VanGuide
//
//  Created by shazron on 10-01-21.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <Foundation/Foundation.h>

@class CalloutViewController, KmlPlacemark;

@protocol CalloutViewDisclosureDelegate

- (void) calloutDetails:(CalloutViewController*)controller forPlacemark:(KmlPlacemark*)marker;

@end
