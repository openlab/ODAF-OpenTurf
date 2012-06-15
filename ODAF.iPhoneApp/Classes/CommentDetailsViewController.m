//
//  CommentDetailsViewController.m
//  VanGuide
//
//  Created by shazron on 10-02-10.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "CommentDetailsViewController.h"
#import "PointDataComment.h"
#import "ODUser.h"
#import "RESTInfo.h"

@implementation CommentDetailsViewController

@synthesize imageView, commentAuthor, subtitle, commentTextView;
@synthesize aggregateComment, spinner, download, connection;


- (void)viewDidLoad 
{
    [super viewDidLoad];
	
	self.imageView.backgroundColor = [UIColor whiteColor];
	[self.imageView setDefaultCornerRadius];
	
	self.commentAuthor.text = self.aggregateComment.CommentAuthor;
	self.commentTextView.text = self.aggregateComment.Comment.Text;
	
	NSDateFormatter* format = [[[NSDateFormatter alloc] init] autorelease];
	[format setDateFormat:@"MMM dd, yyyy HH:mm a"];
	self.subtitle.text = [format stringFromDate:self.aggregateComment.Comment.CreatedOn];
	
	[self performSelector:@selector(updateImage) withObject:nil afterDelay:0.2];
}

- (void) updateImage
{
	RESTInfo* restInfo = [[ODUser sharedInstance] Image:self.aggregateComment.Comment.CreatedById];
	
    self.download = [NSMutableData data];
    // alloc+init and start an NSURLConnection; release on completion/failure
    NSURLConnection *conn = [[NSURLConnection alloc] initWithRequest:
                             [NSURLRequest requestWithURL:restInfo.url] delegate:self];
    self.connection = conn;
    [conn release];
}

- (void)viewDidDisappear:(BOOL)animated
{
	[[Utils sharedInstance] loadingStop];
	[self.connection cancel];
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
	
    [self.connection cancel];
    self.connection = nil;
    self.download = nil;
}

- (void)dealloc 
{
	self.imageView = nil;
	self.commentAuthor = nil;
	self.subtitle = nil;
	self.commentTextView = nil;
	self.spinner = nil;
	self.aggregateComment = nil;
	
	self.download = nil;
	self.connection = nil;
	
    [super dealloc];
}

#pragma mark -
#pragma mark Download support (NSURLConnectionDelegate)

- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data
{
    [self.download appendData:data];
}

- (void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error
{
    // Release the connection now that it's finished
    self.connection = nil;
}

- (void)connectionDidFinishLoading:(NSURLConnection *)connection
{
	CGRect imageViewFrame = self.imageView.frame;
	
	UIGraphicsBeginImageContext(CGSizeMake(imageViewFrame.size.width, imageViewFrame.size.height));
	UIImage* tempImage = [[UIImage alloc] initWithData:self.download];
	[tempImage drawInRect:CGRectMake(0, 0, imageViewFrame.size.width, imageViewFrame.size.height)];
	UIImage* image = UIGraphicsGetImageFromCurrentImageContext();
	UIGraphicsEndImageContext();	
	
	self.imageView.image = image;
	[tempImage release];
	
	[self.spinner stopAnimating];
}

@end
