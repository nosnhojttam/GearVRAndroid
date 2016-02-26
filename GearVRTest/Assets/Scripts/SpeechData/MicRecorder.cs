//Copyright Frostweep Games,LLC
//2015(c), Unity Speech Recognition using Google API
//MicRecorder Script

using UnityEngine;

namespace SpeechRecognition
{
    public class MicRecorder : MonoBehaviour
    {

        private bool isRecord = false;
        private bool isPlayedRec = false;
        private int sampleRate = 16000;

        public bool AutoConvertAudio = false;
        public bool isLoopingRecord = true;
        public int RecordTimeSec = 1;

        private AudioClip RecordClip;

        public SendToGoogle sendToGoogle;

        //for loudness
        private float MicLoudness;
        private int quietCounter = 0;
        public SpeechToText kill;

        void Update()
        {
            if (isRecord)
            {

                MicLoudness = LevelMax()* 100;// This gets loudness from 0 to 100

                Debug.Log(MicLoudness);

                if (MicLoudness < 35)
                {
                    Debug.Log ("QuietCounter="+quietCounter);
                    quietCounter++;
                    if (quietCounter > 45)
                    {
                        kill.TryStopRecord();
                        quietCounter = 0;

                    }

                }
                else
                {
                    quietCounter = 0;
                }

            }
        } //Some work
        public void StartRecord(string MicDeviceName)
        {
            RecordClip = Microphone.Start(MicDeviceName, isLoopingRecord, RecordTimeSec, sampleRate);
            isRecord = true;
            Debug.Log("Recording Started");
        } //Start Mic Record
	
        public AudioClip StopRecord(string MicDeviceName)
        {
            Microphone.End(MicDeviceName);
            Debug.Log("Recording Ended");
            isRecord = false;
            StartCoroutine(sendToGoogle.SendToGoogleAudio(RecordClip));
            return RecordClip;
        } //Stop Mic Record
        public void PlayLastAudioClip()
        {
            if (RecordClip != null)
            {
                GetComponent<AudioSource>().Stop();
                GetComponent<AudioSource>().clip = null;
                GetComponent<AudioSource>().clip = RecordClip;
                GetComponent<AudioSource>().Play();

            }

        }//Play last Recorded audio clip
        //get data from microphone into audioclip
        public float LevelMax()
        {
            float levelMax = 0;
            float[] waveData = new float[sampleRate];
            int micPosition = Microphone.GetPosition(null) - (sampleRate + 1); // null means the first microphone
            //if (micPosition < 0) {
			//	Debug.Log("FUCK");
			//	return 0;
			//}
				RecordClip.GetData(waveData, micPosition);
            // Getting a peak on the last 128 samples
            for (int i = 0; i < sampleRate; i++)
            {
                float wavePeak = waveData[i] * waveData[i];
                if (levelMax < wavePeak)
                {
                    levelMax = wavePeak;
                }
            }
            return levelMax;
        }
    }
}