//
//  RESTWrapper.m
//
//  Adapted from public domain code at http://kosmaczewski.net/projects/objective-c-rest-client/

#import "RESTWrapper.h"
#import "SBJSON.h"

@interface RESTWrapper (Private)
- (void)startConnection:(NSURLRequest *)request;
@end

@implementation RESTWrapper

@synthesize receivedData;
@synthesize asynchronous;
@synthesize mimeType;
@synthesize username;
@synthesize password;
@synthesize delegate;
@synthesize lastStatusCode;
@synthesize callQueue;

#pragma mark -
#pragma mark Constructor and destructor

- (id) init
{
    if(self = [super init])
    {
        receivedData = [[NSMutableData alloc] init];
        conn = nil;

        asynchronous = YES;
        mimeType = @"text/json";
        username = @"";
        password = @"";
		
		self.callQueue = [NSMutableArray arrayWithCapacity:2];
		self.delegate = nil;
    }

    return self;
}

- (void) dealloc
{
    [receivedData release];
    receivedData = nil;
    self.mimeType = nil;
    self.username = nil;
    self.password = nil;
	self.delegate = nil;
    [super dealloc];
}

#pragma mark -
#pragma mark Public methods

- (void) sendNextRequest
{
	// Send the next item in the queue, if any
	if ([self.callQueue count] > 0) {
		RESTInfo* rest = [self.callQueue queueHead];
		if (rest.delegate != nil) {
			self.delegate = rest.delegate;
		}
		if (rest.tag != nil) {
			DebugNSLog(@"Sending request with tag: %@", rest.tag);
		}
		[self sendRequestTo:rest.url usingVerb:rest.verb withParameters:rest.parameters];
	}	
}

- (void) sendRequest:(RESTInfo*)info
{
	// Add to the queue
	[self.callQueue enqueue:info];
	
	// If its the only one in the queue, send it
	if ([self.callQueue count] == 1) {
		[self sendNextRequest];
	} else {
	}
}

