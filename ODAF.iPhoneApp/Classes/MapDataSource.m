//
//  MapDataSource.m
//  VanGuide
//
//  Created by shazron on 09-12-03.
//  Copyright 2009 Nitobi Software Inc. All rights reserved.
//

#import "MapDataSource.h"
#import "KmlPlacemark.h"
#import "RESTWrapper.h"
#import "PointDataSummary.h"
#import "MapAnnotation.h"

@implementation MapDataSource

@synthesize title, uuid, updated, summary;
@synthesize sourceTypes, dataType, rest;
@synthesize delegate, icon, connection, download;
@synthesize xmlParser, tempPlacemark, tempString;

#define kKmlTagPlacemark		@"placemark"
#define kKmlTagName				@"name"
#define kKmlTagCoordinates		@"coordinates"
#define kKmlTagDescription		@"description"


+(MapDataSource*) yourLandmarksDataSource;
{
	// add 'Your Landmarks' social data option
	MapDataSource* dataSource = [[MapDataSource alloc] init];
	dataSource.dataType = SocialData;
	dataSource.uuid = kYourLandmarksFilter;
	dataSource.title = NSLocalizedString(kYourLandmarksText, kYourLandmarksText);
	return [dataSource autorelease];
}


- (id) init
{
	if (self = [super init]) {
		self.sourceTypes = [[NSMutableDictionary alloc] initWithCapacity:3];
		self.tempPlacemark = nil;
		self.xmlParser = nil;
	}
	
	return self;
}

- (void) dealloc
{
	self.rest = nil;
	self.delegate = nil;
	self.sourceTypes = nil;
	self.xmlParser = nil;
	self.tempPlacemark = nil;
	self.tempString = nil;
	
	self.icon = nil;
	self.title = nil;
	self.uuid = nil;
	self.updated = nil;
	self.summary = nil;
	
	[super dealloc];
}

