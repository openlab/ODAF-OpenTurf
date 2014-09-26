//
//  CALayer+RoundRect.m
//  VanGuide
//
//  Created by shazron on 10-02-09.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "UIView+RoundRect.h"
#import <QuartzCore/QuartzCore.h>


@implementation UIView(RoundRect)

- (void) setCornerRadius:(CGFloat)aCornerRadius andBorderWidth:(CGFloat)aBorderWidth;
{
	[self layer].cornerRadius = aCornerRadius;
	[self layer].masksToBounds = YES;
	[self layer].borderWidth = aBorderWidth;
}

- (void) setDefaultCornerRadius
{
	[self setCornerRadius:8.0f andBorderWidth:0.5f];
}

@end
