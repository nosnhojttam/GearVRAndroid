using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViewSkeleton : MonoBehaviour 
{

    public Transform rootNode;
    public Transform[] childNodes;
	// Use this for initialization
	//void Start () {
	
	//}
	
	//// Update is called once per frame
	//void Update () {
	
	//}
    void Awake()
    {
        
    }
    void OnDrawGizmosSelected()
    {
        PopulateChildren();
        if (rootNode != null)
        {
            //get all bones to draw
            //Gizmos.DrawCube(rootNode.position, new Vector3(.1f, .1f, .1f));
            foreach (Transform child in childNodes)
            {
                //check children of the children??
                //map all bones first?
                
                if (child == rootNode)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(child.position, new Vector3(.1f, .1f, .1f));
                }
                else
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(child.position, child.parent.position);
                    Gizmos.DrawCube(child.position, new Vector3(.01f, .01f, .01f));
                }
            }

        }
    }

    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
