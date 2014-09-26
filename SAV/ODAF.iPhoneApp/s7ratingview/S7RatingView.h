//
//  S7RatingView.h
//  S7Touch
//
//  Created by Aleks Nesterow on 7/21/09.
//  aleks.nesterow@gmail.com
//  
//  Copyright 2009 7touch Group, Inc. All rights reserved.
//  
//  
//  Purpose
//  Displays a 5-star rating with ability to set user rating.
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

#import <Foundation/Foundation.h>

#define STATE_NONSELECTED	@"state-nonselected"
#define STATE_SELECTED		@"state-selected"
#define STATE_HALFSELECTED	@"state-halfselected"
#define STATE_HOT			@"state-hot"
#define STATE_HIGHLIGHTED	@"state-highlighted"

@class S7RatingView;

@protocol S7RatingDelegate

- (void)ratingView:(S7RatingView *)ratingView didChangeUserRatingFrom:(int)previousUserRating to:(int)userRating;
- (void)ratingView:(S7RatingView *)ratingView didChangeRatingFrom:(float)previousRating to:(float)rating;

@end

@interface S7RatingView : UIView {
	
	float					rating;
	int						userRating; /* User rating is supposed to be integer. */
	BOOL					highlighted;
	NSArray					*starViews;
	NSMutableDictionary		*stateImageDictionary;
	CGSize					starPlaceSize;
	CGPoint					margin;
	CGFloat					starSpacing;
	
	id<S7RatingDelegate>	delegate;
}

/* Use 0 to specify that there is no rate yet. All the other values will be aligned to [1-5] range. */
@property (nonatomic, assign) float rating;
/* Use 0 to specify that user decided to not leave rating just in the process.
 * All the other values will be aligned to [1-5] range.
 * In fact user rating - if set, i.e. 1+ - will always override rating.
 */
@property (nonatomic, assign) int userRating;
@property (nonatomic, assign) CGSize starPlaceSize;
@property (nonatomic, assign) CGPoint margin;
@property (nonatomic, assign) CGFloat starSpacing;
@property (nonatomic, assign, getter=isHighlighted) BOOL highlighted;

@property (nonatomic, assign) IBOutlet id<S7RatingDelegate> delegate;

- (void)setStarImage:(UIImage *)image forState:(NSString *)state;

@end
