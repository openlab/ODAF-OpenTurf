//
//  KmlParser.h
//  VanGuide
//
//  Created by shazron on 09-12-14.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@class MapDataSource;

@interface AtomParser : NSObject {

	NSInteger dataType;
	
@private
	// XML Parser stuff (and temporary objects for parser)
	NSXMLParser* xmlParser;
	MapDataSource* tempDataSource;
	NSMutableString* tempString;
}

@property (nonatomic, assign) NSInteger dataType;
@property (nonatomic, retain) NSXMLParser* xmlParser;
@property (nonatomic, retain) MapDataSource* tempDataSource;
@property (nonatomic, retain)   NSMutableString* tempString;

- (id) initWithContentsOfURL:(NSURL*)url andDataType:(NSInteger)dataType;
- (void) parse;

@end