- (void) sendRequestTo:(NSURL*)url usingVerb:(NSString*)verb withParameters:(NSDictionary*)parameters
{
    NSData* body = nil;
    NSMutableString* params = nil;
    NSString* contentType = @"text/html; charset=utf-8";
    NSURL* finalURL = url;
	DebugNSLog(@"RESTWrapper url: %@ withVerb: %@ andParamCount: %d", url, verb, 
			   (parameters == nil? 0 : [parameters count]));
	
    if (parameters != nil)
    {
        params = [[NSMutableString alloc] init];
        for (id key in parameters)
        {
            NSString *encodedKey = [key stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
            CFStringRef value = (CFStringRef)[[parameters objectForKey:key] copy];
            // Escape even the "reserved" characters for URLs 
            // as defined in http://www.ietf.org/rfc/rfc2396.txt
            CFStringRef encodedValue = CFURLCreateStringByAddingPercentEscapes(kCFAllocatorDefault, 
                                                                               value,
                                                                               NULL, 
                                                                               (CFStringRef)@";/?:@&=+$,", 
                                                                               kCFStringEncodingUTF8);
            [params appendFormat:@"%@=%@&", encodedKey, encodedValue];
            CFRelease(value);
            CFRelease(encodedValue);
        }
        [params deleteCharactersInRange:NSMakeRange([params length] - 1, 1)];
    }
    
    if ([verb isEqualToString:kHttpVerbPOST] || [verb isEqualToString:kHttpVerbPUT])
    {
        contentType = @"application/x-www-form-urlencoded; charset=utf-8";
        body = [params dataUsingEncoding:NSUTF8StringEncoding];
    }
    else
    {
        if (parameters != nil)
        {
            NSString* urlWithParams = [[url absoluteString] stringByAppendingFormat:@"?%@", params];
            finalURL = [NSURL URLWithString:urlWithParams];
        }
    }

    NSMutableDictionary* headers = [[[NSMutableDictionary alloc] init] autorelease];
    [headers setValue:contentType forKey:@"Content-Type"];
    [headers setValue:mimeType forKey:@"Accept"];
    [headers setValue:@"no-cache" forKey:@"Cache-Control"];
    [headers setValue:@"no-cache" forKey:@"Pragma"];
    [headers setValue:@"close" forKey:@"Connection"]; // Avoid HTTP 1.1 "keep alive" for the connection

    NSMutableURLRequest* request = [NSMutableURLRequest requestWithURL:finalURL
                                                           cachePolicy:NSURLRequestUseProtocolCachePolicy
                                                       timeoutInterval:60.0];
    [request setHTTPMethod:verb];
    [request setAllHTTPHeaderFields:headers];
	
    if (parameters != nil)
    {
        [request setHTTPBody:body];
    }
    
	[params release];
    [self startConnection:request];
}

- (void)uploadData:(NSData *)data toURL:(NSURL *)url
{
    // File upload code adapted from http://www.cocoadev.com/index.pl?HTTPFileUpload
    // and http://www.cocoadev.com/index.pl?HTTPFileUploadSample

    NSString* stringBoundary = [NSString stringWithString:@"0xKhTmLbOuNdArY"];
    
    NSMutableDictionary* headers = [[[NSMutableDictionary alloc] init] autorelease];
    [headers setValue:@"no-cache" forKey:@"Cache-Control"];
    [headers setValue:@"no-cache" forKey:@"Pragma"];
    [headers setValue:[NSString stringWithFormat:@"multipart/form-data; boundary=%@", stringBoundary] forKey:@"Content-Type"];
    
    NSMutableURLRequest* request = [NSMutableURLRequest requestWithURL:url
                                                           cachePolicy:NSURLRequestUseProtocolCachePolicy
                                                       timeoutInterval:60.0];
    [request setHTTPMethod:kHttpVerbPOST];
    [request setAllHTTPHeaderFields:headers];
    
    NSMutableData* postData = [NSMutableData dataWithCapacity:[data length] + 512];
    [postData appendData:[[NSString stringWithFormat:@"--%@\r\n", stringBoundary] dataUsingEncoding:NSUTF8StringEncoding]];
    [postData appendData:[@"Content-Disposition: form-data; name=\"image\"; filename=\"test.bin\"\r\n\r\n" 
                          dataUsingEncoding:NSUTF8StringEncoding]];
    [postData appendData:data];
    [postData appendData:[[NSString stringWithFormat:@"\r\n--%@--\r\n", stringBoundary] dataUsingEncoding:NSUTF8StringEncoding]];
    [request setHTTPBody:postData];
    
    [self startConnection:request];
}

- (void)cancelConnection
{
    [conn cancel];
    [conn release];
    conn = nil;
}

- (NSDictionary*) responseAsPropertyList
{
    NSString* errorStr = nil;
    NSPropertyListFormat format;
    NSDictionary* propertyList = [NSPropertyListSerialization propertyListFromData:receivedData
                                                                  mutabilityOption:NSPropertyListImmutable
                                                                            format:&format
                                                                  errorDescription:&errorStr];
    [errorStr release];
    return propertyList;
}

- (NSString*) responseAsText
{
    return [[[NSString alloc] initWithData:receivedData 
                                 encoding:NSUTF8StringEncoding] autorelease];
}

- (NSObject*) responseAsJson
{
	SBJsonParser* jsonParser = [[[SBJsonParser alloc] init] autorelease];
	return [jsonParser objectWithString:[self responseAsText]];
}


#pragma mark -
#pragma mark Private methods

- (void) startConnection:(NSURLRequest*)request
{
    if (asynchronous)
    {
        [self cancelConnection];
        conn = [[NSURLConnection alloc] initWithRequest:request
                                               delegate:self
                                       startImmediately:NO];
        
        if (!conn)
        {
            if ([self.delegate respondsToSelector:@selector(wrapper:didFailWithError:)])
            {
                NSMutableDictionary* info = [NSMutableDictionary dictionaryWithObject:[request URL] forKey:NSErrorFailingURLStringKey];
                [info setObject:@"Could not open connection" forKey:NSLocalizedDescriptionKey];
                NSError* error = [NSError errorWithDomain:@"Wrapper" code:1 userInfo:info];
                [self.delegate wrapper:self didFailWithError:error];
            }
        }
		else {
			[conn scheduleInRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];
			[conn start];
			
//			NSPort *aPort = [NSPort port];
//			[[NSRunLoop currentRunLoop] addPort:aPort 
//										forMode:NSDefaultRunLoopMode];
//			
//			// Run the runLoop for a few seconds to give the connection request a chance
//			[[NSRunLoop currentRunLoop] runUntilDate:[NSDate dateWithTimeIntervalSinceNow:10]];
		}
    }
    else
    {
        NSURLResponse* response = [[NSURLResponse alloc] init];
        NSError* error = [[NSError alloc] init];
        NSData* data = [NSURLConnection sendSynchronousRequest:request returningResponse:&response error:&error];
        [receivedData setData:data];
        [response release];
        response = nil;
        [error release];
        error = nil;
    }
}

#pragma mark -
#pragma mark NSURLConnection delegate methods

- (void) connection:(NSURLConnection*)connection didReceiveAuthenticationChallenge:(NSURLAuthenticationChallenge*)challenge
{
    NSInteger count = [challenge previousFailureCount];
    if (count == 0)
    {
        NSURLCredential* credential = [NSURLCredential credentialWithUser:username
                                                                  password:password
                                                               persistence:NSURLCredentialPersistenceNone];
        [[challenge sender] useCredential:credential 
               forAuthenticationChallenge:challenge];
    }
    else
    {
        [[challenge sender] cancelAuthenticationChallenge:challenge];
        if ([self.delegate respondsToSelector:@selector(wrapperHasBadCredentials:)])
        {
            [self.delegate wrapperHasBadCredentials:self];
        }
    }
}

- (void) connection:(NSURLConnection*)connection didReceiveResponse:(NSURLResponse*)response
{
    NSHTTPURLResponse* httpResponse = (NSHTTPURLResponse*)response;
	
    self.lastStatusCode = [httpResponse statusCode];
    switch (self.lastStatusCode)
    {
        case 201:
        {
            NSString* url = [[httpResponse allHeaderFields] objectForKey:@"Location"];
            if ([self.delegate respondsToSelector:@selector(wrapper:didCreateResourceAtURL:)])
            {
                [self.delegate wrapper:self didCreateResourceAtURL:url];
            }
			// fall through
        }
            
        // Here you could add more status code handling... for example 404 (not found),
        // 204 (after a PUT or a DELETE), 500 (server error), etc… with the
        // corresponding delegate methods called as required.
        
        case 200:
        default:
        {
            if ([self.delegate respondsToSelector:@selector(wrapper:didReceiveStatusCode:)])
            {
                [self.delegate wrapper:self didReceiveStatusCode:self.lastStatusCode];
            }
            break;
        }
    }
    [receivedData setLength:0];
}

- (void) connection:(NSURLConnection*)connection didReceiveData:(NSData*)data
{
    [receivedData appendData:data];
}

- (void) connection:(NSURLConnection*)connection didFailWithError:(NSError*)error
{
    [self cancelConnection];
    if ([self.delegate respondsToSelector:@selector(wrapper:didFailWithError:)])
    {
        [self.delegate wrapper:self didFailWithError:error];
    }
	
	// Send the next item in the queue, if any
	[self.callQueue dequeue];
	[self sendNextRequest];
}

- (void) connectionDidFinishLoading:(NSURLConnection*)connection
{
    [self cancelConnection];
    if ([self.delegate respondsToSelector:@selector(wrapper:didRetrieveData:)])
    {
        [self.delegate wrapper:self didRetrieveData:receivedData];
    }
	
	
	// Send the next item in the queue, if any
	[self.callQueue dequeue];
	[self sendNextRequest];
}

@end
