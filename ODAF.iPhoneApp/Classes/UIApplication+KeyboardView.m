//
//  UIApplication+KeyboardView.m
//  VanGuide
//
//  Created by shazron on 10-02-05.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "UIApplication+KeyboardView.h"


@implementation UIApplication (KeyboardView)

- (UIView*) keyboardView
{
    NSArray *windows = [self windows];
    for (UIWindow *window in [windows reverseObjectEnumerator])
    {
        for (UIView *view in [window subviews])
        {
            if (!strcmp(object_getClassName(view), "UIKeyboard"))
            {
                return view;
            }
        }
    }
    
    return nil;
}

@end