
////////////////////
/// Public enums ///
////////////////////


namespace AC.LSky
{

	// Structures.
	public enum LSkyColorType{ Color, Gradient }
	public enum LSkyValueType{ Value, Curve }


	// Attributes.
	public enum DefautlColors
	{ 
		red,   green,   blue, 
		cyan,  mangeta, yellow,
		white, gray,    grey,  
		black, clear 
	} 

	// Atmosphere.
	public enum LSkyNightColorType{ Simple, Atmospheric }
	public enum LSkyFogMode
	{
		Linear = 1, 
	    Exponential = 2, 
	    ExponentialSquared = 3 
	}
}