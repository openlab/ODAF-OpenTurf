//
//  RMMarker+Additions.m
//  VanGuide
//
//  Created by shazron on 09-12-14.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import "RMMarker.h"
#import "RMMarker+Additions.h"
#import "MapAnnotation.h"
#import "MapDataSource.h"

@implementation RMMarker(Additions)

- (NSString*) sourceUUID
{
	if ([self.data isKindOfClass:[MapAnnotation class]]) {
		MapAnnotation* annotation = (MapAnnotation*)self.data;
		return annotation.dataSource.uuid;
	}
	return nil;
}

@end
