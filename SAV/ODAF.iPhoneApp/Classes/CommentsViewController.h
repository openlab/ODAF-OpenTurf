//
//  CommentsViewController.h
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RESTWrapperDelegate.h"

typedef enum 
{
	CommentsModeNone,
	LoadFirstPageComments,
	LoadMoreComments
	
} CommentsMode;


@class PointDataSummary, ODComments;

@interface CommentsViewController : UITableViewController < UIScrollViewDelegate, RESTWrapperDelegate > 
{
	PointDataSummary* summary;
	NSMutableArray* comments;
	UIBarButtonItem* addButton;
	BOOL needsUpdate;
	CommentsMode currentMode;
	
	NSUInteger currentPage;
	NSUInteger pageSize;
	NSIndexPath* moreIndexPath;
	UITableViewCell* tvCell;
}

@property (nonatomic, retain) PointDataSummary* summary;
@property (nonatomic, retain) NSMutableArray* comments;
@property (nonatomic, retain) UIBarButtonItem* addButton;
@property (nonatomic, assign) BOOL needsUpdate;
@property (nonatomic, assign) CommentsMode currentMode;
@property (nonatomic, assign) NSUInteger currentPage;
@property (nonatomic, assign) NSUInteger pageSize;
@property (nonatomic, retain) NSIndexPath* moreIndexPath;
@property (nonatomic, assign) IBOutlet UITableViewCell* tvCell;

@end
