//
//  AddTagViewController.h
//  VanGuide
//
//  Created by shazron on 10-02-01.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RESTWrapperDelegate.h"

@class PointDataSummary;

@interface AddTagViewController : UIViewController <RESTWrapperDelegate> {
	PointDataSummary* summary;
	IBOutlet UITextView* currentTags;
	IBOutlet UITextField* tagToAdd;
	IBOutlet UIBarButtonItem* tagButton;
}

@property (nonatomic, retain) PointDataSummary* summary;
@property (nonatomic, retain) UITextView* currentTags;
@property (nonatomic, retain) UITextField* tagToAdd;
@property (nonatomic, retain) UIBarButtonItem* tagButton;

- (IBAction) addTag;
- (IBAction) textFieldDidUpdate:(id)sender;

@end
