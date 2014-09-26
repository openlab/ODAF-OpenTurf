//
//  S7RatingView.m
//  S7Touch
//
//  Created by Aleks Nesterow on 7/21/09.
//  aleks.nesterow@gmail.com
//  
//  Copyright 2009 7touch Group, Inc. All rights reserved.
//  
//
//  Redistribution and use in source and binary forms, with or without modification,
//  are permitted provided that the following conditions are met:
//
//  * Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//  * Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//  * Neither the name of 7touch Group, Inc. nor the names of its contributors
//    may be used to endorse or promote products derived from this software without
//    specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
//  OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
//  AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER
//  OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
//  DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
//  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
//  IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
//  OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//  

#import "S7RatingView.h"
#import "math.h"

#define PROP_RATING			@"rating"

#define STAR_HALFSELECTED	@"star-halfselected.png"
#define STAR_HOT			@"star-hot.png"
#define STAR_NONSELECTED	@"star-nonselected.png"
#define STAR_SELECTED		@"star-selected.png"
#define STAR_HIGHLIGHTED	@"star-highlighted.png"

#define MIN_RATING			1
#define MAX_RATING			5
#define STARS				MAX_RATING - MIN_RATING + 1

#define ALIGN(X)			(MIN(MAX_RATING, MAX(MIN_RATING, X)))

typedef UIImageView			StarView;
typedef UIImageView*		StarViewRef;

@interface S7RatingView (PrivateMethods)

- (NSMutableDictionary *)stateImageDictionary;
- (UIImage *)imageForState:(NSString *)state fromDictionary:(NSDictionary *)stateImageDict defaultName:(NSString *)defaultImageName;
- (void)initializeComponent;
- (int)getRatingFromTouches:(NSSet *)touches withEvent:(UIEvent *)event;
- (void)visualizeCurrentUserRating:(int)currentUserRating;
- (void)visualizeCurrentRating:(float)currentRating;

@end

@implementation S7RatingView

@synthesize delegate, margin, starSpacing;

- (id)initWithFrame:(CGRect)frame {
	
	if (self = [super initWithFrame:frame]) {
		self.clipsToBounds = YES;
		[self initializeComponent];
	}
	
	return self;
}

- (void)drawRect:(CGRect)rect 
{
	self.clipsToBounds = YES;
	[self initializeComponent];
}

- (void)dealloc {
	
	[stateImageDictionary release];
	[starViews release];
	[super dealloc];
}

#pragma mark Properties

@synthesize rating;
- (void)setRating:(float)value {
	
	if (rating != value) {
		
		float previousRating = rating;
		rating = value;
		[self visualizeCurrentRating:value];
		[delegate ratingView:self didChangeRatingFrom:previousRating to:rating];
	}
}

@synthesize userRating;
- (void)setUserRating:(int)value {
	
	int previousUserRating = userRating;
	
	if (!value) {
		if (!userRating /* User hasn't voted yet. */) {
			[self visualizeCurrentRating:self.rating];
		} else {
			[self visualizeCurrentRating:userRating]; /* Visualizing previous user rating. */
		}
	} else {
		/* Align the passed value so that it would fit physical range of 5 stars. */
		userRating = ALIGN(value);
		
		NSMutableDictionary *stateImageDict = [self stateImageDictionary];
		
		for (int i = 0; i < userRating; i++) {
			StarViewRef starView = [starViews objectAtIndex:i];
			starView.image = [self imageForState:STATE_SELECTED fromDictionary:stateImageDict defaultName:STAR_SELECTED];
		}
		
		if (value < starViews.count) {
			/* Need to leave some stars with non-selected images. */
			for (int i = starViews.count - 1; i >= value; i--) {
				StarViewRef starView = [starViews objectAtIndex:i];
				starView.image = [self imageForState:STATE_NONSELECTED fromDictionary:stateImageDict defaultName:STAR_NONSELECTED];
			}
		}
	}
	
	if (previousUserRating != userRating) {
		[delegate ratingView:self didChangeUserRatingFrom:previousUserRating to:userRating];
	}
}

@synthesize starPlaceSize;
- (void)setStarPlaceSize:(CGSize)value {
	
	starPlaceSize = value;
	for (int i = 0; i < STARS; i++) {
		StarViewRef starView = [starViews objectAtIndex:i];
		int width = starPlaceSize.width;
		starView.frame = CGRectMake(i * width, 0, width, starPlaceSize.height); 
	}
}

@synthesize highlighted;
- (void)setHighlighted:(BOOL)value {
	
	for (StarViewRef starView in starViews) {
		starView.highlighted = value;
	}
}

#pragma mark Public Methods

- (void)setStarImage:(UIImage *)image forState:(NSString *)state {
	
	if ([STATE_HIGHLIGHTED isEqualToString:state]) {
		for (StarViewRef starView in starViews) {
			starView.highlightedImage = image;
		}
	} else {
		NSMutableDictionary *stateImageDict = [self stateImageDictionary];
		image = [image retain];
		[stateImageDict setObject:image forKey:state];
		[self visualizeCurrentRating:self.rating];
	}
}

#pragma mark User Interaction

- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event {

	int starsToHighlight = [self getRatingFromTouches:touches withEvent:event];
	[self visualizeCurrentUserRating:starsToHighlight];
}