- (BOOL) startLoadingForSourceType:(NSString*) sourceType
{
	if (self.delegate == nil) {
		DebugNSLog(@"Delegate not set for MapDataSource, cancelling load.");
		return NO;
	}
	
	NSString* sourceUrl = [self.sourceTypes objectForKey:sourceType];
	if (sourceUrl == nil) {
		DebugNSLog(@"No url found for sourceType: %@", sourceType);
		return NO;
	}
	
	BOOL handled = NO;
	
	if ([sourceType isEqualToString:kSourceTypeKml]) {
		
		self.download = [NSMutableData data];
		sourceUrl = [sourceUrl stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
		NSURLRequest* request = [NSURLRequest requestWithURL:[NSURL URLWithString:sourceUrl] 
												 cachePolicy:NSURLRequestReturnCacheDataElseLoad
											 timeoutInterval:10.0];
		
		NSURLConnection* conn = [[NSURLConnection alloc] initWithRequest:request delegate:self
                                       startImmediately:NO];
        
		[conn scheduleInRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];

		if ([self.delegate respondsToSelector:@selector(mapDataWillLoad:)]) {
			[self.delegate mapDataWillLoad:self];
		}
		[conn start];
		
		self.connection = conn;
		
		NSPort *aPort = [NSPort port];
		[[NSRunLoop currentRunLoop] addPort:aPort 
									forMode:NSDefaultRunLoopMode];
		
		// Run the runLoop for a few seconds to give the connection request a chance
		[[NSRunLoop currentRunLoop] runUntilDate:[NSDate dateWithTimeIntervalSinceNow:10]];
		
		[conn release];
		
		handled = YES;
	} else 	if ([sourceType isEqualToString:kSourceTypeJson]) {
		self.rest = [[RESTWrapper alloc] init];
		self.rest.delegate = self;
		NSString* pagedUrl = [NSString stringWithFormat:sourceUrl, 0, 100]; // page 0, page_size 100

		if ([self.delegate respondsToSelector:@selector(mapDataWillLoad:)]) {
			[self.delegate mapDataWillLoad:self];
		}

		[self.rest sendRequestTo:[NSURL URLWithString:pagedUrl]	usingVerb:kHttpVerbGET withParameters:nil];
		
		NSPort *aPort = [NSPort port];
		[[NSRunLoop currentRunLoop] addPort:aPort 
									forMode:NSDefaultRunLoopMode];
		
		// Run the runLoop for a few seconds to give the connection request a chance
		[[NSRunLoop currentRunLoop] runUntilDate:[NSDate dateWithTimeIntervalSinceNow:10]];
		
		handled = YES;
		
	} else {
	}
	
	return handled;
}

- (void) cancelLoading
{
	[self.connection cancel];
	
	// Clear the download property to allow later attempts
    self.download = nil;
    
    // Release the connection now that it's finished
    self.connection = nil;
	
	[self.xmlParser abortParsing];
	
	self.tempPlacemark = nil;
	self.tempString = nil;
	self.xmlParser = nil;
}

- (void) parseKml
{
	self.xmlParser = [[NSXMLParser alloc] initWithData:self.download];
	self.xmlParser.delegate = self;
	
	[self.delegate mapDataWillLoad:self];
	[self.xmlParser parse];
}

- (NSString*) iconUrl
{
	return [self.sourceTypes objectForKey:kSourceTypeImagePng];
}

#pragma mark -
#pragma mark RESTWrapper delegate

- (void) wrapper:(RESTWrapper*)wrapper didRetrieveData:(NSData *)data
{
	DebugNSLog(@"Retrieved data: %@", [wrapper responseAsText]);
	
	// Ignore error data
	if (wrapper.lastStatusCode != 200) {
		return;
	}
	
	NSArray* array = (NSArray*)[wrapper responseAsJson];
	NSEnumerator* enumerator = [array objectEnumerator];
	NSDictionary* dict = nil;
	PointDataSummary* aSummary = nil;
	
	while ((dict = [enumerator nextObject])!= nil) 
	{
		aSummary = [[PointDataSummary alloc] init];
		[aSummary setValuesForKeysWithDictionary:dict];
		
		// call delegate, pass it the summary converted to a placemark
		KmlPlacemark* placemark = [aSummary convertToPlacemark];
		placemark.sourceUUID = self.uuid;
		[self.delegate mapDataParsed:self placeMark:placemark];
		
		[aSummary release];
	}
	
	if ([self.delegate respondsToSelector:@selector(mapDataDidLoad:)]) {
		[self.delegate mapDataDidLoad:self];
	}
}

- (void) wrapper:(RESTWrapper*)wrapper didFailWithError:(NSError *)error
{
	NSLog(@"REST call failed: %@", [error description]);
	[self.delegate mapDataDidLoad:self];
}

- (void)wrapper:(RESTWrapper*)wrapper didReceiveStatusCode:(int)statusCode
{
	DebugNSLog(@"Status code: %d", statusCode);
	switch(statusCode) 
	{
		case 200:
			break;
		case 401:
			break;
		default:
			break;
	}
}

#pragma mark -
#pragma mark NSXMLParser delegate

- (void)parser:(NSXMLParser *)parser didStartElement:(NSString *)elementName 
  namespaceURI:(NSString *)namespaceURI qualifiedName:(NSString *)qualifiedName 
	attributes:(NSDictionary *)attributeDict
{
	if ([[elementName lowercaseString] isEqualToString:kKmlTagPlacemark]) {
		self.tempPlacemark = nil;
		self.tempPlacemark = [[KmlPlacemark alloc] init];
	}
}

- (void)parser:(NSXMLParser *)parser didEndElement:(NSString *)elementName 
  namespaceURI:(NSString *)namespaceURI qualifiedName:(NSString *)qName
{
	NSString* lowerElem = [elementName lowercaseString];

	if ([lowerElem isEqualToString:kKmlTagPlacemark]) {
		self.tempPlacemark.sourceUUID = self.uuid;
	
		if ([self.delegate respondsToSelector:@selector(mapDataParsed:placeMark:)]) {
			[self.delegate mapDataParsed:self placeMark:[self.tempPlacemark autorelease]];
		}
		self.tempPlacemark = nil;
		self.tempString = nil;
	}
	
	if (self.tempPlacemark != nil) 
	{
		if ([lowerElem isEqualToString:kKmlTagName]) {
			//self.tempPlacemark.name = self.tempString;
			self.tempPlacemark.guid = self.tempString;
		} else if ([lowerElem isEqualToString:kKmlTagCoordinates]) {
			self.tempPlacemark.coordinates = [self.tempString stringByTrimmingCharactersInSet:
										 [NSCharacterSet whitespaceAndNewlineCharacterSet]];
		} else if ([lowerElem isEqualToString:kKmlTagDescription]) {
			self.tempPlacemark.description = self.tempString;
			self.tempPlacemark.name = self.tempString;
		}
	}
}

- (void)parser:(NSXMLParser*)parser foundCharacters:(NSString *)string
{
	self.tempString = string;
}

- (void)parser:(NSXMLParser*)parser foundCDATA:(NSData *)CDATABlock
{
	self.tempString = [[[NSString alloc] initWithData:CDATABlock encoding:NSUTF8StringEncoding] autorelease];
}

- (void)parser:(NSXMLParser*)parser parseErrorOccurred:(NSError *)parseError
{
	NSLog(@"MapDataSource parser error (%@): %@", [parseError description], self.title);
}

- (void)parserDidEndDocument:(NSXMLParser*)parser
{
	[self.delegate mapDataDidLoad:self];
	DebugNSLog(@"MapDataSource parse end (%@)", self.title);
}


#pragma mark -
#pragma mark Download support (NSURLConnectionDelegate)

- (void) connection:(NSURLConnection*)connection didReceiveData:(NSData *)data
{
    [self.download appendData:data];
}

- (void) connection:(NSURLConnection*)connection didFailWithError:(NSError *)error
{
	[self cancelLoading];
}

- (void) connectionDidFinishLoading:(NSURLConnection*)connection
{
	[self performSelector:@selector(parseKml) withObject:nil afterDelay:0.2];
}

@end
