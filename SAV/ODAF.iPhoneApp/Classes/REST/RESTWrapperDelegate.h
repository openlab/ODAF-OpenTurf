//
//  RESTWrapperDelegate.h
//
//  Adapted from public domain code at http://kosmaczewski.net/projects/objective-c-rest-client/

#import <Foundation/Foundation.h> 

@class RESTWrapper;

@protocol RESTWrapperDelegate

@required
- (void)wrapper:(RESTWrapper *)wrapper didRetrieveData:(NSData *)data;

@optional
- (void)wrapperHasBadCredentials:(RESTWrapper *)wrapper;
- (void)wrapper:(RESTWrapper *)wrapper didCreateResourceAtURL:(NSString *)url;
- (void)wrapper:(RESTWrapper *)wrapper didFailWithError:(NSError *)error;
- (void)wrapper:(RESTWrapper *)wrapper didReceiveStatusCode:(int)statusCode;

@end
