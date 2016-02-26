using UnityEngine;
using System.Collections;

public class DialogManager : MonoBehaviour {

	//For SpeechToText
	bool wordsReceived =false;
	public SpeechToText speechToText;
	public TTSDemoSceneManager textToSpeech;

	//For ChatBot
	public MyPandoraBotUI chatBot;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (speechToText.getWords() != "banana") {
			chatBot.sendToBot(speechToText.getWords());
			speechToText.resetWordsReceived();

		}

		if (chatBot.getResponse () != "") {

			Debug.Log(chatBot.getResponse());
			textToSpeech.Speak(chatBot.getResponse());
			chatBot.resetResponse();

		}

	}
}
