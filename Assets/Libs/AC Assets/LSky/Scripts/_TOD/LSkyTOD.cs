

///////////////////////////
/// Simple Time of day. ///
///////////////////////////


using System;
using UnityEngine;


namespace AC.LSky
{

	[AddComponentMenu ("AC/LSky/Time Of Day")]
	[ExecuteInEditMode]	public class LSkyTOD : LSkyTime
	{

		private LSky m_SkyManager = null;
		private Transform m_Transform = null;

		public    bool  useSystemTime;
		[Range(-12f, 12f)] public int UTC = 0;

		public float Timeline{ get{return timeline + UTC; } }

		public float EVALUATE_TIME_BY_TIMELINE{ get { return timeline/(k_HoursPerDay); }}
		//-----------------------------------------------------------------------------------------

	

		[LSkyFloatAttribute(-180f, 180f, 0.0f, 0.0f, 1.0f, 360f, DefautlColors.yellow)]
		public LSkyFloat equatorPos = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.0f,
			curve        = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f),
			evaluateTime = 0.0f

		};

		[Range(-180f, 180f)] public float orientation = 0.0f;

		//-----------------------------------------------------------------------------------------

		[Tooltip("Rotate moon in the opposite sun direction")]
		public bool autoRotateMoon;

		[LSkyFloatAttribute(-180f, 180f, 0.0f, 0.0f, 1.0f, 360f, DefautlColors.yellow)]
		public LSkyFloat moonXRot = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.0f,
			curve        = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f),
			evaluateTime = 0.0f

		};


		[LSkyFloatAttribute(-180f, 180f, 0.0f, 0.0f, 1.0f, 360f, DefautlColors.yellow)]
		public LSkyFloat moonYRot = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.0f,
			curve        = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f),
			evaluateTime = 0.0f

		};

		public float XRot{ get{ return Timeline * (360f / k_HoursPerDay); } }

		public Quaternion SunRotation
		{

			get 
			{

				equatorPos.evaluateTime = EVALUATE_TIME_BY_TIMELINE;


				float x = XRot - 90f;
				float y = equatorPos.OutputValue;

				return Quaternion.Euler(x, 0, 0) * Quaternion.Euler(0, y, 0);
			}
		}
		//-----------------------------------------------------------------------------------------


		public Quaternion MoonRotation
		{

			get 
			{

				moonXRot.evaluateTime = EVALUATE_TIME_BY_TIMELINE;
				moonYRot.evaluateTime = EVALUATE_TIME_BY_TIMELINE;

				float x = moonXRot.OutputValue + 90;
				float y = moonYRot.OutputValue;

				return (Quaternion.Euler(x, 0, 0) * Quaternion.Euler(0, y, 0));
			}
		}

		public Quaternion MoonRotationOpposiveSun
		{


			get 
			{

				float x = XRot - 270f;
				float y = -equatorPos.OutputValue;
				return Quaternion.Euler (x, 0, 0) * Quaternion.Euler (0, y, 0);

			}

		}

		//-----------------------------------------------------------------------------------------

	//	public DateTime SystemDateTime{ get{ return DateTime.Now; } }

		public int CurrentHour{ get{ return (int)Mathf.Floor(timeline); } }
		public int CurrentMinute{ get{ return (int)Mathf.Floor( (timeline - CurrentHour) * 60); } }
		//-----------------------------------------------------------------------------------------


		void Update()
		{

			if(!CheckComponents())
			{
				m_Transform  = this.transform;
				m_SkyManager = GetComponent<LSky>();
				return;
			}
				
			ProgressTime(useSystemTime);

			m_SkyManager.SetSunLightLocalRotation(SunRotation);
			m_SkyManager.SetMoonLightLocalRotation(autoRotateMoon ? MoonRotationOpposiveSun : MoonRotation );

			m_Transform.localEulerAngles = new Vector3(0.0f, orientation, 0.0f);

		}


		Vector2 ComputeSunPos(DateTime dateTime, float lat, float lon)
		{


			dateTime = dateTime.ToUniversalTime();

			float julianDate = 367 * dateTime.Year -  (int)((7.0f / 4.0f) * (dateTime.Year + (int)((dateTime.Month + 9.0f) / 12.0f))) +  (int)((275.0f * dateTime.Month) / 9.0f) +  dateTime.Day - 730531.5f;  

			float julianCenturies = julianDate / 36525.0f;  

			// Sidereal Time  
			float siderealTimeHours = 6.6974f + 2400.0513f * julianCenturies;  

			float siderealTimeUT = siderealTimeHours +  (366.2422f / 365.2422f) * (float)dateTime.TimeOfDay.TotalHours;  

			float siderealTime = siderealTimeUT * 15 + lon;  

			// Refine to number of days (fractional) to specific time.  
			julianDate += (float)dateTime.TimeOfDay.TotalHours / 24.0f;  
			julianCenturies = julianDate / 36525.0f;  

			// Solar Coordinates  
			float meanLongitude = CorrectAngle(Mathf.Deg2Rad *  (280.466f + 36000.77f * julianCenturies));  

			float meanAnomaly = CorrectAngle(Mathf.Deg2Rad *  
				(357.529f + 35999.05f * julianCenturies));  

			float equationOfCenter = Mathf.Deg2Rad * ((1.915f - 0.005f * julianCenturies) *   
				Mathf.Sin(meanAnomaly) + 0.02f * Mathf.Sin(2 * meanAnomaly));  

			float elipticalLongitude =  
				CorrectAngle(meanLongitude + equationOfCenter);  

			float obliquity = (23.439f - 0.013f * julianCenturies) * Mathf.Deg2Rad;  

			// Right Ascension  
			float rightAscension = Mathf.Atan2(  
				Mathf.Cos(obliquity) * Mathf.Sin(elipticalLongitude),  
				Mathf.Cos(elipticalLongitude));  

			float declination = Mathf.Asin(  
				Mathf.Sin(rightAscension) * Mathf.Sin(obliquity));  

			// Horizontal Coordinates  
			float hourAngle = CorrectAngle(siderealTime * Mathf.Deg2Rad) - rightAscension;  

			if (hourAngle > Mathf.PI)  
			{  
				hourAngle -= 2 * Mathf.PI;  
			}  

			float altitude = Mathf.Asin(Mathf.Sin(lat * Mathf.Deg2Rad) *  
				Mathf.Sin(declination) + Mathf.Cos(lat* Mathf.Deg2Rad) *  
				Mathf.Cos(declination) * Mathf.Cos(hourAngle));  

			// Nominator and denominator for calculating Azimuth  
			// angle. Needed to test which quadrant the angle is in.  
			float aziNom = -Mathf.Sin(hourAngle);  
			float aziDenom =  
				Mathf.Tan(declination) * Mathf.Cos(lat * Mathf.Deg2Rad) -  
				Mathf.Sin(lat * Mathf.Deg2Rad) * Mathf.Cos(hourAngle);  

			float azimuth = Mathf.Atan(aziNom / aziDenom);  

			if (aziDenom < 0) // In 2nd or 3rd quadrant  
			{  
				azimuth += Mathf.PI;  
			}  
			else if (aziNom < 0) // In 4th quadrant  
			{  
				azimuth += 2 * Mathf.PI;  
			}  

			return new Vector2() 
			{
				x = altitude * Mathf.Rad2Deg,
				y = azimuth * Mathf.Rad2Deg
			};
		}


		float CorrectAngle(float angleInRadians)  
		{  
			if (angleInRadians < 0)  
			{  
				return 2 * Mathf.PI - (Mathf.Abs(angleInRadians) % (2 * Mathf.PI));  
			}  
			else if (angleInRadians > 2 * Mathf.PI)  
			{  
				return angleInRadians % (2 * Mathf.PI);  
			}  
			else  
			{  
				return angleInRadians;  
			}  
		}  

		bool CheckComponents()
		{


			if(m_Transform == null)
				return false;

			if(m_SkyManager == null)
				return false;

			if(!m_SkyManager.IsReady)
				return false;

			return true;
		}
			


	}
}