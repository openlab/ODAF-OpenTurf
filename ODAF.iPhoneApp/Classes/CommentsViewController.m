//
//  CommentsViewController.m
//  VanGuide
//
//  Created by shazron on 18/01/10.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import "CommentsViewController.h"
#import "AddCommentViewController.h"
#import "PointDataSummary.h"
#import "RESTWrapper.h"
#import "ODComments.h"
#import "PointDataSummary.h"
#import "PointDataComment.h"
#import "CommentDetailsViewController.h"
#import "VanGuideAppDelegate.h"

@implementation CommentsViewController


@synthesize tvCell, summary, comments, addButton, needsUpdate, currentMode, currentPage, pageSize, moreIndexPath;

- (id) init
{
	if (self = [super initWithStyle:UITableViewStylePlain]) {
		self.comments = [NSMutableArray arrayWithCapacity:15];
		self.currentMode = CommentsModeNone;
		self.currentPage = 0;
		self.pageSize = kCommentsPageSize;
		self.tableView.pagingEnabled = NO;
	}
	
	return self;
}

- (void)viewDidLoad 
{
	DebugNSLog(@"View did load");

    [super viewDidLoad];
	
	self.needsUpdate = YES;
	self.tableView.rowHeight = 60.0f;
	
	self.addButton = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemAdd 
													  target:self action:@selector(addComment)];
	self.navigationItem.rightBarButtonItem = self.addButton;
	
	// Register for AddComment notif
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(refreshComments) 
												 name:ODRESTCommentAdded object:nil];
}

- (void) viewDidUnload
{
	[super viewDidUnload];
}

- (void) viewDidAppear:(BOOL)animated
{
	if (self.needsUpdate) {
		self.comments = nil;
		[self.tableView reloadData];
		
		self.comments = [NSMutableArray arrayWithCapacity:10];
		[self performSelector:@selector(loadFirstPageComments) withObject:nil afterDelay:0.1];
		self.needsUpdate = NO;
	}

	[super viewDidAppear:animated];
}

- (void)viewDidDisappear:(BOOL)animated
{
	ODComments* commentsAPI = [ODComments sharedInstance];
	[[Utils sharedInstance] loadingStop];
	commentsAPI.webservice.delegate = nil;	
	[super viewDidDisappear:animated];
}

- (void) addComment
{
	AddCommentViewController* viewController = [[AddCommentViewController alloc] init];
	viewController.summary = self.summary;
	
	[[VanGuideAppDelegate sharedApplicationDelegate] pushViewControllerToStack:viewController];
	[viewController release];
}

- (void) addSpinnerForLoadingComments
{
	NSIndexPath* indexPath = [NSIndexPath indexPathForRow:0 inSection:0];
	UITableViewCell* cell = [self.tableView cellForRowAtIndexPath:indexPath];
	cell.textLabel.text = @"";
	
	UIActivityIndicatorView* spinner = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
	spinner.tag = kTableViewCellSpinner;
	[cell.contentView addSubview:spinner];
	spinner.frame = CGRectMake(145, 10, 20, 20); // TODO: this should be calculated to be middle of cell
	spinner.hidesWhenStopped = YES;
	[spinner startAnimating];
	[spinner release];
}

- (void) loadFirstPageComments
{
	DebugNSLog(@"Load First Page Comments");
	
	// No need authentication for this

	ODComments* commentsAPI = [ODComments sharedInstance];
	RESTInfo* call = [commentsAPI List:self.summary.Id page:self.currentPage page_size:self.pageSize];
	call.tag = @"loadFirstPageComments";
	call.delegate = self;
	
	[self addSpinnerForLoadingComments];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = LoadFirstPageComments;
	[commentsAPI.webservice sendRequest:call];
}

- (void) loadMoreComments
{
	DebugNSLog(@"Load More Comments");
	
	// No need authentication for this
	
	ODComments* commentsAPI = [ODComments sharedInstance];
	RESTInfo* call = [commentsAPI List:self.summary.Id page:self.currentPage+1 page_size:self.pageSize];
	call.tag = @"loadMoreComments";
	call.delegate = self;
	
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = LoadMoreComments;
	[commentsAPI.webservice sendRequest:call];
}

- (void) refreshComments
{
	DebugNSLog(@"Refreshing comments.");
	
	self.needsUpdate = YES;
	
	// TODO: must update Summary.CommentCount!
	
	//[self viewDidAppear:NO];
}

- (void) didReceiveMemoryWarning
{
	// Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
	
	// Release any cached data, images, etc that aren't in use.
}

- (void)dealloc 
{
    [super dealloc];
}

#pragma mark -
#pragma mark RESTWrapperDelegate protocol

