//
//  TweetThisViewController.h
//  VanGuide
//
//  Created by shazron on 10-02-22.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RESTWrapperDelegate.h"

typedef enum 
{
	TweetThisModeNone,
	PostTweet,
	GetShortenedLink
	
} TweetThisMode;

@class PointDataSummary;

@interface TweetThisViewController : UIViewController <RESTWrapperDelegate, UITextViewDelegate> {
	IBOutlet UITextView* tweetTextView;
	PointDataSummary* summary;
	IBOutlet UIBarButtonItem* tweetButton;
	IBOutlet UILabel* tweetCharCountLabel;
	TweetThisMode currentMode;
}

@property (nonatomic, retain) UITextView* tweetTextView;
@property (nonatomic, retain) PointDataSummary* summary;
@property (nonatomic, retain) UIBarButtonItem* tweetButton;
@property (nonatomic, retain) UILabel* tweetCharCountLabel;
@property (nonatomic, assign) TweetThisMode currentMode;

@end
