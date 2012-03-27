//
//  CalloutViewController.h
//  VanGuide
//
//  Created by shazron on 10-01-21.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "CalloutViewDisclosureDelegate.h"

@class KmlPlacemark;

@interface CalloutViewController : UIViewController {
	IBOutlet UILabel* textLabel;
	IBOutlet UILabel* subtitleLabel;
	KmlPlacemark* placemark;
	id<CalloutViewDisclosureDelegate> delegate;
}

@property (nonatomic, retain) UILabel* textLabel;
@property (nonatomic, retain) UILabel* subtitleLabel;
@property (nonatomic, retain) KmlPlacemark* placemark;
@property (nonatomic, retain) id<CalloutViewDisclosureDelegate> delegate;


- (id) initWithPlacemark:(KmlPlacemark*)placemark;
- (IBAction) disclosureButtonPressed;

@end
