//
//  MapDetailsViewController.m
//  VanGuide
//
//  Created by shazron on 10-01-08.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import "MapDetailsViewController.h"
#import "KmlPlacemark.h"
#import "CommentsViewController.h"
#import "PointDataSummary.h"
#import "PointDataComment.h"
#import "VanGuideAppDelegate.h"
#import "CellRatingViewController.h"
#import "AddCommentViewController.h"
#import "AddRatingsViewController.h"
#import "AddTagViewController.h"
#import "OAuthBrowserViewController.h"
#import "MapDetailsHeaderViewController.h"
#import "MapDataSourceModel.h"
#import "RESTWrapper.h"
#import "ODSummaries.h"
#import "ODComments.h"
#import "CommentDetailsViewController.h"
#import "TweetThisViewController.h"
#import "MapAnnotation.h"
#import "ColorfulButton.h"
#import "MapDataSource.h"

#define RATINGS_TAGS_SECTION	0
#define COMMENTS_SECTION 1
#define ADD_COMMENT_SECTION 2

@implementation ViewData

@synthesize SummaryId, CommentId;

@end


@implementation MapDetailsViewController

@synthesize tvCell, tableView, annotation, summary, comments, currentMode, viewData, ratingViewController;


// Implement viewDidLoad to do additional setup after loading the view, typically from a nib.
- (void)viewDidLoad 
{
    [super viewDidLoad];
	
	self.tableView.dataSource = self;
	self.tableView.delegate = self;
	
	self.comments = [NSMutableArray arrayWithCapacity:3];
	
	// Register for AddComment notif
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(getSummary) 
												 name:ODRESTCommentAdded object:nil];
	// Register for AddTag notif
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(getSummary) 
												 name:ODRESTTagAdded object:nil];

	// Register for AddRating notif
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(getSummary) 
												 name:ODRESTRatingAdded object:nil];
	
	// Register for SummaryRequired notif
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(createSummary) 
												 name:ODRESTSummaryRequired object:nil];
	
	self.summary = [PointDataSummary newFromPlacemark:self.annotation.placemark];
	if (viewData == nil) {
		[self performSelector:@selector(getSummary) withObject:nil afterDelay:0.5];
	} else {
		self.summary.Name = NSLocalizedString(kLoadingText, kLoadingText);
		[self performSelector:@selector(viewCommentGetSummary) withObject:nil afterDelay:0.5];
	}
}

- (void)viewDidDisappear:(BOOL)animated
{
	[[Utils sharedInstance] loadingStop];
	[ODComments sharedInstance].webservice.delegate = nil;	
	[ODSummaries sharedInstance].webservice.delegate = nil;	
	[super viewDidDisappear:animated];
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
	
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTAuthenticateSuccess object: nil];
}

- (void)dealloc 
{
    [super dealloc];
}

- (void) addComment
{
	AddCommentViewController* viewController = [[AddCommentViewController alloc] init];
	viewController.summary = self.summary;
	
	[[VanGuideAppDelegate sharedApplicationDelegate] pushViewControllerToStack:viewController];
	[viewController release];
}

- (void) tweetThis
{
	TweetThisViewController* viewController = [[TweetThisViewController alloc] init];
	viewController.summary = self.summary;
	
	[[VanGuideAppDelegate sharedApplicationDelegate] pushViewControllerToStack:viewController];
	[viewController release];
}

- (UIView*) headerView
{
	NSString* icon = [[MapDataSourceModel sharedInstance].iconMapSettings objectForKey:self.annotation.placemark.sourceUUID];
	if (icon == nil) {
		icon = kPinAddedPoint;
	}

	if (viewData == nil) 
	{
		NSString* description = self.annotation.placemark.isOGDI ? @"" : self.annotation.placemark.description;
		MapDetailsHeaderViewController* viewController = [[[MapDetailsHeaderViewController alloc] initWithName:self.annotation.placemark.name 
																								   description:description
																									  andImage:icon] autorelease];
		return viewController.view;
	}
	else 
	{
		MapDetailsHeaderViewController* viewController = [[[MapDetailsHeaderViewController alloc] initWithName:self.summary.Name 
																								   description:self.summary.Description
																									  andImage:icon] autorelease];
		return viewController.view;
		
	}
}

