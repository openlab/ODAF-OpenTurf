//
//  MapDetailsHeaderViewController.m
//  VanGuide
//
//  Created by shazron on 10-02-04.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "MapDetailsHeaderViewController.h"
#import <QuartzCore/QuartzCore.h>
#import "MapDetailsHeaderView.h"

@implementation MapDetailsHeaderViewController

@synthesize headerImage, nameLabel, descriptionLabel, name, description, image;
@synthesize nameLabelFrame, descriptionLabelFrame;

- (id) initWithName:(NSString*)myName description:(NSString*)myDescription andImage:(NSString*)myImage;
{
	if (self = [super init]) 
	{
		self.name = myName;
		self.description = myDescription;	
		self.image = myImage;
		((MapDetailsHeaderView*)(self.view)).delegate = self;
	}
	
	return self;
}

- (void) viewDidLoad 
{
    [super viewDidLoad];
	
	// save original origin and sizes (for tap to expand feature)
	self.nameLabelFrame = self.nameLabel.frame;
	self.descriptionLabelFrame = self.descriptionLabel.frame;
	
	self.view.backgroundColor = [UIColor clearColor];
	self.nameLabel.text = self.name;
	self.descriptionLabel.text = self.description;
	
	if ([self.description length] == 0) {
		[self nameLabelTouched];
	}
	
	self.headerImage.image = [UIImage imageNamed:self.image];
	
	self.headerImage.backgroundColor = [UIColor whiteColor];
	[self.headerImage setDefaultCornerRadius];
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

#pragma mark -
#pragma mark MapDetailsHeaderDelegate

- (void) nameLabelTouched
{
	if (CGRectEqualToRect(self.nameLabel.frame, self.nameLabelFrame)) // original state, expand
	{
		self.descriptionLabel.hidden = YES;
		
		// increase the name label, make it multiline
		CGRect frame = self.nameLabelFrame;
		frame.size.height += self.descriptionLabelFrame.size.height;
		self.nameLabel.numberOfLines = 3;
		self.nameLabel.frame = frame;
	} 
	else // expanded state, revert to original
	{ 
		if ([self.description length] == 0) { // if description empty, we want namelabel always expanded
			return;
		}
		
		self.nameLabel.numberOfLines = 1;
		self.nameLabel.frame = self.nameLabelFrame;
		self.descriptionLabel.hidden = NO;
	}
}

- (void) descriptionLabelTouched
{
	if (CGRectEqualToRect(self.descriptionLabel.frame, self.descriptionLabelFrame)) // original state, expand
	{
		self.nameLabel.hidden = YES;
		
		// increase the desc label, make it multiline
		CGRect frame = self.descriptionLabelFrame;
		frame.size.height += self.nameLabelFrame.size.height;
		frame.origin = self.nameLabelFrame.origin;
		
		self.descriptionLabel.numberOfLines = 4;
		self.descriptionLabel.frame = frame;
	} 
	else // expanded state, revert to original
	{ 
		self.descriptionLabel.numberOfLines = 2;
		self.descriptionLabel.frame = self.descriptionLabelFrame;
		self.nameLabel.hidden = NO;
	}
}

@end
