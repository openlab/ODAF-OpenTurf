/*
 *  Constants.h
 *  VanGuide
 *
 *  Created by shazron on 10-02-14.
 *  Copyright 2010 Nitobi Software Inc.. All rights reserved.
 *
 */

/* keys */

// CHANGEME: Add your Twitter App consumer key
#define kTwitterAppConsumerKey				@"ryS5RtMKdb5nlCsho1o9A"
// CHANGEME: Add your Twitter App consumer secret
#define kTwitterAppConsumerSecret			@"3T2S0xzSSnDvVcVx3Y3T5t51srnk7wNzRBm1D119C5k"

/* limits */

#define kSummaryNameMinChars				8
#define kTweetMessageMaxChars				140
#define kCommentsPageSize					15
#define kMapDetailsCommentsPageSize			2

#define kMapInitialLatitudeSpanMetres		5000
#define kMapInitialLongitudeSpanMetres		10000

/* ActionSheet Tags */

#define kActionSheetNoFiltersSelected		1
#define kActionSheetUserWillAuthenticate	2
#define kActionSheetUseSavedCredentials		3
#define kTableViewCellSpinner				100

/* View Tags */

#define kViewControllerFilters				200


/* General */

#define kYourLandmarksFilter				@"YourLandmarksFilter"
// CHANGEME: Add your Bing Maps Dev Key
#define BING_MAPS_DEV_KEY					@"Av2ZW7NluP8HOWXWxyZjqE60XGCfnwQwe-HDmGb7ogLLrgxXPrmi1G8vWMBjke3Y"

#define kListTerminationText				@"•"
#define kLoadingText						@"Loading…"
#define kDownloadingText					@"Downloading…"

#define kInitializingText					@"Initializing…"
#define kTweetingText						@"Tweeting…"
#define kShorteningUrlText					@"Shortening Url…"

#define kAuthenticatingText					@"Authenticating…"
#define kAddingText							@"Adding…"
#define kVerifyingText						@"Verifying…"
#define kTaggingText						@"Tagging…"
#define kRatingText							@"Rating…"
#define kPostingText						@"Posting…"
#define kLoadingSourcesText					@"Loading sources…"
#define kSourcesLoadedText					@"Sources loaded…"
#define kMoreText							@"more…"

#define kCouldntSignYouInText				@"Couldn't sign you in."
#define kTwitterText						@"Twitter"
#define kErrorText							@"Error"
#define kWebBrowserErrorText				@"Web Browser Error"

#define kPostText							@"Post"
#define kTwitterErrorText					@"Twitter Error"
#define kRateText							@"Rate"
#define kTagText							@"Tag"
#define kAddText							@"Add"
#define kAddLandmarkText					@"Add Landmark"
#define kTweetText							@"Tweet"

#define kMonthsText							@"months"
#define kMonthText							@"month"
#define kYearsText							@"years"
#define kYearText							@"year"
#define kCloseText							@"Close"

#define kYourLandmarksText					@"Your Landmarks"
#define kAddNewLandmarkText					@"Add New Landmark"
#define kShakeToRemoveText					@"shake to remove"

#define kNoneText							@"None"
#define kTagsText							@"Tags"
#define kAddCommentText						@"Add Comment"
#define kNoCommentsText						@"No comments"
#define kCommentsText						@"Comments"
#define kCommentText						@"Comment"
#define kDetailsText						@"Details"

#define kSocialLandmarksText				@"Social Landmarks"
#define kPointDataText						@"Point Data"
#define kRegionDataText						@"Region Data"
#define kSocialDataText						@"Social Data"

#define kPostCommentText					@"Post Comment"
#define kAddRatingText						@"Add Rating"
#define kYouveAlreadyRatedThisLandmarkText	@"You've already rated this landmark."
#define kAddTagsText						@"Add Tags"
#define kTweetThisText						@"Tweet This"
#define kCheckOutThisVanGuideLocationText	@"Check out this VanGuide location:"

#define kNRatingsText						@"%ld ratings"
#define kNRatingText						@"%ld rating"
#define kNoText								@"No"
#define kYesText							@"Yes"

#define kSavedTwitterLoginText				@"Saved Twitter Login"
#define kUseSavedCredentialsText			@"Do you want to use the saved credentials for '%@'?"
#define kSignInText							@"Sign In"

/* Pin names */

#define kPinAddedPoint						@"pin2.png"
#define kPinNewPoint						@"pin.png"
#define kPinCurrentLocation					@"currentlocation.png"
#define kPinCellImagePlaceholder			@"placeholder.png"

/* Notifications */

#define ODNetworkReachabilityChanged		@"ODNetworkReachabilityChanged"