- (void)wrapper:(RESTWrapper *)wrapper didRetrieveData:(NSData *)data
{
	DebugNSLog(@"Retrieved data: %@", [wrapper responseAsText]);
	
	if (wrapper.lastStatusCode != 200) {
		[[Utils sharedInstance] loadingStop];
		[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
		return;
	}
	
	switch (self.currentMode) 
	{
		case LoadFirstPageComments:
		{
			// stop the animation, remove the spinner in the first row
			NSIndexPath* indexPath = [NSIndexPath indexPathForRow:0 inSection:0];
			UITableViewCell* cell = [self.tableView cellForRowAtIndexPath:indexPath];
			UIActivityIndicatorView* spinner = (UIActivityIndicatorView*)[cell viewWithTag:kTableViewCellSpinner];
			[spinner stopAnimating];
			[spinner removeFromSuperview];
			
			NSArray* array = (NSArray*)[wrapper responseAsJson];
			
			NSEnumerator* enumerator = [array objectEnumerator];
			NSDictionary* dict = nil;
			BOOL showMoreCommentsRow = (self.summary.CommentCount > [array count]);
			NSUInteger remainder = (self.summary.CommentCount - [array count]);
			
			while ((dict = [enumerator nextObject])!= nil) 
			{
				AggregateComment* agg = [[AggregateComment alloc] init];
				[agg setValuesForKeysWithDictionary:dict];
				[self.comments addObject:agg];
				[agg release];
			}
			
			if (showMoreCommentsRow) {
				PointDataComment* comment = [[PointDataComment alloc] init];
				NSString* moreText = NSLocalizedString(kMoreText, kMoreText);
				comment.Text = [NSString stringWithFormat:@"%d %@", remainder, moreText];
				comment.CreatedById = 0;
				
				AggregateComment* agg = [[AggregateComment alloc] initWithComment:comment];
				[self.comments addObject:agg];
				[agg release];
				[comment release];
			}
			
			if (remainder == 0) {
				PointDataComment* comment = [[PointDataComment alloc] init];
				comment.Text = kListTerminationText;
				comment.CreatedById = 0;
				
				AggregateComment* agg = [[AggregateComment alloc] initWithComment:comment];
				[self.comments addObject:agg];
				[agg release];
				[comment release];
			}
			
			// Update Comments
			if ([array count] > 0) {
				[self.tableView reloadData];
			}			
		}
			break;
		case LoadMoreComments:
		{
			NSArray* array = (NSArray*)[wrapper responseAsJson]; 
			NSEnumerator* enumerator = [array objectEnumerator];
			NSDictionary* dict = nil;
			
			if ([array count] > 0) {
				UITableViewCell* cell = [self.tableView cellForRowAtIndexPath:self.moreIndexPath];
				UIActivityIndicatorView* spinner = (UIActivityIndicatorView*)[cell viewWithTag:kTableViewCellSpinner];
				[spinner stopAnimating];
				[spinner removeFromSuperview];
				
				[self.comments removeObjectAtIndex:self.moreIndexPath.row];
				[self.tableView reloadData];
			}			

			NSUInteger currentCount = ([self.comments count] + [array count]);
			BOOL showMoreCommentsRow = (self.summary.CommentCount > currentCount);
			NSUInteger remainder = (self.summary.CommentCount - currentCount);
			
			while ((dict = [enumerator nextObject])!= nil) 
			{
				AggregateComment* agg = [[AggregateComment alloc] init];
				[agg setValuesForKeysWithDictionary:dict];
				[self.comments addObject:agg];
				[agg release];
			}
			
			if (showMoreCommentsRow) {
				PointDataComment* comment = [[PointDataComment alloc] init];
				NSString* moreText = NSLocalizedString(kMoreText, kMoreText);
				comment.Text = [NSString stringWithFormat:@"%d %@", remainder, moreText];
				comment.CreatedById = 0;
				
				AggregateComment* agg = [[AggregateComment alloc] initWithComment:comment];
				[self.comments addObject:agg];
				[agg release];
				[comment release];
			}
			
			if (remainder == 0) {
				PointDataComment* comment = [[PointDataComment alloc] init];
				comment.Text = kListTerminationText;
				comment.CreatedById = 0;
				
				AggregateComment* agg = [[AggregateComment alloc] initWithComment:comment];
				[self.comments addObject:agg];
				[agg release];
				[comment release];
			}
			
			// Update Comments
			if ([array count] > 0) {
				// increment the current page
				self.currentPage += 1;
				[self.tableView reloadData];
				[self.tableView scrollToRowAtIndexPath:self.moreIndexPath atScrollPosition:UITableViewScrollPositionBottom animated:NO];
			}			
		}
			break;
		default:
			break;
	}
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
}

- (void) wrapper:(RESTWrapper*)wrapper didFailWithError:(NSError *)error
{
	NSLog(@"REST call error: %@", [error description]);
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
}

- (void) wrapper:(RESTWrapper*)wrapper didReceiveStatusCode:(int)statusCode
{
	DebugNSLog(@"Status code: %d", statusCode);
	
	switch(statusCode) 
	{
		case 200:
			break;
		default:
			[[Utils sharedInstance] showAlert:[NSString stringWithFormat:@"Code: %d", statusCode] 
									withTitle:NSLocalizedString(kErrorText, kErrorText)];
			break;
	}
}

#pragma mark -
#pragma mark UITableViewDataSource protocol

- (UITableViewCell*) tableView:(UITableView*)aTableView cellForRowAtIndexPath:(NSIndexPath*)indexPath
{
	NSUInteger nodeCount = [self.comments count];
	
	if (nodeCount == 0 && indexPath.row == 0)
	{
		DebugNSLog(@"Empty initial");
        UITableViewCell* cell = [aTableView dequeueReusableCellWithIdentifier:@"temp_cell"];
        if (cell == nil)
		{
            cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleSubtitle
										   reuseIdentifier:@"temp_cell"] autorelease];  
			
            cell.detailTextLabel.textAlignment = UITextAlignmentCenter;
			cell.selectionStyle = UITableViewCellSelectionStyleNone;
        }
		
		return cell;
    }
	
	AggregateComment* aggregateComment = [self.comments objectAtIndex:indexPath.row];
	
	static NSString* MyIdentifier = @"CommentCellIdentifier";
	
	UITableViewCell *cell = [aTableView dequeueReusableCellWithIdentifier:MyIdentifier];
    if (cell == nil) 
	{
        [[NSBundle mainBundle] loadNibNamed:@"CommentTVCell" owner:self options:nil];
        cell = tvCell;
        self.tvCell = nil;
    }
	
	UILabel* nameLabel = (UILabel*)[cell viewWithTag:1];
	UILabel* commentLabel = (UILabel*)[cell viewWithTag:2];
	UILabel* timeLabel = (UILabel*)[cell viewWithTag:3];
	
	// More... setup
	nameLabel.text = @"";
	commentLabel.text = @"";
	timeLabel.text = @"";
	cell.textLabel.text = @"";

	// Comment setup
	if (aggregateComment.isValid) {
		commentLabel.text = aggregateComment.Comment.Text;

		timeLabel.text = [[Utils sharedInstance] dateDiffFromNowToString:aggregateComment.Comment.CreatedOn withThreshold:YES];
		cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
		commentLabel.textAlignment = UITextAlignmentLeft;
		nameLabel.text = aggregateComment.CommentAuthor;
	} else {
		cell.textLabel.text = aggregateComment.Comment.Text;
		cell.textLabel.font = [UIFont boldSystemFontOfSize:14.0f];

		cell.accessoryType = UITableViewCellAccessoryNone;
		cell.textLabel.textAlignment = UITextAlignmentCenter;
	}

    return cell;
}

