//
//  MapDetailsViewController.h
//  VanGuide
//
//  Created by shazron on 10-01-08.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RMMarker.h"
#import "RMMarker+Additions.h"
#import "RESTWrapperDelegate.h"

typedef enum 
{
	SummaryDetailModeNone,
	GetSummary,
	AddSummary,
	LoadComments,
	ViewCommentLoadComments,
	ViewCommentGetSummary,
	ViewCommentGetComment
	
} SummaryDetailMode;


@interface ViewData : NSObject {
	NSUInteger SummaryId;
	NSUInteger CommentId;
}

@property (nonatomic, assign) NSUInteger SummaryId;
@property (nonatomic, assign) NSUInteger CommentId;

@end


@class MapAnnotation;
@class PointDataSummary;
@class CellRatingViewController;

@interface MapDetailsViewController : UIViewController <UITableViewDataSource, UITableViewDelegate, RESTWrapperDelegate > {
	IBOutlet UITableView* tableView;
	MapAnnotation* annotation;
	PointDataSummary* summary;
	NSMutableArray* comments;
	SummaryDetailMode currentMode;
	ViewData* viewData;
	CellRatingViewController* ratingViewController;
	
	UITableViewCell* tvCell;
}

@property (nonatomic, retain) UITableView* tableView;
@property (nonatomic, retain) MapAnnotation* annotation;
@property (nonatomic, retain) PointDataSummary* summary;
@property (nonatomic, retain) NSMutableArray* comments;
@property (nonatomic, assign) SummaryDetailMode currentMode;
@property (nonatomic, retain) ViewData* viewData;
@property (nonatomic, retain) CellRatingViewController* ratingViewController;
@property (nonatomic, assign) IBOutlet UITableViewCell* tvCell;

@end
