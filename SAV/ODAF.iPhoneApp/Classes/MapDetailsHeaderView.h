//
//  MapDetailsHeaderView.h
//  VanGuide
//
//  Created by shazron on 10-03-12.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "MapDetailsHeaderViewController.h"

@interface MapDetailsHeaderView : UIView {
	id<MapDetailsHeaderDelegate> delegate;
}

@property (nonatomic, retain) id<MapDetailsHeaderDelegate> delegate;

@end
