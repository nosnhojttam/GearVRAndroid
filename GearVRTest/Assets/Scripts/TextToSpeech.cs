//Copyright Frostweep Games,LLC
//2015(c), Unity Speech Recognition using Google API
//Example Script

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SpeechRecognition; //plugin namespace

public class TextToSpeech : MonoBehaviour {

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
    private bool listen = false;
    private GameObject micController;
    private float loudness;
    private int sampleRate = 16000;
    public int sensitivity = 100;

    void Start()
    {
		ChoiseMicPart1 ();
        devices = Microphone.devices;
        MicDeviceName = devices[0];
        RecordedClip = Microphone.Start(MicDeviceName, true, 1, sampleRate);
		listen = true;//Triggers loudness to start testing volume in update()

    }//Init Script
    private void ChoiseMicPart1()//Find Microphone devices
    {
        devices = Microphone.devices;
        int i = 1;
        if (devices.Length > 0)
        {
            foreach (var device in devices)
            {
                UI_FirstPage[0].GetComponent<Text>().text += "\n" + i + ") " + device.ToString();
            }
        }
        else
        {
            LockAll = true;
            UI_FirstPage[0].GetComponent<Text>().text += "Don't Found Microphone Devices!!!";
        }
        
}
    public void ChoiseMicPart2()
    {
        if (!LockAll)
        {
            try
            {
                int t = System.Convert.ToInt32(UI_FirstPage[1].GetComponent<InputField>().text);
                if (t <= devices.Length && t > -1)
                {
                    MicDeviceName = devices[t - 1];
                    UI_FirstPage[3].SetActive(false);
                    UI_FirstPage[4].SetActive(true);

                    isChoosedMic = true;
                }
            }
            catch (UnityException ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }//Choice Mic Device
    public void TryStartRecord()
    {
        if(!string.IsNullOrEmpty(MicDeviceName))
        {
            listen = false;
            micRecoder.StartRecord(MicDeviceName);
            PostLog("Start Record", 0);
        }
        else
        {
            PostLog("MicDeviceName is null or empty", 0);
        }
            
    }//Start Record
    public void TryStopRecord()
    {
        if (!string.IsNullOrEmpty(MicDeviceName))
        {
            listen = true;
            RecordedClip = micRecoder.StopRecord(MicDeviceName);
            PostLog("Stop Record", 0);
        }
        if (RecordedClip != null)
            isConverted = false;
    }//Stop Record
   
    public void TryPlayLastAudioClip()
    {
        PostLog("Try to Play last Audio", 0);
        micRecoder.PlayLastAudioClip();
    }//Play last audio if != null
  
    public void AddCommand(InputField Command)
    {
        if (!Commands.Contains(Command.text) && !string.IsNullOrEmpty(Command.text))
        {
            Commands.Add(Command.text);
            PostLog("Added command: " + Command.text, 1);

            Command.text = "";
        }
        else
            PostLog("Error Add command! Try again!", 1);

    }//Add command to words dictionary
    public void GetCommand(int index, bool All)
    {
        if (All)
        {
            foreach(string str in Commands)
            {
                PostLog("Get command: " + str, 1);
            }
        }
        else
        {
            if (index < Commands.Count)
            {
                PostLog("Get command: " + Commands[index], 1);
            }
            else
            {
                PostLog("Null return Exception!", 1);
            }
        }
        
}//Get all or one command from words dictionary using index

    
    public void DoCommand()
    {
        string[] Commands_ = RequestWords;

        if (Commands_.Length > 0)
        {
            for (int i = 0; i < Commands_.Length; i++)
            {
                foreach (string str in Commands_)
                {
                    if (Equals(Commands[i], str))
                    {
                        PostLog(Commands[i] + " OK!!", 1);
                        return;
                    }
                    else
                    {
                        PostLog(Commands[i] + " != " + str, 1);
                    }
                }
            }
        }
        else
        {
            PostLog("Error! Requested words length = 0", 1);
        }
        Commands_ = null;

    }//Do it!!!
    public bool Equals(string a, string b)
    {
        return a == b ? true : false;
    } // Is Equals the commands and returned google request;
    private void PostLog(string log, int LogPart)
    {
        switch (LogPart)
        {
            case 0:
                if (!string.IsNullOrEmpty(RecordLogsText.text))
                    RecordLogsText.text += "\n" + log;
                else
                    RecordLogsText.text += log;
                break;
            case 1:
                if (!string.IsNullOrEmpty(DictionaryLogsText.text))
                    DictionaryLogsText.text += "\n" + log;
                else
                    DictionaryLogsText.text += log;
                break;
            default: break;
        }
       
    }//Post Logs into 2 logs fields 
    private void Update()
    {

        if (listen)
        {
            loudness = LevelMax() * 100;//for volume before speech

            Debug.Log("Loudness : " + loudness);

            if (loudness > 8)
            {
                Microphone.End(MicDeviceName);
                Debug.Log("Sent Record");
                TryStartRecord();
            }
        }

            if (!isChoosedMic)
            {
                int t = -1;
                string text_ = UI_FirstPage[1].GetComponent<InputField>().text;

                if (!string.IsNullOrEmpty(text_) && text_ != "-")
                    t = System.Convert.ToInt32(UI_FirstPage[1].GetComponent<InputField>().text);

                if (!string.IsNullOrEmpty(text_) && t <= devices.Length && t > 0)
                {
                    UI_FirstPage[2].GetComponent<Button>().interactable = true;
                    UI_FirstPage[2].GetComponent<Button>().gameObject.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    UI_FirstPage[2].GetComponent<Button>().interactable = false;
                    UI_FirstPage[2].GetComponent<Button>().gameObject.GetComponent<Image>().color = Color.red;
                }
                   
            }


            //play last rec audio button
            if(RecordedClip != null)
                UI_SecondPage[1].GetComponent<Button>().interactable = true;
            else
                UI_SecondPage[1].GetComponent<Button>().interactable = false;
            //----------------------------------------------


        if(!string.IsNullOrEmpty(sendToGoogle.GetResponse) && !isGetRequest)
        {
            isGetRequest = true;
            RequestWords = sendToGoogle.GetWords();

            if (RequestWords == null)
                return;

            for (int i = 0; i < RequestWords.Length; i++)
				Debug.Log (RequestWords[i]);
				//PostLog(RequestWords[i], 1);
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
}