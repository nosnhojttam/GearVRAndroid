using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyPandoraBotUI : MonoBehaviour {

	//public GameObject UI; // To change UI text

	protected string text;
	protected string response;
	protected string sessionId; // Remains null until after first message is parsed
	protected bool waiting;

	public string botid = "azim196";// Name of chatbot to use. Has to be made using website
	public string appid = "1409612435252"; // From Pandorabots application
	public string userkey = "99f6ae1b5dfb10b4786535f97f6a651d";//From Pandorabots application
	//public string clientname = "Anthony"; //This can be anything, it is used to store information about the client such as age/hair color

	// Use this for initialization
	void Start () {
		text = "";
		response = "";
	}
	
	// Update is called once per frame
	void Update () {
		

	}

	string sanitizePandoraResponse(string wwwText)
	{
		string responseString = "";

		int startIndex = wwwText.IndexOf (" [") + 2;
		int endIndex = wwwText.IndexOf ("],");
		responseString = wwwText.Substring (startIndex, endIndex - startIndex);

		Debug.Log ("Sanitized response: " + responseString);
		//UI.GetComponent<UIMessageText> ().changeTextTo (responseString);
		return responseString;
	}

	void getSessionIdOfPandoraResponse(string wwwText)
	{
		int startIndex = wwwText.IndexOf ("sessionid") + 12;
		int endIndex = wwwText.IndexOf ("}") - 1;

		sessionId = wwwText.Substring (startIndex, endIndex - startIndex);
	}

	private IEnumerator PandoraBotRequestCoRoutine( string text )
	{
		waiting = true;
		string url = "https://aiaas.pandorabots.com/talk/" + appid; 
		url = url + "/" + botid;
		url = url +  "?input=" + WWW.EscapeURL(text);
		if (sessionId != null) {
			//url = url + "&client_name=" + clientname; //Client name is causing an error.
			url = url + "&sessionid=" + sessionId;
		}
		url = url + "&user_key=" + userkey;
		//url = "https://aiaas.pandorabots.com/talk/1409612420718/azim196?input=Hello.&user_key=2a6abf36666921926208374ef29d012a";
		//url = "https://aiaas.pandorabots.com/talk/1409612420718/azim196?input=Hi&client_name=Anthony&sessionid=15428601&user_key=2a6abf36666921926208374ef29d012a";
		//url = "https://aiaas.pandorabots.com/talk/1409612420718/azim196?input=Hi&sessionid=15428601&user_key=2a6abf36666921926208374ef29d012a";

		Debug.Log (url);

		WWW www = new WWW(url, new byte[]{0}); //You cannot do POST with empty post data, new byte is just dummy data to solve this problem

		yield return www;

		if( www.error == null )
		{
			Debug.Log(www.text);
			getSessionIdOfPandoraResponse (www.text);
			Debug.Log ("SessionId:" + sessionId + ".");

			response = sanitizePandoraResponse (www.text); // THIS IS THE MESSAGE THAT SHOULD BE TTS'd
		}
		else
		{
			Debug.LogWarning(www.error);
		}
	}

	void OnGUI()
	{
		int label_width = 90;		
		int edit_width = 250;

		text = GUI.TextArea(new Rect(10, Screen.height - 60 , edit_width + label_width, 50), text, 512);

		if( text.Contains("\n") || text.Contains("..."))
		{

			StartCoroutine(PandoraBotRequestCoRoutine(text));	
			text = "";
		}
		GUI.Label(new Rect(10, Screen.height - 80, 300, 20), "Type something below and hit enter!");

	}

	public void sendToBot(string phrase){

		StartCoroutine(PandoraBotRequestCoRoutine(phrase));


	}

	public string getResponse(){

		return response;

	}

	public void resetResponse(){

		response = "";

	}

}
