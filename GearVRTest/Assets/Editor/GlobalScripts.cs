//Copyright Frostweep Games,LLC
//2015(c), Unity Speech Recognition using Google API
//GlobalScripts Editor Script

using UnityEngine;
using UnityEditor;
using System.Diagnostics;


namespace SpeechRecognition
{
    public class GlobalScripts : EditorWindow
    {

        private object Documentation;
        public GameObject BasePrefab;
        public Texture2D logo;
        static EditorWindow window;
        private bool languageRu = false;
        private string[] Localization = new string[8];

        [MenuItem("Speech Recognition/Speech Plugin Window")]
        static void Init()
        {
            window = GetWindow(typeof(GlobalScripts), false, "Unity Speech Recognition using Google");
            window.position = new Rect(50, 50, 450, 350);
            window.maxSize = new Vector2(450, 350);
            window.minSize = new Vector2(450, 350);
            window.Show();
        }

        void OnGUI()
        {
            bool togle_ = false;

            GUI.Label(new Rect(110, 0, 250, 40), "Unity Speech Recognition using Google API");
            GUI.DrawTexture(new Rect(5, 5, 440, 150), logo);

            if (GUI.Button(new Rect(10, 160, 210, 40), Localization[0]))
            {
                Instantiate(BasePrefab, Vector3.zero, Quaternion.identity);
            }
            if (GUI.Button(new Rect(10, 210, 210, 40), Localization[1]))
            {
                Process.Start(Application.dataPath + Localization[6]);
            }
            if (GUI.Button(new Rect(10, 260, 210, 40), Localization[2]))
            {
                Application.OpenURL("https://www.chromium.org/developers/how-tos/api-keys");
            }
            if (GUI.Button(new Rect(230, 160, 210, 40), Localization[3]))
            {
                Application.LoadLevel("sample");
            }
            if (GUI.Button(new Rect(230, 210, 210, 40), Localization[4]))
            {
                Application.OpenURL("https://twitter.com/frostweep_games");
            }
            if (GUI.Button(new Rect(230, 260, 210, 40), Localization[5]))
            {
                Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/publisher/14839/page/1/sortby/popularity");
            }
            if (GUI.Button(new Rect(10, 310, 140, 30), Localization[7]))
            {
                languageRu = !languageRu;
            }

            GUI.Label(new Rect(180, 320, 250, 40), "Copyright Frostweep Games, LLC (c)2015");

        }

        void Update()
        {
            if(languageRu)
            {
                Localization[0] = "Добавить базовый префаб";
                Localization[1] = "Открыть документацию";
                Localization[2] = "Получить API ключ";
                Localization[3] = "Открыть экзампл сцену";
                Localization[4] = "Оставить отзыв";
                Localization[5] = "Другие продукты FG, LLC";
                Localization[6] = "/DocumentationRU.pdf";
                Localization[7] = "English";
            }
            else
            {
                Localization[0] = "Add Base Prefab";
                Localization[1] = "Open Documentation";
                Localization[2] = "Get Google Speech API Key";
                Localization[3] = "Open Example Scene";
                Localization[4] = "Give Feedback";
                Localization[5] = "Other Frostweep Games Products";
                Localization[6] = "/DocumentationEN.pdf";
                Localization[7] = "Русский";
            }
        }
    }
}