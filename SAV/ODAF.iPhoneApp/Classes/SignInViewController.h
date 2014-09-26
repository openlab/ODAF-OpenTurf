//
//  SignInViewController.h
//  VanGuide
//
//  Created by shazron on 10-03-05.
//  Copyright 2010 Nitobi Software Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RESTWrapperDelegate.h"

@class ColorfulButton, xAuth, OAuthAccount;

@interface SignInViewController : UIViewController < RESTWrapperDelegate, UIAlertViewDelegate > {
	IBOutlet UITextField* username;
	IBOutlet UITextField* password;
	IBOutlet ColorfulButton* signInButton;
	IBOutlet UIToolbar* toolbar;
	BOOL   noReauth;
	xAuth* twitterAuth;
	OAuthAccount* oauthAccount;
	IBOutlet UIActivityIndicatorView* spinner;
	
}

@property (nonatomic, retain) UITextField* username;
@property (nonatomic, retain) UITextField* password;
@property (nonatomic, retain) ColorfulButton* signInButton;
@property (nonatomic, retain) UIToolbar* toolbar;
@property (nonatomic, retain) xAuth* twitterAuth;
@property (nonatomic, retain) OAuthAccount* oauthAccount;
@property (nonatomic, retain) UIActivityIndicatorView* spinner;
@property (nonatomic, assign) BOOL noReauth;

- (IBAction) signIn;
- (IBAction) close;

@end
