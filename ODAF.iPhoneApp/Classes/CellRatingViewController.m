//
//  CellRatingViewController.m
//  VanGuide
//
//  Created by shazron on 10-02-12.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "CellRatingViewController.h"
#import "S7RatingView.h"
#import <QuartzCore/QuartzCore.h>

@implementation CellRatingViewController

@synthesize ratingView, ratingCaption;

- (void)viewDidLoad 
{
    [super viewDidLoad];
	
	CGRect frame = self.view.frame;
	frame.origin = CGPointMake(10, 10);
	self.view.frame = frame;
	
	self.ratingView.starPlaceSize = CGSizeMake(35, 20);
	
	[self.ratingView setStarImage:[UIImage imageNamed:@"star-empty.png"]   forState:STATE_NONSELECTED];
	[self.ratingView setStarImage:[UIImage imageNamed:@"star-full.png"]      forState:STATE_SELECTED];
	[self.ratingView setStarImage:[UIImage imageNamed:@"star-half.png"]  forState:STATE_HALFSELECTED];
	[self.ratingView setStarImage:[UIImage imageNamed:@"star-full.png"]           forState:STATE_HOT];
	[self.ratingView setStarImage:[UIImage imageNamed:@"star-full.png"]   forState:STATE_HIGHLIGHTED];
	[self.ratingView setBackgroundColor:[UIColor clearColor]];
	
	self.ratingView.userInteractionEnabled = NO;
}

- (void) setRatingCountText:(NSUInteger)count
{
	NSString* template_plural = NSLocalizedString(kNRatingsText, kNRatingsText);
	NSString* template_singular = NSLocalizedString(kNRatingText, kNRatingText);
	self.ratingCaption.text = [NSString stringWithFormat:(count == 1 ? template_singular: template_plural), count];
}

- (void)didReceiveMemoryWarning 
{
	// Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
	
	// Release any cached data, images, etc that aren't in use.
}

- (void)viewDidUnload 
{
	// Release any retained subviews of the main view.
	// e.g. self.myOutlet = nil;
}

- (void)dealloc 
{
    [super dealloc];
}

@end
