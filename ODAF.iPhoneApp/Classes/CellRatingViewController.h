//
//  CellRatingViewController.h
//  VanGuide
//
//  Created by shazron on 10-02-12.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <UIKit/UIKit.h>

@class S7RatingView;

@interface CellRatingViewController : UIViewController {
	IBOutlet S7RatingView* ratingView;
	IBOutlet UILabel* ratingCaption;
}

@property (nonatomic, retain) S7RatingView* ratingView;
@property (nonatomic, retain) UILabel* ratingCaption;

- (void) setRatingCountText:(NSUInteger)count;

@end
