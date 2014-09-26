//
//  MapDataSourceModel.h
//  VanGuide
//
//  Created by shazron on 09-12-07.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MapDataSourceDelegate.h"
#import "RegionPickerDataSource.h"
#import "ImageDownloader.h"

@class MapDataSourceChangeSet;
@class RegionPickerDataSource;

@interface MapDataSourceModel : NSObject < ImageDownloaderDelegate, MapSourceDelegate, MapDataDelegate, UITableViewDataSource, UITableViewDelegate > {
	NSMutableArray* mapPointDataSources;
	NSMutableArray* mapSocialDataSources;
	RegionPickerDataSource* regionPickerDataSource;
	
	NSDictionary* iconMapSettings;
	NSMutableDictionary* filterSettings;
	MapDataSourceChangeSet* changeSet;
	
	NSMutableDictionary* imageDownloadsInProgress;
}

@property (nonatomic, retain) NSMutableArray* mapPointDataSources;
@property (nonatomic, retain) NSMutableArray* mapSocialDataSources;
@property (nonatomic, retain) RegionPickerDataSource* regionPickerDataSource;
@property (nonatomic, retain) NSDictionary* iconMapSettings;
@property (nonatomic, retain) NSMutableDictionary* filterSettings;
@property (nonatomic, retain) MapDataSourceChangeSet* changeSet;
@property (nonatomic, retain) NSMutableDictionary* imageDownloadsInProgress;

+ (MapDataSourceModel*) sharedInstance;
- (void) reset;
- (void) reinitialize;

@end


@interface MapDataSourceChangeSet : NSObject
{
	NSMutableArray* additions;
	NSMutableArray* deletions;
}

@property (nonatomic, retain) NSMutableArray* additions;
@property (nonatomic, retain) NSMutableArray* deletions;

@end