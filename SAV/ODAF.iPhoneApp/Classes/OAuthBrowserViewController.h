//
//  OAuthBrowserViewController.h
//  PhoneGap
//
//

#import <UIKit/UIKit.h>
#import "RESTWrapperDelegate.h"

typedef enum {
	OAuthModeNone,
	RequestAuthToken,
	GetAccessToken,
	Authenticate
} OAuthMode;

@interface GetAccessTokenResponse : NSObject
{
	NSString* oauth_token;
	NSString* oauth_token_secret;
	NSString* oauth_token_expiry;
} 

@property (nonatomic, copy) NSString* oauth_token;
@property (nonatomic, copy) NSString* oauth_token_secret;
@property (nonatomic, copy) NSString* oauth_token_expiry;

@end


@interface RequestAuthTokenResponse : NSObject
{
	NSString* oauth_token;
	NSString* oauth_token_secret;
	NSString* link;
} 

@property (nonatomic, copy) NSString* oauth_token;
@property (nonatomic, copy) NSString* oauth_token_secret;
@property (nonatomic, copy) NSString* link;

@end

@interface OAuthBrowserViewController : UIViewController < UIWebViewDelegate, RESTWrapperDelegate > {
	IBOutlet UIWebView* webView;
	IBOutlet UIActivityIndicatorView* spinner;
	IBOutlet UIToolbar* toolbar;
	RequestAuthTokenResponse* requestAuthTokenResponse;
	GetAccessTokenResponse* getAccessTokenResponse;
	OAuthMode currentMode;
}

@property (nonatomic, retain) UIWebView* webView;
@property (nonatomic, retain) UIActivityIndicatorView* spinner;
@property (nonatomic, retain) UIToolbar* toolbar;
@property (nonatomic, retain) RequestAuthTokenResponse* requestAuthTokenResponse;
@property (nonatomic, retain) GetAccessTokenResponse* getAccessTokenResponse;
@property (nonatomic, assign) OAuthMode currentMode;

- (IBAction) onDoneButtonPress:(id)sender;
- (IBAction) onNewButtonPress:(id)sender;

- (void)loadURL:(NSString*)url;

@end
