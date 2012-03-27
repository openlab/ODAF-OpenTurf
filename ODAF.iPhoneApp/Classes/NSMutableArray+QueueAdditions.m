//
//  NSMutableArray+QueueAdditions.m
//  VanGuide
//
//  Created by shazron on 10-02-24.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import "NSMutableArray+QueueAdditions.h"

@implementation NSMutableArray (QueueAdditions)

- (id) queueHead
{
    if ([self count] == 0) {
		return nil;
	}
	
    id head = [self objectAtIndex:0];
    return head;
}

- (id) dequeue 
{
    if ([self count] == 0) {
		return nil;
	}
	
    id head = [self objectAtIndex:0];
    if (head != nil) {
        [[head retain] autorelease];
        [self removeObjectAtIndex:0];
    }
	
    return head;
}

- (void) enqueue:(id)object 
{
    [self addObject:object];
}

@end