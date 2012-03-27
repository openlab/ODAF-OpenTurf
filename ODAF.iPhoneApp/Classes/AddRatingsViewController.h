//
//  AddRatingsViewController.h
//  VanGuide
//
//  Created by shazron on 10-02-01.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "S7RatingView.h"
#import "RESTWrapperDelegate.h"

@class PointDataSummary;

@interface AddRatingsViewController : UIViewController <S7RatingDelegate, RESTWrapperDelegate> {
	IBOutlet UIBarButtonItem* rateButton;
	IBOutlet S7RatingView* ratingView;
	IBOutlet UITextView* ratingCaption;
	PointDataSummary* summary;
}

@property (nonatomic, retain) UIBarButtonItem* rateButton;
@property (nonatomic, retain) S7RatingView* ratingView;
@property (nonatomic, retain) UITextView* ratingCaption;
@property (nonatomic, retain) PointDataSummary* summary;

- (IBAction) addRating;

@end
