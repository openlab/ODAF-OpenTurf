//
//  MapDataSource.h
//  VanGuide
//
//  Created by shazron on 09-12-03.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MapDataSourceDelegate.h"
#import "RESTWrapperDelegate.h"

// MIME types
#define kSourceTypeKml			@"application/vnd.google-earth.kml+xml"
#define kSourceTypeCsv			@"text/csv"
#define kSourceTypeExcel		@"application/vnd.ms-excel"
#define kSourceTypeImagePng		@"image/png"
#define kSourceTypeJson			@"text/json"

@class RESTWrapper;

@interface MapDataSource : NSObject <RESTWrapperDelegate> {
	NSString* title;
	NSString* uuid;
	NSString* updated;
	NSString* summary;
	NSMutableDictionary* sourceTypes;
	NSInteger dataType;
	UIImage* icon;
	RESTWrapper* rest;
	
	id<MapDataDelegate> delegate;
	
@private
	// XML Parser stuff (and temporary objects for parser)
	NSXMLParser* xmlParser;
	KmlPlacemark* tempPlacemark;
	NSString* tempString;
	
	// NSURLConnection stuff
    NSURLConnection* connection;
	NSMutableData* download;
}

@property (nonatomic, copy) NSString* title;
@property (nonatomic, copy) NSString* uuid;
@property (nonatomic, copy) NSString* updated;
@property (nonatomic, copy) NSString* summary;
@property (nonatomic, retain) NSMutableDictionary* sourceTypes;
@property (nonatomic, assign) NSInteger dataType;
@property (nonatomic, readonly) NSString* iconUrl;
@property (nonatomic, retain) UIImage* icon;
@property (nonatomic, retain) RESTWrapper* rest;

@property (nonatomic, retain) NSURLConnection* connection;
@property (nonatomic, retain) NSMutableData* download;

@property (nonatomic, assign) id<MapDataDelegate> delegate;

@property (nonatomic, retain) NSXMLParser* xmlParser;
@property (nonatomic, retain) KmlPlacemark* tempPlacemark;
@property (nonatomic, copy)   NSString* tempString;

- (BOOL) startLoadingForSourceType:(NSString*) sourceType;
- (void) cancelLoading;

+(MapDataSource*) yourLandmarksDataSource;

@end