#pragma mark -
#pragma mark UITableViewDataSource protocol

- (UITableViewCell*) tableView:(UITableView*)myTableView cellForRowAtIndexPath:(NSIndexPath*)indexPath
{
	NSString *CellIdentifier = @"map_details_cells";
	switch (indexPath.section) 
	{
		case RATINGS_TAGS_SECTION:
			CellIdentifier = @"ratings_tags_cells";
			break;
		case COMMENTS_SECTION:
			CellIdentifier = @"CommentCellMiniIdentifier";
			break;
		case ADD_COMMENT_SECTION:
			CellIdentifier = @"add_comment_cells";
			break;
		default:
			break;
	}
		
    UITableViewCell* cell = [myTableView dequeueReusableCellWithIdentifier:CellIdentifier];
	// Static data
    if (cell == nil) 
	{
		
		switch (indexPath.section) 
		{
			case RATINGS_TAGS_SECTION:
			{
				cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier] autorelease];

				cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
				if (indexPath.row == 0) // Rating
				{ 
					if (self.ratingViewController == nil ) {
						self.ratingViewController = [[CellRatingViewController alloc] init];
					}
					
					[cell.contentView addSubview:self.ratingViewController.view];
				} 
				else // Tag
				{
					cell.textLabel.text = NSLocalizedString(kTagsText, kTagsText);
				}

			}
				break;
			case COMMENTS_SECTION:
			{
				[[NSBundle mainBundle] loadNibNamed:@"CommentTVCellMini" owner:self options:nil];
				cell = tvCell;
				self.tvCell = nil;
			}
				break;
			case ADD_COMMENT_SECTION:
			{
				cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier] autorelease];
				
				NSUInteger button_spacing = 5;
				NSUInteger button_count = 1; // don't forget to change this value if you add more buttons below
				NSUInteger button_width =  (cell.frame.size.width - ((button_count - 1) * button_spacing) - (2 * cell.indentationWidth)) / button_count;
				NSUInteger button_height = cell.frame.size.height;
				NSUInteger button_x = button_width + button_spacing;
				NSUInteger button_index = 0;
				CGRect aFrame;
				
				UIColor* gradientHigh = [UIColor colorWithHexString:[[Utils sharedInstance] getConfigSetting:kConfigKeyButtonGradientHighRGB]];
				UIColor* gradientLow = [UIColor colorWithHexString:[[Utils sharedInstance] getConfigSetting:kConfigKeyButtonGradientLowRGB]];
				
				aFrame = CGRectMake(button_index++ * button_x, 0, button_width, button_height);
				ColorfulButton* addCommentButton = [[ColorfulButton alloc] initWithFrame:aFrame];
				[addCommentButton setTitle:NSLocalizedString(kAddCommentText, kAddCommentText) forState:UIControlStateNormal];
				[addCommentButton addTarget:self action:@selector(addComment) forControlEvents:UIControlEventTouchUpInside];
				[addCommentButton setHighColor:gradientHigh];
				[addCommentButton setLowColor:gradientLow];
				[cell.contentView addSubview:addCommentButton];
				[addCommentButton release];
				
