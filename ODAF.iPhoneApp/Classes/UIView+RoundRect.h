//
//  UIView+RoundRect.h
//  VanGuide
//
//  Created by shazron on 10-02-09.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface UIView(RoundRect)

- (void) setCornerRadius:(CGFloat)aCornerRadius andBorderWidth:(CGFloat)aBorderWidth;
- (void) setDefaultCornerRadius;

@end