#define ODRESTAuthorizationRequired			@"ODRESTAuthorizationRequired"
#define ODRESTSummaryRequired				@"ODRESTSummaryRequired"

#define ODRESTAuthenticateSuccess			@"ODRESTAuthenticateSuccess"
#define ODRESTAuthenticateFail				@"ODRESTAuthenticateFail"
#define ODRESTCommentAdded					@"ODRESTCommentAdded"
#define ODRESTRatingAdded					@"ODRESTRatingAdded"
#define ODRESTTagAdded						@"ODRESTTagAdded"
#define ODRESTNewMarkerAdded				@"ODRESTNewMarkerAdded"

#define ODRESTSummaryAddSuccess				@"ODRESTSummaryAddSuccess"
#define ODRESTSummaryAddFail				@"ODRESTSummaryAddFail"
#define ODRESTUserSummaryAddSuccess			@"ODRESTUserSummaryAddSuccess"
#define ODRESTUserSummaryAddFail			@"ODRESTUserSummaryAddFail"
#define ODRESTTweetPosted					@"ODRESTTweetPosted"

#define ODYourLandmarksNeedsUpdate			@"ODYourLandmarksNeedsUpdate"
#define ODNoFiltersSelected					@"ODNoFiltersSelected"
#define ODRefreshFilterList					@"ODRefreshFilterList"

#define ODAddMapAnnotation					@"ODAddMapAnnotation"
#define ODAddRegionAnnotation				@"ODAddRegionAnnotation"

#define ODMapViewRegionChanged				@"ODMapViewRegionChanged"
#define ODMapDataLoaded						@"ODMapDataLoaded"
#define ODMapDataFiltered					@"ODMapDataFiltered"

#define ODShowRegionPicker					@"ODShowRegionPicker"
#define ODRegionDataChanged					@"ODDataRegionChanged"
#define ODRegionDataCleared					@"ODDataRegionCleared"
#define ODCurrentRegionData					@"ODCurrentRegionData"

#define ODRESTxAuthSuccess					@"ODRESTxAuthSuccess"
#define ODRESTxAuthFail						@"ODRESTxAuthFail"

#define ODViewComment						@"ODViewComment"
#define ODAddPointForViewComment			@"ODAddPointForViewComment"

/* Preferences keys */

#define kPreferencesKeySaveTwitterLogin		@"save_twitter_login_preference"
#define kPreferencesKeyTweetThis			@"tweet_this_preference"

/* General plist keys */

#define kServiceUserId						@"user_id"
#define kUserId								@"Id"

#define kUrlParamSummaryId					@"SummaryId"
#define kUrlParamCommentId					@"CommentId"

/* Configuration plist Keys */

#define kConfigKeyReachableUrl				@"ReachableUrl"
#define kConfigKeyDevMode					@"DevMode"
#define kConfigKeyDevSettings				@"DevSettings"
#define kConfigKeyPointDataSourcesFile		@"PointDataSourcesFile"
#define kConfigKeyRegionDataSourcesFile		@"RegionDataSourcesFile"
#define kConfigKeyCommunityDataSourcesFile	@"CommunityDataSourcesFile"
#define kConfigKeyWebAppUrl					@"WebAppUrl"
#define kConfigKeyClientAppId				@"ClientAppId"
#define kConfigKeyTintColourRGB				@"TintColourRGB"
#define kConfigKeyUseMapKit					@"UseMapKit"
#define kConfigKeyLoadRemoteSources			@"LoadRemoteSources"

#define kConfigKeyMapSettings				@"MapSettings"
#define kConfigKeyMapInitialLatitude		@"MapInitialLatitude"
#define kConfigKeyMapInitialLongitude		@"MapInitialLongitude"
#define kConfigKeyMapInitialZoom			@"MapInitialZoom"
#define kConfigKeyMapCurrentLocationZoom	@"MapCurrentLocationZoom"
#define kConfigKeyMapViewCommentZoom		@"MapViewCommentZoom"

#define kConfigKeyButtonGradientLowRGB		@"ButtonColourGradientLowRGB"
#define kConfigKeyButtonGradientHighRGB		@"ButtonColourGradientHighRGB"

/* Read-only Plists */

#define kPlistConfiguration					@"Configuration"
#define kPlistIconMap						@"icon_map"

/* Cache Plists */

#define kPlistUserObjectTemp				@"UserObjectTemp.plist"
#define kPlistUserCredentialsCache			@"UserCredentialsCache.plist"
#define kPlistFilter						@"Filters.plist"
#define kPlistRatingsCache					@"RatingsCache.plist"

/* Table Sections */

#define POINT_DATA_SECTION					0
#define SOCIAL_DATA_SECTION					1
#define REGION_DATA_SECTION					2
