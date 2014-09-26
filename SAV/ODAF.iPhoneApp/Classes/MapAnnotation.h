//
//  MapAnnotation.h
//  VanGuide
//
//  Created by shazron on 10-02-24.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <Foundation/Foundation.h>


@class MapDataSource, KmlPlacemark;

@interface MapAnnotation : NSObject {
	@private
	MapDataSource* dataSource;
	KmlPlacemark* placemark;
}

@property (nonatomic, retain) MapDataSource* dataSource;
@property (nonatomic, retain) KmlPlacemark* placemark;

@end
