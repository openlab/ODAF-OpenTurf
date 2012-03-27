//
//  RegionPickerDataSource.h
//  VanGuide
//
//  Created by shazron on 8-01-2010.
//  Copyright Nitobi Software Inc. 2010. All rights reserved.
//


@interface RegionPickerDataSource : NSObject <UIPickerViewDataSource, UIPickerViewDelegate>
{
	NSMutableArray* mapRegionDataSources;
}

@property (nonatomic, retain) NSMutableArray* mapRegionDataSources;

@end
