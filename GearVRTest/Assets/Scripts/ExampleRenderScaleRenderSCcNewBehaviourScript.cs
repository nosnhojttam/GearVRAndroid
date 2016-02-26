using UnityEngine;
using System.Collections;
using UnityEngine.VR;



    public class ExampleRenderScale : MonoBehaviour
    {
        [SerializeField]
        private float m_RenderScale = 2f;              //The render scale. Higher numbers = better quality, but trades performance

        void Start()
        {
            VRSettings.renderScale = m_RenderScale;
        Debug.Log("WORKING");
        }
    }