- (NSInteger) numberOfSectionsInTableView:(UITableView*)tableView
{
	DebugNSLog(@"No of sections");
	return 1;
}

- (NSInteger) tableView:(UITableView*)tableView numberOfRowsInSection:(NSInteger)section
{
	DebugNSLog(@"No of rows in section");
	return MAX(1, [self.comments count]);
}

- (void)tableView:(UITableView *)aTableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
	AggregateComment* aggregateComment = [self.comments objectAtIndex:indexPath.row];
	VanGuideAppDelegate* delg = [VanGuideAppDelegate sharedApplicationDelegate];
	
	if (aggregateComment.isValid) 
	{
		CommentDetailsViewController* viewController = [[CommentDetailsViewController alloc] init];
		viewController.aggregateComment = aggregateComment;
		viewController.title = NSLocalizedString(kCommentText, kCommentText);
		[delg pushViewControllerToStack:viewController];
		[viewController release];
	} 
	else 
	{
		// this is the "MORE..." cell
		[aTableView deselectRowAtIndexPath:indexPath animated:YES];
		
		UITableViewCell* cell = [aTableView cellForRowAtIndexPath:indexPath];
		if ([cell.textLabel.text isEqualToString:kListTerminationText]) {
			return;
		}
		
		cell.textLabel.text = @"";

		UIActivityIndicatorView* spinner = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
		spinner.tag = kTableViewCellSpinner;
		[cell.contentView addSubview:spinner];
		spinner.frame = CGRectMake(145, 20, 20, 20); // TODO: this should be calculate to be middle of cell
		spinner.hidesWhenStopped = YES;
		[spinner startAnimating];
		[spinner release];
		
		self.moreIndexPath = indexPath;
		
		[self loadMoreComments];
	}
}

@end
