using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class AnthonyTextUIStuff : MonoBehaviour {
	protected Text text;
	public SpeechToText words;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(words.getWords () != "banana")
			text.text = words.getWords ();

	}

}
