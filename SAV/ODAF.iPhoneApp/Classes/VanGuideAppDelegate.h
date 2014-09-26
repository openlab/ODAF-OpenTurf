//
//  MSOpenDataAppDelegate.h
//  VanGuide
//
//  Created by shazron on 09-11-27.
//  Copyright Nitobi Software Inc 2009. All rights reserved.
//
#import <Foundation/Foundation.h>

@class MapViewController;

@interface VanGuideAppDelegate : NSObject <UIApplicationDelegate> {
    UIWindow *window;
	MapViewController* viewController;
	UINavigationController* navigationController;
	UIImageView* splashScreen;
}

@property (nonatomic, retain) IBOutlet UIWindow *window;
@property (nonatomic, retain) UIImageView* splashScreen;
@property (nonatomic, retain) MapViewController* viewController;
@property (nonatomic, retain) UINavigationController* navigationController;

+ (VanGuideAppDelegate*) sharedApplicationDelegate;
- (void) pushViewControllerToStack:(UIViewController*)aViewController;
- (UIViewController*) topViewControllerOnStack;

@end

