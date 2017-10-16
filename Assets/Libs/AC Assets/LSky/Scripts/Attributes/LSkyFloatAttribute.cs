

using System;
using UnityEngine;


namespace AC.LSky
{

	[Serializable] public class LSkyFloatAttribute : PropertyAttribute 
	{

		// Value range.
		public float minValue;
		public float maxValue;
		//------------------------------------------

		// Curve rect.
		public float timeStart;
		public float valueStart;
		public float timeEnd;
		public float valueEnd;
		//------------------------------------------

		public DefautlColors curveColor; 
		//------------------------------------------

		public LSkyFloatAttribute(float minValue, float maxValue, float timeStart, float valueStart, float timeEnd, float valueEnd, DefautlColors curveColor)
		{
			this.minValue   = minValue;
			this.maxValue   = maxValue;
			this.timeStart  = timeStart;
			this.valueStart = valueStart;
			this.timeEnd    = timeEnd;
			this.valueEnd   = valueEnd;
			this.curveColor = curveColor;
		}

		public Color GetCurveColor()
		{

			Color col = new Color();

			switch (curveColor) 
			{

				case DefautlColors.red:     col = Color.red;     break;
				case DefautlColors.green:   col = Color.green;   break;
				case DefautlColors.blue:    col = Color.blue;    break;
				case DefautlColors.cyan:    col = Color.cyan;    break;
				case DefautlColors.mangeta: col = Color.magenta; break;
				case DefautlColors.yellow:  col = Color.yellow;  break;
				case DefautlColors.white:   col = Color.white;   break;
				case DefautlColors.gray:    col = Color.gray;    break;
				case DefautlColors.grey:    col = Color.grey;    break;
				case DefautlColors.black:   col = Color.black;   break;
				case DefautlColors.clear:   col = Color.clear;   break;
			}

			return col;
		}
	}

}