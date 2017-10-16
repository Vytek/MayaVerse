

////////////////////////////////
/// Color/Gradient structure ///
////////////////////////////////

using System;
using UnityEngine;


namespace AC.LSky
{


	[Serializable] public struct LSkyColor
	{

		public LSkyColorType     colorType;     // Input color type. 
		public Color             inputColor;
		public Gradient          gradient;
		public float             evaluateTime;

		public Color OutputColor 
		{
			get{ return (colorType == LSkyColorType.Gradient) ? gradient.Evaluate(evaluateTime) : inputColor; }
		}

		public LSkyColor(LSkyColorType colorType, Color inputColor, Gradient gradient, float evaluateTime) 
		{

			this.colorType    = colorType;
			this.inputColor   = inputColor;
			this.gradient     = gradient;
			this.evaluateTime = evaluateTime;
		}

	}
}
