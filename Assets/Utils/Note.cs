using UnityEngine;

namespace UnityNotes
{
	[AddComponentMenu("Miscellaneous/Note")]
	public class Note : MonoBehaviour
	{
		[SerializeField]
		string title = "Note title";

		[SerializeField]
		string note = "Your note here!";

		public string TitleText
		{
			get { return title; }
			set { title = value; }
		}

		public string NoteText
		{
			get { return note; }
			set { note = value; }
		}
	}
}