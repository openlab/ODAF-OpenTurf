//
//  MapDetailsHeaderView.m
//  VanGuide
//
//  Created by shazron on 10-03-12.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "MapDetailsHeaderView.h"


@implementation MapDetailsHeaderView

@synthesize delegate;

- (id) initWithFrame:(CGRect)frame 
{
    if (self = [super initWithFrame:frame]) {
        // Initialization code
    }
    return self;
}

- (void) drawRect:(CGRect)rect 
{
    // Drawing code
}

- (void) dealloc 
{
    [super dealloc];
}

- (void) touchesEnded:(NSSet*)touches withEvent:(UIEvent*)event
{
	UILabel* nameLabel = (UILabel*)[self viewWithTag:1];
	UILabel* descriptionLabel = (UILabel*)[self viewWithTag:2];
	
	UITouch* touch = [[touches allObjects] objectAtIndex:0];
	CGPoint point = [touch locationInView:self];
	
	if (CGRectContainsPoint(nameLabel.frame, point) && !nameLabel.hidden) {
		if (self.delegate != nil) {
			[self.delegate nameLabelTouched];
		}
	} else if (CGRectContainsPoint(descriptionLabel.frame, point) && !descriptionLabel.hidden) {
		if (self.delegate != nil) {
			[self.delegate descriptionLabelTouched];
		}
	}
}

@end
