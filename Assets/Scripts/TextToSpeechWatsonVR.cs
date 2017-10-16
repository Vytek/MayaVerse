using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Logging;
#pragma warning disable 0414

public class TextToSpeechWatsonVR : MonoBehaviour {
	TextToSpeech m_TextToSpeech = new TextToSpeech();
	string m_TestString = "Salve! Benvenuto su MayaVerse, il primo mondo in realtà virtuale completamente italiano!";
	string m_ExpressiveText = "<speak version=\"1.0\"><prosody pitch=\"150Hz\">Hello! This is the </prosody><say-as interpret-as=\"letters\">IBM</say-as> Watson <express-as type=\"GoodNews\">Unity</express-as></prosody><say-as interpret-as=\"letters\">SDK</say-as>!</speak>";

	// Use this for initialization
	void Start () {
		m_TextToSpeech.Voice = VoiceType.it_IT_Francesca;
		m_TextToSpeech.ToSpeech(m_TestString, HandleToSpeechCallback);

		//m_TextToSpeech.Voice = VoiceType.en_US_Allison;
		//m_TextToSpeech.ToSpeech(m_ExpressiveText, HandleToSpeechCallback);
	}
	
	void HandleToSpeechCallback (AudioClip clip)
	{
		PlayClip(clip);
	}

	private void PlayClip(AudioClip clip)
	{
		if (Application.isPlaying && clip != null) {
			GameObject audioObject = new GameObject ("AudioObject");
			AudioSource source = audioObject.AddComponent<AudioSource> ();
			source.spatialBlend = 0.0f;
            source.volume = 0.2f;
			source.loop = false;
			source.clip = clip;
			source.Play ();

			GameObject.Destroy (audioObject, clip.length);
		}
	}
}