//				aFrame = CGRectMake(button_index++ * button_x, 0, button_width, button_height);
//				ColorfulButton* tweetThisButton = [[ColorfulButton alloc] initWithFrame:aFrame];
//				[tweetThisButton setTitle:NSLocalizedString(kTweetThisText, kTweetThisText) forState:UIControlStateNormal];
//				[tweetThisButton addTarget:self action:@selector(tweetThis) forControlEvents:UIControlEventTouchUpInside];
//				[tweetThisButton setHighColor:gradientHigh];
//				[tweetThisButton setLowColor:gradientLow];
//				[cell.contentView addSubview:tweetThisButton];
			}
				break;
			default:
				break;
		}
		cell.clipsToBounds = YES;
		cell.contentView.clipsToBounds = YES;
	} 
	
	// Dynamic data
	switch (indexPath.section) 
	{
		case RATINGS_TAGS_SECTION:
		{
			if (indexPath.row == 0) // Rating
			{ 
				[self.ratingViewController setRatingCountText:self.summary.RatingCount];
				[self.ratingViewController.ratingView setRating:[self.summary calculatedRatingFromMin:1 andMax:5]];
			}
		}
			break;
		case COMMENTS_SECTION:
		{
			UILabel* nameLabel = (UILabel*)[cell viewWithTag:1];
			UILabel* commentLabel = (UILabel*)[cell viewWithTag:2];
			UILabel* timeLabel = (UILabel*)[cell viewWithTag:3];
			
			cell.textLabel.text = @"";
			commentLabel.text = @"";
			timeLabel.text = @"";
			nameLabel.text = @"";
			
			if ([self.comments count] == 0) 
			{
				// need to hide because they are opaque for performance reasons
				nameLabel.hidden = YES;
				commentLabel.hidden = YES;
				timeLabel.hidden = YES;
				
				cell.textLabel.font = [UIFont systemFontOfSize:13];
				cell.textLabel.textAlignment = UITextAlignmentCenter;
				cell.textLabel.text = NSLocalizedString(kNoCommentsText, kNoCommentsText);
			} 
			else 
			{
				cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
				AggregateComment* aggregateComment = [self.comments objectAtIndex:indexPath.row];
				if (aggregateComment.isValid) {
					
					// need to show because they might be hidden before
					nameLabel.hidden = NO;
					commentLabel.hidden = NO;
					timeLabel.hidden = NO;
					
					nameLabel.text = aggregateComment.CommentAuthor;
					commentLabel.textAlignment = UITextAlignmentLeft;
					commentLabel.text = aggregateComment.Comment.Text;
					timeLabel.text = [[Utils sharedInstance] dateDiffFromNowToString:aggregateComment.Comment.CreatedOn withThreshold:YES];
				} else {
					
					// need to hide because they are opaque for performance reasons
					nameLabel.hidden = YES;
					commentLabel.hidden = YES;
					timeLabel.hidden = YES;
					
					cell.textLabel.font = [UIFont boldSystemFontOfSize:13];
					cell.textLabel.textAlignment = UITextAlignmentCenter;
					cell.textLabel.text = aggregateComment.Comment.Text;
				}
			}
		}
			break;
		default:
			break;
	}
			
    return cell;
}

- (void)tableView:(UITableView *)tableView willDisplayCell:(UITableViewCell*)cell forRowAtIndexPath:(NSIndexPath *)indexPath 
{ 
	switch (indexPath.section) 
	{
		case ADD_COMMENT_SECTION:
			cell.backgroundView = nil; // remove the background
			break;
		default:
			break;
	}
}
- (NSString*) tableView:(UITableView*)tableView titleForHeaderInSection:(NSInteger)section
{
	switch (section) 
	{
		case RATINGS_TAGS_SECTION:
			return self.annotation.placemark.name;
		case COMMENTS_SECTION:
		{
			NSUInteger count = self.summary.CommentCount;
			NSString* locComments = NSLocalizedString(kCommentsText, kCommentsText);
			NSString* header = [NSString stringWithFormat:@"%@ (%ld)", locComments, count];
			return count > 0 ? header : locComments;
		}
		default:
			return @"";
	}
}

- (CGFloat) tableView:(UITableView *)tableView heightForHeaderInSection:(NSInteger)section
{
	switch (section) 
	{
		case RATINGS_TAGS_SECTION:
			return 70.0f;
		case COMMENTS_SECTION:
			return 26.0f;
		default:
			return 0;
	}
}

- (UIView *)tableView:(UITableView *)tableView viewForHeaderInSection:(NSInteger)section
{
	switch (section) 
	{
		case RATINGS_TAGS_SECTION:
			return [self headerView];
		default:
			return nil;
	}
}

