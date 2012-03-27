//
//  NSMutableArray+QueueAdditions.h
//  VanGuide
//
//  Created by shazron on 10-02-24.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface NSMutableArray (QueueAdditions)

- (id) queueHead;
- (id) dequeue;
- (void) enqueue:(id)obj;

@end
