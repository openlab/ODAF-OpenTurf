//
//  CommentDetailsViewController.h
//  VanGuide
//
//  Created by shazron on 10-02-10.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <UIKit/UIKit.h>

@class AggregateComment;

@interface CommentDetailsViewController : UIViewController {
	IBOutlet UIImageView* imageView;
	IBOutlet UILabel* commentAuthor;
	IBOutlet UILabel* subtitle;
	IBOutlet UITextView* commentTextView;
	IBOutlet UIActivityIndicatorView* spinner;
	AggregateComment* aggregateComment;

    NSMutableData* download;
    NSURLConnection* connection;
}

@property (nonatomic, retain) UIImageView* imageView;
@property (nonatomic, retain) UILabel* commentAuthor;
@property (nonatomic, retain) UILabel* subtitle;
@property (nonatomic, retain) UITextView* commentTextView;
@property (nonatomic, retain) UIActivityIndicatorView* spinner;
@property (nonatomic, retain) AggregateComment* aggregateComment;

@property (nonatomic, retain) NSMutableData* download;
@property (nonatomic, retain) NSURLConnection* connection;

@end