- (NSInteger) numberOfSectionsInTableView:(UITableView*)tableView
{
	return 3;
}

- (NSInteger) tableView:(UITableView*)tableView numberOfRowsInSection:(NSInteger)section
{
	switch (section) 
	{
		case RATINGS_TAGS_SECTION:
			return 2;
		case COMMENTS_SECTION:
			return MAX([self.comments count], 1);
		default:
			return 1;
	}
}

- (NSInteger) tableView:(UITableView*)tableView sectionForSectionIndexTitle:(NSString*)title atIndex:(NSInteger)index
{
	return 0;
}

- (void)tableView:(UITableView *)aTableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
	switch(indexPath.section)
	{
		case RATINGS_TAGS_SECTION:
		{
			if (indexPath.row == 0) { // Rating
				AddRatingsViewController* viewController = [ [ AddRatingsViewController alloc ] init ];
				viewController.summary = self.summary;

				[[VanGuideAppDelegate sharedApplicationDelegate] pushViewControllerToStack:viewController];
				[viewController release];
				
			} else { // Tag
				AddTagViewController* viewController = [ [ AddTagViewController alloc ] init ];
				viewController.summary = self.summary;

				[[VanGuideAppDelegate sharedApplicationDelegate] pushViewControllerToStack:viewController];
				[viewController release];
			}
		}
			break;
		case COMMENTS_SECTION:
		{
			if ([self.comments count] == 0) {
				return;
			}
			
			AggregateComment* aggregateComment = [self.comments objectAtIndex:indexPath.row];
			VanGuideAppDelegate* delg = [VanGuideAppDelegate sharedApplicationDelegate];
			[aTableView deselectRowAtIndexPath:indexPath animated:YES];
			
			if (aggregateComment.isValid) {
				CommentDetailsViewController* viewController = [[CommentDetailsViewController alloc] init];
				viewController.aggregateComment = aggregateComment;
				viewController.title = NSLocalizedString(kCommentText, kCommentText);
				
				[delg pushViewControllerToStack:viewController];
				[viewController release];
			} else {
				CommentsViewController* viewController = [[CommentsViewController alloc] init];
				viewController.summary = self.summary;
				viewController.title = NSLocalizedString(kCommentsText, kCommentsText);

				[delg pushViewControllerToStack:viewController];
				[viewController release];
			}
		} 
			break;
		case ADD_COMMENT_SECTION: 
			// do nothing, already handled in button handlers
			break;
		default:
			break;
	}
}

- (CGFloat) tableView:(UITableView*)aTableView heightForRowAtIndexPath:(NSIndexPath*)indexPath
{
	switch (indexPath.section) 
	{
		case COMMENTS_SECTION:
			if ([self.comments count] == 0) {
				return aTableView.rowHeight;
			}
			return (indexPath.row < 2) ? 50.0f : 45.0f;
		case ADD_COMMENT_SECTION:
			return 45.0f;
		default:
			return aTableView.rowHeight;
	}
}

#pragma mark -
#pragma mark REST calls

- (void) viewCommentGetComment
{
	// No need authentication for this
	
	ODComments* commentsAPI = [ODComments sharedInstance];
	RESTInfo* call = [commentsAPI Show:viewData.CommentId];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false withText:NSLocalizedString(kLoadingText, kLoadingText)] 
								withView:self.view];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = ViewCommentGetComment;
	[commentsAPI.webservice sendRequest:call];
}

- (void) viewCommentGetSummary
{
	// No need authentication for this
	
	ODSummaries* summariesAPI = [ODSummaries sharedInstance];
	RESTInfo* call = [summariesAPI ShowById:viewData.SummaryId];
	call.delegate = self;
	
	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false withText:NSLocalizedString(kLoadingText, kLoadingText)] 
								withView:self.view];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = ViewCommentGetSummary;
	[summariesAPI.webservice sendRequest:call];
}

