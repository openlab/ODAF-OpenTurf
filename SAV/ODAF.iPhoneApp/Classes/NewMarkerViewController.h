//
//  NewMarkerViewController.h
//  VanGuide
//
//  Created by shazron on 10-02-16.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RESTWrapperDelegate.h"

@class KmlPlacemark, PointDataSummary;

@interface NewMarkerViewController : UIViewController <RESTWrapperDelegate> {
	IBOutlet UITextField* placeName;
	IBOutlet UITextView* placeDescription;
	UIBarButtonItem* addButton;
	PointDataSummary* summary;
}

@property (nonatomic, retain) UITextField* placeName;
@property (nonatomic, retain) UITextView* placeDescription;
@property (nonatomic, retain) UIBarButtonItem* addButton;
@property (nonatomic, retain) PointDataSummary* summary;

- (IBAction) textFieldDidUpdate:(id) sender;

@end
