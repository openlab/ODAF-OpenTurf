//
//  InfoViewController.h
//  VanGuide
//
//  Created by shazron on 10-02-22.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <UIKit/UIKit.h>


@interface InfoViewController : UIViewController <UIWebViewDelegate> {
	IBOutlet UIWebView* webView;
	IBOutlet UIBarButtonItem* backButton;
	IBOutlet UIBarButtonItem* forwardButton;
	
	IBOutlet UIToolbar* topToolbar;
	IBOutlet UIToolbar* bottomToolbar;
}

@property (nonatomic, retain) UIWebView* webView;
@property (nonatomic, retain) UIBarButtonItem* backButton;
@property (nonatomic, retain) UIBarButtonItem* forwardButton;

@property (nonatomic, retain) UIToolbar* topToolbar;
@property (nonatomic, retain) UIToolbar* bottomToolbar;

- (IBAction) close;

@end
