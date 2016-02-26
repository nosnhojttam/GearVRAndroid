using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WordTester : MonoBehaviour {

	public MyPandoraBotUI chatBot;
	private Text displayText;
	// Use this for initialization
	void Start () {
		displayText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		/*speakText = wordGen.getWords();
		if (speakText != "banana") {
			displayText = GetComponent<Text>();
			displayText.text = speakText;
		}*/
		if (chatBot.getResponse () != "") {
			displayText.text = chatBot.getResponse ();
		}
	}
}
