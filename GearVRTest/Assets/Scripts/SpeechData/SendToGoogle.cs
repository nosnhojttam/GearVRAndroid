//Copyright Frostweep Games,LLC
//2015(c), Unity Speech Recognition using Google API
//SendToGoogle Script

using UnityEngine;
using System.Collections;
using System;
using System.Linq;

namespace SpeechRecognition
{
    public class SendToGoogle : MonoBehaviour
    {

        public string ApiKey = "";//google speech api key; /* if you want to have self key - read documentation */
        public int SampleRate = 16000; // warning!! Flac Converter will be convert audio with 44100 or 16000 sample rate
        private string url_ = "https://www.google.com/speech-api/v2/recognize?output=json&lang=";
        private string language = "en-us"; //default language
        private string _response; //google returned request
        private string[] words; // parsed response

        public LanguageEnum Language = LanguageEnum.DEFAULT;//default google speech language
        public enum LanguageEnum
        {
            DEFAULT = 0,
            RU,
            UK,
            ENUS,
            ENGB,
            DE,
            FR
        } //languages
        public string GetResponse
        {
            get { return _response; }
            private set { _response = value; }
        } //public property return google response

        private void Start()
        {
            SampleRate = AudioSettings.outputSampleRate;
            switch (Language)
            {
                case LanguageEnum.RU:
                    language = "ru-ru";
                    break;
                case LanguageEnum.ENGB:
                    language = "en-gb";
                    break;
                case LanguageEnum.DE:
                    language = "de";
                    break;
                case LanguageEnum.FR:
                    language = "fr";
                    break;
                case LanguageEnum.UK:
                    language = "uk";
                    break;
                case LanguageEnum.ENUS:
                case LanguageEnum.DEFAULT:
                    language = "en-us";
                    break;

            }
            url_ += language + "&key=" + ApiKey;
        }//Init fields

        private void ParseResult(string text_) // simple parse the google returned request
        {
            words = text_.Split(new char[] { ',', '"', ':', '[', ']', '{', '}' });

            words = words.Where(x => !string.IsNullOrEmpty(x)).Select(x => x).ToArray();
        }

        public IEnumerator SendToGoogleAudio(AudioClip clip_)
        {
            byte[] buffer;
            SavePCMIntoMemory.Save(clip_, out buffer);
                
            var form = new WWWForm();
            var headers = form.headers;

            headers["Method"] = "POST";
            headers["Host"] = "www.google.com";
            headers["Send-Chunked"] = "true";
            headers["User-Agent"] = Application.companyName;
            headers["Content-Type"] = "audio/l16; rate=16000";
            headers["Content-Length"] = buffer.Length.ToString();
            headers["Accept"] = "application/json";


            var httpRequest = new WWW(url_, buffer, headers);

            yield return httpRequest;

            if (httpRequest.isDone && string.IsNullOrEmpty(httpRequest.error))
            {
                _response = httpRequest.text;
                ParseResult(_response);
            }
            else
            {
                //Debug log
                _response = String.Format("Request failed with Error: {0}{1}", Environment.NewLine, httpRequest.error);
                Debug.Log(String.Format("Request failed with Error: {0}", httpRequest.error));
            }

        } //send to google
        public string[] GetWords()
        {
            if(words != null)
                GetResponse = null;
            return words;
        } // return parsed words list
    }
}