using UnityEngine;
using System.Collections;
using System;
using System.Linq;

namespace Mumble {
    [RequireComponent(typeof(AudioSource))]
    public class MumbleAudioPlayer : MonoBehaviour {

        public float Gain = 1;
        public string MumbleUserName;
        private MumbleClient _mumbleClient;
        private UInt32 _session;
        private AudioClip _clip;

        const int SampleLengthSeconds = 1;
        const bool isStreamingAudio = true;

        void Start() {
            //print("outout rate " + AudioSettings.outputSampleRate);
        }
        public void Initialize(MumbleClient mumbleClient, UInt32 session, string mumbleusername)
        {
            Debug.Log("Initialized " + session, this);
            _mumbleClient = mumbleClient;
            _session = session;
            Debug.Log("Username: " + mumbleusername);
            this.MumbleUserName = mumbleusername;
        }
        void OnAudioFilterRead(float[] data, int channels)
        {
            //print("On audio read " + _session);
            if (_mumbleClient == null || !_mumbleClient.ConnectionSetupFinished)
                return;

            _mumbleClient.LoadArrayWithVoiceData(_session, data, 0, data.Length);

            //Debug.Log("playing audio with avg: " + data.Average() + " and max " + data.Max());
            if (Gain == 1)
                return;

            for (int i = 0; i < data.Length; i++)
                data[i] = Mathf.Clamp(data[i] * Gain, -1f, 1f);
            //Debug.Log("playing audio with avg: " + data.Average() + " and max " + data.Max());
        }
    }
}
