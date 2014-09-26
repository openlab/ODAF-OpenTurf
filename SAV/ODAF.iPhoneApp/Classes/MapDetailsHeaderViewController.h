//
//  MapDetailsHeaderViewController.h
//  VanGuide
//
//  Created by shazron on 10-02-04.
//  Copyright 2010 Nitobi Software Inc.. All rights reserved.
//

#import <UIKit/UIKit.h>

@protocol MapDetailsHeaderDelegate <NSObject>

- (void) nameLabelTouched;
- (void) descriptionLabelTouched;
 
@end


@interface MapDetailsHeaderViewController : UIViewController <MapDetailsHeaderDelegate> {
	IBOutlet UILabel* nameLabel;
	IBOutlet UILabel* descriptionLabel;
	IBOutlet UIImageView* headerImage;
	
@private
	NSString* name;
	NSString* description;
	NSString* image;
	CGRect nameLabelFrame;
	CGRect descriptionLabelFrame;
}

@property (nonatomic, retain) UILabel* nameLabel;
@property (nonatomic, retain) UILabel* descriptionLabel;
@property (nonatomic, retain) UIImageView* headerImage;

@property (nonatomic, retain) NSString* name;
@property (nonatomic, retain) NSString* description;
@property (nonatomic, retain) NSString* image;

@property (nonatomic, assign) CGRect nameLabelFrame;
@property (nonatomic, assign) CGRect descriptionLabelFrame;


- (id) initWithName:(NSString*)text description:(NSString*)description andImage:(NSString*)image;

@end
