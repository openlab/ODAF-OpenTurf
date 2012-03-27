//
//  MapViewController.h
//  VanGuide
//
//  Created by shazron on 09-12-14.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "FlipsideViewController.h"

@class AtomParser;
@class MapDataSource;

@interface MapViewController : UIViewController < UIAlertViewDelegate, UIActionSheetDelegate, FlipsideViewControllerDelegate, UINavigationControllerDelegate > {

	IBOutlet UISegmentedControl* segmentedControl;
	NSThread* filterThread;
	AtomParser* pointDataAtomParser;
	AtomParser* regionDataAtomParser;
	AtomParser* communityDataAtomParser;
	NSNotification* pendingNotification;
	
	IBOutlet UIBarButtonItem* filterButton;
	id popoverController;
		
@private 
	UIViewController* authenticateParentViewController;
}

@property (nonatomic, retain) UISegmentedControl* segmentedControl;
@property (nonatomic, retain) AtomParser* pointDataAtomParser;
@property (nonatomic, retain) AtomParser* regionDataAtomParser;
@property (nonatomic, retain) AtomParser* communityDataAtomParser;
@property (nonatomic, retain) NSThread* filterThread;
@property (nonatomic, retain) UIViewController* authenticateParentViewController;
@property (nonatomic, retain) NSNotification* pendingNotification;

@property (nonatomic, retain) UIBarButtonItem* filterButton;
@property (nonatomic, retain) id popoverController;

- (void) loadMapDataSources;
- (void) filterAnnotations;

- (IBAction) showFilters;
- (IBAction) locateMe;
- (IBAction) locateHome;

- (void) plotMapAnnotation:(NSNotification*)notification;
- (void) plotRegionAnnotation:(NSNotification*)notification;
- (NSArray*) allMapAnnotations;
- (NSString*) categoryTypeForAnnotation:(id)annotation;
- (void) removeMapAnnotations:(NSArray*)annotations;
- (void) goToHomeLocation;
@end
