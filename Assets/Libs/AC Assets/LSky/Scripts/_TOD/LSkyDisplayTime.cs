
using UnityEngine;
using UnityEngine.UI;

namespace AC.LSky
{

	public class LSkyDisplayTime : MonoBehaviour 
	{



		private LSkyTOD TOD     = null;
		public Text timeText; // UI text component.


		void Start()
		{
			TOD = GetComponent<LSkyTOD> ();
		}


		void Update()
		{

			if (timeText != null || TOD != null) 
			{
				timeText.text = GetTimeString; 
			}
		}


		public string GetTimeString
		{
			get
			{
				string h = TOD.CurrentHour < 10 ? "0" + TOD.CurrentHour.ToString () : TOD.CurrentHour.ToString ();
				string m = TOD.CurrentMinute < 10 ? "0" + TOD.CurrentMinute.ToString () : TOD.CurrentMinute.ToString ();
				//----------------------------------------------------------------------------------------------------------------

				return h + ":" + m;
			}
		}


	}
}
