//
//  AddCommentViewController.h
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RESTWrapperDelegate.h"

typedef enum 
{
	AddCommentModeNone,
	AddComment,
	TweetThis,
	GetLinkForComment
	
} AddCommentMode;


@class S7RatingView, RESTInfo, PointDataSummary, AggregateComment;

@interface AddCommentViewController : UIViewController <RESTWrapperDelegate, UITextViewDelegate> {
	PointDataSummary* summary;
	AggregateComment* aggregateComment;
	AddCommentMode currentMode;

	IBOutlet UITextView* textView;
	IBOutlet UIBarButtonItem* postButton;
	IBOutlet UISwitch* tweetThisSwitch;
}

@property (nonatomic, retain) PointDataSummary* summary;
@property (nonatomic, retain) AggregateComment* aggregateComment;
@property (nonatomic, assign) AddCommentMode currentMode;

@property (nonatomic, retain) UITextView* textView;
@property (nonatomic, retain) UIBarButtonItem* postButton;
@property (nonatomic, retain) UISwitch* tweetThisSwitch;

@end
