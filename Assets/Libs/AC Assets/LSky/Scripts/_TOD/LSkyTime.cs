
/////////////
/// Time  ///
/////////////


using System;
using UnityEngine;


namespace AC.LSky
{

	public abstract class LSkyTime : MonoBehaviour 
	{


		public    bool  playTime      = true; // Progress time.
		public    float dayInSeconds  = 900;  // 60*15 = 900 (15 minutes).
		protected const int k_HoursPerDay = 24;   
		//-------------------------------------------------------------------

		[Range(0.0f, 24f)] public float timeline = 7.0f;
		//-------------------------------------------------------------------


		protected void ProgressTime()
		{
			timeline = Mathf.Repeat (timeline, k_HoursPerDay);

			// Add time.
			if (playTime && Application.isPlaying && dayInSeconds != 0)
			{
				timeline += (Time.deltaTime / dayInSeconds) * k_HoursPerDay; 
			}
		}


		protected void ProgressTime(bool useSystemTime)
		{


			if(useSystemTime) 
			{
				if(playTime) 
				{
					DateTime dateTime = DateTime.Now;
					timeline = (float)dateTime.Hour + (float)dateTime.Minute / 60;
				}
			} 
			else 
			{
				timeline = Mathf.Repeat (timeline, k_HoursPerDay);

				// Add time.
				if (playTime && Application.isPlaying && dayInSeconds != 0)
				{
					timeline += (Time.deltaTime / dayInSeconds) * k_HoursPerDay; 
				}
			}

		}
		//-------------------------------------------------------------------
	}
}