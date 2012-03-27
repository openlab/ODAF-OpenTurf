#import "ColorfulButton.h"

@implementation ColorfulButton

@synthesize _highColor;
@synthesize _lowColor;
@synthesize gradientLayer;

- (id) initWithFrame:(CGRect)aFrame
{
	if ((self = [super initWithFrame:aFrame]) != nil) {
		[self awakeFromNib];
	}
	
	return self;
}

- (void)awakeFromNib;
{
    gradientLayer = [[CAGradientLayer alloc] init];
    [gradientLayer setBounds:[self bounds]];
    [gradientLayer setPosition:CGPointMake([self bounds].size.width/2, [self bounds].size.height/2)];
    
    [[self layer] insertSublayer:gradientLayer atIndex:0];

    [[self layer] setCornerRadius:8.0f];
    [[self layer] setMasksToBounds:YES];
    [[self layer] setBorderWidth:1.0f];
    
}

- (void)drawRect:(CGRect)rect;
{
    if (_highColor && _lowColor)
    {
        [gradientLayer setColors:[NSArray arrayWithObjects:(id)[_highColor CGColor], (id)[_lowColor CGColor], nil]];
    }
    [super drawRect:rect];
}

- (void)setHighColor:(UIColor*)color;
{
    [self set_highColor:color];
    [[self layer] setNeedsDisplay];
}

- (void)setLowColor:(UIColor*)color;
{
    [self set_lowColor:color];
    [[self layer] setNeedsDisplay];
}

- (void)dealloc {
    [gradientLayer release];
    [super dealloc];
}

@end