- (void)touchesMoved:(NSSet *)touches withEvent:(UIEvent *)event {

	int starsToHighlight = [self getRatingFromTouches:touches withEvent:event];
	[self visualizeCurrentUserRating:starsToHighlight];
}

- (void)touchesEnded:(NSSet *)touches withEvent:(UIEvent *)event {
	
	int starsToSelect = [self getRatingFromTouches:touches withEvent:event]; /* Basically this is final user rating. */
	[self setUserRating:starsToSelect];
}

- (void)touchesCancelled:(NSSet *)touches withEvent:(UIEvent *)event {
	
	[self setUserRating:self.rating];
}

- (UIView *)hitTest:(CGPoint)point withEvent:(UIEvent *)event {

	if (self.userInteractionEnabled && [self pointInside:point withEvent:event]) {
		return self; /* Only intercept events if the touch happened inside the view. */
	}
	return [super hitTest:point withEvent:event];
}

- (void)setUserInteractionEnabled:(BOOL)value {
	
	[super setUserInteractionEnabled:value];
	for (StarViewRef starView in starViews) {
		starView.userInteractionEnabled = value;
	}
}

#pragma mark Private Methods

- (NSMutableDictionary *)stateImageDictionary {

	if (!stateImageDictionary) {
		stateImageDictionary = [[NSMutableDictionary alloc] initWithCapacity:4];
	}
	return stateImageDictionary;
}

- (UIImage *)imageForState:(NSString *)state fromDictionary:(NSDictionary *)stateImageDict defaultName:(NSString *)defaultImageName {

	UIImage *result = [stateImageDict objectForKey:state];
	if (!result) {
		result = [UIImage imageNamed:defaultImageName];
	}
	return result;
}

- (void)initializeComponent {
	
	NSMutableArray *aStarViewList = [[NSMutableArray alloc] initWithCapacity:STARS];
	NSDictionary *stateImageDict = [self stateImageDictionary];
	
	for (int i = 0; i < STARS; i++) {
		
		UIImage* image = [self imageForState:STATE_NONSELECTED fromDictionary:stateImageDict defaultName:STAR_NONSELECTED];
		CGSize imageSize = image.size;
		StarViewRef starView = [[StarView alloc] initWithFrame:CGRectMake((i * (imageSize.width + self.starSpacing)) + self.margin.x, self.margin.y, imageSize.width, imageSize.height)];
		starView.clearsContextBeforeDrawing = YES;
		starView.contentMode = UIViewContentModeCenter;
		starView.highlightedImage = [UIImage imageNamed:STAR_HIGHLIGHTED];
		starView.image = image;
		starView.multipleTouchEnabled = YES;
		starView.tag = MIN_RATING + i; /* Associated rating, which is from MIN_RATING to MAX_RATING. */
		[aStarViewList addObject:starView];
		[self addSubview:starView];
		[starView release];
	}
	
	starViews = aStarViewList;
}

- (int)getRatingFromTouches:(NSSet *)touches withEvent:(UIEvent *)event {

	id touche = [touches anyObject];
	
	for (StarViewRef starView in starViews) {
		if ([starView pointInside:[touche locationInView:starView] withEvent:event]) {
			return starView.tag;
		}
	}
	
	return 0;
}

- (void)visualizeCurrentUserRating:(int)currentUserRating {

	NSDictionary *stateImageDict = [self stateImageDictionary];
	
	/* Making red the stars that indicate the current rating. */
	
	for (int i = 0; i < currentUserRating; i++) {
		StarViewRef starView = [starViews objectAtIndex:i];
		starView.image = [self imageForState:STATE_HOT fromDictionary:stateImageDict defaultName:STAR_HOT];
	}
	
	/* Leaving only star borders for the others. */
	
	for (int i = starViews.count - 1; i >= currentUserRating; i--) {
		StarViewRef starView = [starViews objectAtIndex:i];
		starView.image = [self imageForState:STATE_NONSELECTED fromDictionary:stateImageDict defaultName:STAR_NONSELECTED];
	}
}

- (void)visualizeCurrentRating:(float)currentRating {
	
	int counter = 0;
	NSDictionary *stateImageDict = [self stateImageDictionary];
	
	if (currentRating != 0) {
		
		/* Round currentRating to 0.5 stop. */
		
		currentRating = (lroundf(ALIGN(currentRating) * 2)) / 2.0;

		/* First set images for full stars. */
		
		int fullStars = floorf(currentRating);
		for (int i = 0; i < fullStars; i++, counter++) {
			StarViewRef starView = [starViews objectAtIndex:i];
			starView.image = [self imageForState:STATE_SELECTED fromDictionary:stateImageDict defaultName:STAR_SELECTED];
		}
		
		/* Now set images for a half star if any. */
		
		if (currentRating - fullStars > 0) {
			StarViewRef starView = [starViews objectAtIndex:counter++];
			starView.image = [self imageForState:STATE_HALFSELECTED fromDictionary:stateImageDict defaultName:STAR_HALFSELECTED];
		}
	}
	
	/* Leave other stars unselected. */
	
	for (int i = starViews.count - 1; i >= counter; i--) {
		StarViewRef starView = [starViews objectAtIndex:i];
		starView.image = [self imageForState:STATE_NONSELECTED fromDictionary:stateImageDict defaultName:STAR_NONSELECTED];
	}
}

@end
