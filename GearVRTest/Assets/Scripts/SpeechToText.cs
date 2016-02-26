//Copyright Frostweep Games,LLC
//2015(c), Unity Speech Recognition using Google API
//Example Script

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SpeechRecognition; //plugin namespace

public class SpeechToText : MonoBehaviour {

    private bool LockAll = false;
    private bool isChoosedMic = false;
    private bool isConverted = false;
    private bool isSended = false;
    private bool isGetRequest = false;
    private string MicDeviceName;
    private string[] devices;
    private string[] RequestWords;

    public GameObject[] UI_FirstPage = new GameObject[5];
    public GameObject[] UI_SecondPage = new GameObject[6];

    public MicRecorder micRecoder;
    public SendToGoogle sendToGoogle;

    public Text RecordLogsText;
    public Text DictionaryLogsText;

    private List<string> Commands = new List<string>();

    private AudioClip RecordedClip;

    //For Mic Loudness
    private bool listening = false;
    private GameObject micController;
    private float loudness;
    private int sampleRate = 16000;
    public int sensitivity = 100;
	
	bool wordsReceived = false;

	//To save audio
//	public SavWav savWav;	

    void Start()
    {
        devices = Microphone.devices;
        MicDeviceName = devices[0];
        RecordedClip = Microphone.Start(MicDeviceName, true, 1, sampleRate);
		listening = true;//Triggers loudness to start testing volume in update()

    }//Init Script
	
    public void TryStartRecord()
    {
        if(!string.IsNullOrEmpty(MicDeviceName))
        {
			Microphone.End(MicDeviceName);
            listening = false;
            micRecoder.StartRecord(MicDeviceName);
        }
        else
        {

        }
            
    }//Start Record
    public void TryStopRecord()
    {
        if (!string.IsNullOrEmpty(MicDeviceName))
        {
            micRecoder.StopRecord(MicDeviceName);
			listening = true;
			Debug.Log ("Finished recording");
			RecordedClip = Microphone.Start(MicDeviceName, true, 1, sampleRate);

		}
        if (RecordedClip != null)
            isConverted = false;
    }//Stop Record
     
	private void Update()
    {

        if (listening)
        {
            loudness = LevelMax() * 100;//for volume before speech

            //Debug.Log("Loudness : " + loudness);

            if (loudness > 8)
            {
                Debug.Log("Sent Record");
                TryStartRecord();
            }
        }
			
        if(!string.IsNullOrEmpty(sendToGoogle.GetResponse) && !isGetRequest)
        {
            isGetRequest = true;
            RequestWords = sendToGoogle.GetWords();

            if (RequestWords == null)
                return;

			wordsReceived = true;

			isGetRequest = false;
        }
    } //Update script data

    //Determines loudness from 0 to 1
    private float LevelMax()
    {
        float levelMax = 0;
        float[] waveData = new float[sampleRate];
        //int micPosition = Microphone.GetPosition(null)-(sampleRate+1); // null means the first microphone
        //if (micPosition < 0) return 0;
        RecordedClip.GetData(waveData, 1);
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

	public string getWords(){

		if (wordsReceived ) {
			if(RequestWords.Length > 5)
				return RequestWords [5];
			else {
				Debug.Log (RequestWords);
				return "banana";
			}
			wordsReceived = false;

		} else {
			return "banana";
		}

	}

	public void resetWordsReceived(){
		wordsReceived = false;
	}

	void displayAudioData(){

		float[] samples = new float[RecordedClip.samples * RecordedClip.channels];
		RecordedClip.GetData (samples, 0);
		int i = 0;
		while (i < samples.Length) {
			samples [i] = samples [i] * 0.5F;
			++i;
		}

		//SavWav.Save("myfile", myAudioClip);

	}

}