//
//  ImageDownloader.h
//  VanGuide
//
//  Adapted from Apple's LazyTableImages sample code
//
//  Created by shazron on 10-02-24.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <Foundation/Foundation.h>

@class MapAnnotation;

@protocol ImageDownloaderDelegate

@optional
- (void) imageDidLoad:(UIImage*)image forTableView:(UITableView*)tableView atIndexPath:(NSIndexPath*)indexPath;
- (void) imageDidLoad:(UIImage*)image forAnnotation:(MapAnnotation*)annotation;

@end

@interface ImageDownloader : NSObject {
    NSIndexPath* indexPath;
	UITableView* tableView;
	MapAnnotation* annotation;
    id <ImageDownloaderDelegate> delegate;
    
    NSMutableData* download;
    NSURLConnection* connection;
}

@property (nonatomic, retain) NSIndexPath* indexPath;
@property (nonatomic, retain) UITableView* tableView;
@property (nonatomic, retain) MapAnnotation* annotation;
@property (nonatomic, assign) id <ImageDownloaderDelegate> delegate;

@property (nonatomic, retain) NSMutableData* download;
@property (nonatomic, retain) NSURLConnection* connection;

- (void) start:(NSString*)url;
- (void) cancel;


@end
