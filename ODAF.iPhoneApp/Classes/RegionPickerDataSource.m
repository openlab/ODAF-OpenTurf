//
//  RegionPickerDataSource.m
//  VanGuide
//
//  Created by shazron on 8-01-2010.
//  Copyright Nitobi Software Inc. 2010. All rights reserved.
//
#import "RegionPickerDataSource.h"
#import "MapDataSourceModel.h"
#import "MapDataSource.h"

@implementation RegionPickerDataSource

@synthesize mapRegionDataSources;

- (id)init
{
	// use predetermined frame size
	self = [super init];
	if (self)
	{
		self.mapRegionDataSources = [NSMutableArray arrayWithCapacity:5];
	}
	return self;
}

- (void)dealloc
{
	self.mapRegionDataSources = nil;
	[super dealloc];
}


#pragma mark -
#pragma mark UIPickerViewDataSource

- (NSInteger)pickerView:(UIPickerView *)pickerView numberOfRowsInComponent:(NSInteger)component
{
	return [mapRegionDataSources count] + 1;
}

- (NSInteger)numberOfComponentsInPickerView:(UIPickerView *)pickerView
{
	return 1;
}

#pragma mark -
#pragma mark UIPickerViewDelegate

- (NSString *)pickerView:(UIPickerView *)thePickerView titleForRow:(NSInteger)row forComponent:(NSInteger)component 
{
	if (row == 0) {
		return NSLocalizedString(kNoneText, kNoneText);
	}
	
	MapDataSource* mapSource = [self.mapRegionDataSources objectAtIndex:row-1];
	return mapSource.title;
}

@end
