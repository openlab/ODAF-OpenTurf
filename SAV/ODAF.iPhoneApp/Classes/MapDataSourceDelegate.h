//
//  MapDataSourceDelegate.h
//  VanGuide
//
//  Created by shazron on 09-12-03.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@class MapDataSource;
@class KmlPlacemark;

typedef enum  {
	PointData,
	SocialData,
	RegionData
	
} MapDataType;

@protocol MapDataDelegate <NSObject>

@optional
- (void) mapDataWillLoad:(MapDataSource*)dataSource;
- (void) mapDataDidLoad:(MapDataSource*)dataSource;
@required
- (void) mapDataParsed:(MapDataSource*)dataSource placeMark:(KmlPlacemark*)placeMark;

@end


@protocol MapSourceDelegate <NSObject>

- (void) mapSourcesWillLoad:(MapDataType)dataType;
- (void) mapSourceParsed:(MapDataSource*)dataSource forDataType:(MapDataType)dataType;
- (void) mapSourcesDidLoad:(MapDataType)dataType;

@end
