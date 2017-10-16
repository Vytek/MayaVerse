

/////////////////////////////
/// Float/Curve structure ///
/////////////////////////////

using System;
using UnityEngine;


namespace AC.LSky
{


	[Serializable] public struct LSkyFloat
	{


		public LSkyValueType     valueType;     // Input value type.
		public float             inputValue;
		public AnimationCurve    curve; 
		public float             evaluateTime;

		public float OutputValue
		{
			get{ return valueType == LSkyValueType.Curve ? curve.Evaluate(evaluateTime) : inputValue; }
		}

		public LSkyFloat(LSkyValueType valueType, float inputValue, AnimationCurve curve, float evaluateTime)
		{

			this.valueType    = valueType;
			this.inputValue   = inputValue;
			this.curve        = curve;
			this.evaluateTime = evaluateTime;
		}


	}
}
