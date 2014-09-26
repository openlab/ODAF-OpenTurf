//
//  FlipsideViewController.h
//  VanGuide
//
//  Created by shazron on 09-11-27.
//  Copyright Nitobi Software Inc 2009. All rights reserved.
//

@protocol FlipsideViewControllerDelegate;


@interface FlipsideViewController : UIViewController {
	id <FlipsideViewControllerDelegate> delegate;
	IBOutlet UITableView* tableView;
	IBOutlet UIView* regionPickerView;
	IBOutlet UIPickerView* pickerView;
	IBOutlet UIBarButtonItem* buttonRegionPickerDone;
	IBOutlet UINavigationBar* navbar;
	
	NSIndexPath* dataSourceNeedsAuthIndexPath;
}

@property (nonatomic, assign) id <FlipsideViewControllerDelegate> delegate;
@property (nonatomic, retain) UITableView* tableView;
@property (nonatomic, retain) UIView* regionPickerView;
@property (nonatomic, retain) UIPickerView* pickerView;
@property (nonatomic, retain) UINavigationBar* navbar;
@property (nonatomic, retain) NSIndexPath* dataSourceNeedsAuthIndexPath;

- (IBAction) buttonChooseFiltersDoneSelected;
- (IBAction) buttonRegionPickerDoneSelected;

@end


@protocol FlipsideViewControllerDelegate
- (void)flipsideViewControllerDidFinish:(FlipsideViewController *)controller;
@end

