using UnityEngine;
using System.Collections;

public class Headtracking : MonoBehaviour {

    public GameObject target;
    Vector3 headlocation;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        headlocation = target.gameObject.transform.position;

        gameObject.transform.position = headlocation;
	}
}
