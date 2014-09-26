//
//  RESTWrapper.h
//
//  Adapted from public domain code at http://kosmaczewski.net/projects/objective-c-rest-client/

#import <Foundation/Foundation.h> 
#import "RESTWrapperDelegate.h"
#import "RESTInfo.h"


@interface RESTWrapper : NSObject 
{
@private
    NSMutableData* receivedData;
    NSString* mimeType;
    NSURLConnection *conn;
    BOOL asynchronous;
    NSObject<RESTWrapperDelegate>* delegate;
    NSString* username;
    NSString* password;
	int lastStatusCode;
	
@private
	NSMutableArray* callQueue;
}

@property (nonatomic, readonly) NSData *receivedData;
@property (nonatomic) BOOL asynchronous;
@property (nonatomic, copy) NSString *mimeType;
@property (nonatomic, copy) NSString *username;
@property (nonatomic, copy) NSString *password;
@property (assign) NSObject<RESTWrapperDelegate>* delegate; // Do not retain delegates!
@property (nonatomic, assign) int lastStatusCode;
@property (retain) NSMutableArray* callQueue;

- (void) sendRequestTo:(NSURL*)url usingVerb:(NSString*)verb withParameters:(NSDictionary*)parameters;
- (void) sendRequest:(RESTInfo*)info;

- (void) uploadData:(NSData*)data toURL:(NSURL*)url;
- (void) cancelConnection;

- (NSDictionary*) responseAsPropertyList;
- (NSString*) responseAsText;
- (NSObject*) responseAsJson;

@end

