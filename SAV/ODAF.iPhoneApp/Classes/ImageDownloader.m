//
//  ImageDownloader.m
//  VanGuide
//
//  Created by shazron on 10-02-24.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "ImageDownloader.h"
#import "KmlPlacemark.h"

@implementation ImageDownloader

@synthesize indexPath, delegate, connection, download, tableView, annotation;

#pragma mark

- (void)dealloc
{
	self.indexPath = nil;
	self.delegate = nil;
	self.connection = nil;
	self.download = nil;
	self.tableView = nil;
	self.annotation = nil;
    
    [super dealloc];
}

- (void) start:(NSString*)url
{
    self.download = [NSMutableData data];
    // alloc+init and start an NSURLConnection; release on completion/failure
    NSURLConnection *conn = [[NSURLConnection alloc] initWithRequest:
                             [NSURLRequest requestWithURL:
                              [NSURL URLWithString:url]] delegate:self];
    self.connection = conn;
    [conn release];
}

- (void)cancel
{
    [self.connection cancel];
    self.connection = nil;
    self.download = nil;
}

#pragma mark -
#pragma mark Download support (NSURLConnectionDelegate)

- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data
{
    [self.download appendData:data];
}

- (void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error
{
	// Clear the download property to allow later attempts
    self.download = nil;
    
    // Release the connection now that it's finished
    self.connection = nil;
}

- (void)connectionDidFinishLoading:(NSURLConnection *)connection
{
    UIImage* image = [[[UIImage alloc] initWithData:self.download] autorelease];

    // call our delegate and tell it that our icon is ready for display
	if (self.tableView) {
		[delegate imageDidLoad:image forTableView:self.tableView atIndexPath:self.indexPath];
	}
	if (self.annotation) {
		[delegate imageDidLoad:image forAnnotation:self.annotation];
	}
}


@end
