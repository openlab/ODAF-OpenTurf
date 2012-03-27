//
//  KmlParser.m
//  VanGuide
//
//  Created by shazron on 09-12-14.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import "AtomParser.h"
#import "MapDataSourceModel.h"
#import "MapDataSource.h"

#define kAtomTagEntry		@"entry"
#define kAtomTagTitle		@"title"
#define kAtomTagLink		@"link"
#define kAtomTagType		@"type"
#define kAtomTagHref		@"href"
#define kAtomTagId			@"id"
#define kAtomTagUpdated		@"updated"
#define kAtomTagSummary		@"summary"

@implementation AtomParser

@synthesize xmlParser, tempDataSource, tempString, dataType;

- (id) initWithContentsOfURL:(NSURL*)url andDataType:(NSInteger)myDataType
{
	if (self = [super init]) {
		self.dataType = myDataType;
		
		self.xmlParser = [[NSXMLParser alloc] initWithContentsOfURL:url];
		[self.xmlParser setShouldProcessNamespaces:NO];
		[self.xmlParser setShouldReportNamespacePrefixes:NO];
		[self.xmlParser setShouldResolveExternalEntities:NO];
		[self.xmlParser setDelegate:self];
	}
	
	return self;
}

- (void) parse
{
	[self.xmlParser parse];
}

- (void) dealloc
{
	self.xmlParser = nil;
	self.tempDataSource = nil;
	self.tempString = nil;

	[super dealloc];
}

#pragma mark NSXMLParser delegate

- (void)parser:(NSXMLParser *)parser didStartElement:(NSString *)elementName 
  namespaceURI:(NSString *)namespaceURI qualifiedName:(NSString *)qualifiedName 
	attributes:(NSDictionary *)attributeDict
{
	if ([elementName isEqualToString:kAtomTagEntry]) {
		self.tempDataSource = [[MapDataSource alloc] init];
	}
	
	if (self.tempDataSource != nil) 
	{
		if ([elementName isEqualToString:kAtomTagTitle]) {
		} else if ([elementName isEqualToString:kAtomTagLink]) {
			
			NSString* type = [attributeDict objectForKey:kAtomTagType];
			NSString* href = [attributeDict objectForKey:kAtomTagHref];
			if (type != nil && href != nil) {
				[self.tempDataSource.sourceTypes setValue:href forKey:type];
			}
			
		} else if ([elementName isEqualToString:kAtomTagId]) {
		} else if ([elementName isEqualToString:kAtomTagUpdated]) {
		} else if ([elementName isEqualToString:kAtomTagSummary]) {
		}
	}
}

- (void)parser:(NSXMLParser *)parser didEndElement:(NSString *)elementName 
  namespaceURI:(NSString *)namespaceURI qualifiedName:(NSString *)qName
{
	if ([elementName isEqualToString:kAtomTagEntry]) {
		[[MapDataSourceModel sharedInstance] mapSourceParsed:tempDataSource forDataType:dataType];
		self.tempDataSource = nil;
		self.tempString = nil;
	}
	
	if (tempDataSource != nil) 
	{
		if ([elementName isEqualToString:kAtomTagTitle]) {
			self.tempDataSource.title = self.tempString;
		} else if ([elementName isEqualToString:kAtomTagId]) {
			self.tempDataSource.uuid = self.tempString;
		} else if ([elementName isEqualToString:kAtomTagUpdated]) {
			self.tempDataSource.updated = self.tempString;
		} else if ([elementName isEqualToString:kAtomTagSummary]) {
			self.tempDataSource.summary = self.tempString;
		}
	}
}

- (void)parser:(NSXMLParser *)parser foundCharacters:(NSString *)string
{
	self.tempString = string;
}

- (void)parser:(NSXMLParser *)parser parseErrorOccurred:(NSError *)parseError
{
	NSLog(@"KmlParser error: %@", [parseError description]);
}

- (void)parserDidEndDocument:(NSXMLParser *)parser
{
	[[MapDataSourceModel sharedInstance] mapSourcesDidLoad:dataType];
}

@end
