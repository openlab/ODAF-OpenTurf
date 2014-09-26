//
//  CalloutViewController.m
//  VanGuide
//
//  Created by shazron on 10-01-21.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "CalloutViewController.h"
#import "KmlPlacemark.h"

@implementation CalloutViewController

@synthesize textLabel, subtitleLabel, placemark, delegate;

- (id) initWithPlacemark:(KmlPlacemark*)aPlacemark
{
	if (self = [super init]) {
		self.placemark = aPlacemark;
	}
	
	return self;
}

- (void)viewDidLoad 
{
    [super viewDidLoad];
	
	self.view.backgroundColor = [UIColor clearColor];
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
	self.placemark = nil;
	self.delegate = nil;
	self.textLabel = nil;
	self.subtitleLabel = nil;
	
    [super dealloc];
}

- (IBAction) disclosureButtonPressed
{
	if (self.delegate) {
		[self.delegate calloutDetails:self forPlacemark:self.placemark];
	}
}

@end