- (void) getSummary
{
	// No need authentication for this
	
	ODSummaries* summariesAPI = [ODSummaries sharedInstance];
	RESTInfo* call = [summariesAPI ShowByGuid:self.annotation.placemark.guid];
	call.delegate = self;

	[[Utils sharedInstance] loadingStart:[[Utils sharedInstance] createLoadingViewOptionsFullScreen:false withText:NSLocalizedString(kLoadingText, kLoadingText)] 
								withView:self.view];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;

	self.currentMode = GetSummary;
	[summariesAPI.webservice sendRequest:call];
}

- (void) addSpinnerForLoadingComments
{
	NSIndexPath* indexPath = [NSIndexPath indexPathForRow:0 inSection:COMMENTS_SECTION];
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

- (void) viewCommentLoadComments
{
	// No need authentication for this
	
	ODComments* commentsAPI = [ODComments sharedInstance];
	RESTInfo* call = [commentsAPI List:self.summary.Id page:0 page_size:kMapDetailsCommentsPageSize];
	call.delegate = self;
	
	[self addSpinnerForLoadingComments];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = ViewCommentLoadComments;
	[commentsAPI.webservice sendRequest:call];
}

- (void) loadComments
{
	// No need authentication for this
	
	ODComments* commentsAPI = [ODComments sharedInstance];
	RESTInfo* call = [commentsAPI List:self.summary.Id page:0 page_size:kMapDetailsCommentsPageSize];
	call.delegate = self;
		
	[self addSpinnerForLoadingComments];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	 
	self.currentMode = LoadComments;
	[commentsAPI.webservice sendRequest:call];
}

- (void) createSummary
{
	DebugNSLog(@"Creating summary...");
	
	ODSummaries* summariesAPI = [ODSummaries sharedInstance];
	
	PointDataSummary* newSummary = [[PointDataSummary newFromPlacemark:self.annotation.placemark] autorelease];
	newSummary.Description = @""; // TODO: we override the Guid in the description
	RESTInfo* call = [summariesAPI Add:newSummary];
	call.delegate = self;
	
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	self.currentMode = AddSummary;
	[summariesAPI.webservice sendRequest:call];
}

- (void) authSuccess:(NSNotification*)notification
{
	// remove notif registration for authenticate
	[[NSNotificationCenter defaultCenter] removeObserver:self name:ODRESTAuthenticateSuccess object:nil];
	
	//NSDictionary* userDict = (NSDictionary*)notification.object;

	// now since we are authenticated, we check whether the summary has been created
	// if not, we send out the notif to create it
	if (!self.summary.isCreated) 
	{
		[self createSummary];
	} 
}

#pragma mark -
#pragma mark RESTWrapperDelegate

- (void) wrapper:(RESTWrapper*)wrapper didRetrieveData:(NSData *)data
{
	DebugNSLog(@"Retrieved data: %@", [wrapper responseAsText]);
	[[Utils sharedInstance] loadingStop];
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
	
	// Ignore error data
	if (wrapper.lastStatusCode < 200 || wrapper.lastStatusCode >= 300) {
		return;
	}
	
	switch (self.currentMode) 
	{
		case AddSummary:
		{
			DebugNSLog(@"Success! Added summary on server.");
			
			NSDictionary* dict = (NSDictionary*)[wrapper responseAsJson];
			[self.summary setValuesForKeysWithDictionary:dict];
			
			[self.tableView reloadData];

			// Notify on success
			NSNotification* notif = [NSNotification notificationWithName:ODRESTSummaryAddSuccess object: self];
			[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
		}
			break;
		case ViewCommentGetSummary:
		{
			NSDictionary* dict = (NSDictionary*)[wrapper responseAsJson];
			[self.summary setValuesForKeysWithDictionary:dict];
			// update placemark
			self.annotation.placemark = [self.summary convertToPlacemark];
			
			// Pin is not on map, post notif to add pin
			MapAnnotation* newAnnotation = [[[MapAnnotation alloc] init] autorelease];
			newAnnotation.dataSource = [MapDataSource yourLandmarksDataSource];
			newAnnotation.placemark = self.annotation.placemark;
			
			NSNotification* notif = [NSNotification notificationWithName:ODAddPointForViewComment object:newAnnotation];
			[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
			// ///////////////////////////////////////
			
			
			[self.tableView reloadData];
			
			// Load comments
			[self viewCommentLoadComments];
		}
			break;
		case GetSummary:
		{
			NSDictionary* dict = (NSDictionary*)[wrapper responseAsJson];
			[self.summary setValuesForKeysWithDictionary:dict];
			// update placemark
			self.annotation.placemark = [self.summary convertToPlacemark];
			
			[self.tableView reloadData];
			
			// Load comments
			[self loadComments];
		}
			break;
		case ViewCommentGetComment:
		{
			NSDictionary* dict = (NSDictionary*)[wrapper responseAsJson];
			AggregateComment* aggregateComment = [[AggregateComment alloc] init];
			[aggregateComment setValuesForKeysWithDictionary:dict];
			
			CommentDetailsViewController* viewController = [[CommentDetailsViewController alloc] init];
			viewController.aggregateComment = aggregateComment;
			viewController.title = NSLocalizedString(kCommentText, kCommentText);
			
			VanGuideAppDelegate* delg = [VanGuideAppDelegate sharedApplicationDelegate];
			[delg pushViewControllerToStack:viewController];
			[viewController release];
		}
			break;
		case ViewCommentLoadComments:
		{
			[self viewCommentGetComment];
		}
			// fall through
		case LoadComments:
		{
			// stop the animation, remove the spinner in the first row
			NSIndexPath* indexPath = [NSIndexPath indexPathForRow:0 inSection:COMMENTS_SECTION];
			UITableViewCell* cell = [self.tableView cellForRowAtIndexPath:indexPath];
			UIActivityIndicatorView* spinner = (UIActivityIndicatorView*)[cell viewWithTag:kTableViewCellSpinner];
			[spinner stopAnimating];
			[spinner removeFromSuperview];
			
			NSArray* array = (NSArray*)[wrapper responseAsJson]; 
			self.comments = nil;
			self.comments = [NSMutableArray arrayWithCapacity:[array count]];
			
			NSEnumerator* enumerator = [array objectEnumerator];
			NSDictionary* dict = nil;
			BOOL showMoreCommentsRow = (self.summary.CommentCount > [array count]);

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
				comment.Text = [NSString stringWithFormat:@"%d %@", (self.summary.CommentCount - kMapDetailsCommentsPageSize), moreText];
				comment.CreatedById = 0;

				AggregateComment* agg = [[AggregateComment alloc] initWithComment:comment];
				[self.comments addObject:agg];
				[agg release];
				[comment release];
			}
			
			// Update Comments
			[self.tableView reloadData];
		}
			break;
		default:
			break;
	}
}

- (void) wrapper:(RESTWrapper*)wrapper didFailWithError:(NSError*)error
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
		case 201:
			break;
		case 401: // Not Authorized
		{
			// register for authenticate notif
			[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(authSuccess:) 
														 name:ODRESTAuthenticateSuccess object:nil];

			UIViewController* authParentViewController = self.modalViewController;
			if (self.modalViewController == nil) {
				// find the viewcontroller on top of the stack
				authParentViewController = [[VanGuideAppDelegate sharedApplicationDelegate] topViewControllerOnStack];
			}
			
			[[NSNotificationCenter defaultCenter] postNotificationName:ODRESTAuthorizationRequired object:authParentViewController];
		}
			break;
		case 404:
			if (self.currentMode == GetSummary) { // it's ok if Summary not found, we are doing a lazy-create
				break;
			}
		default:
			[[Utils sharedInstance] showAlert:[NSString stringWithFormat:@"Code: %d", statusCode] 
									withTitle:NSLocalizedString(kErrorText, kErrorText)];
			
			if (self.currentMode == AddSummary) {
				DebugNSLog(@"Failed! for Add Summary on server.");
				
				NSNotification* notif = [NSNotification notificationWithName:ODRESTSummaryAddFail object: self];
				[[NSNotificationQueue defaultQueue] enqueueNotification:notif postingStyle: NSPostASAP];
			}
			break;
	}
}

@end
