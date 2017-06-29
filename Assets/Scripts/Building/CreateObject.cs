using UnityEngine;
using System.Collections;

/**
 * Used by the RoadCreation button in the game UI to create roads.
 */
public class CreateObject : MonoBehaviour {
    public GameObject construct;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    /**
     * Creates a new road object to display where roads can be built and what they will look like when built.
     */
    public void Create ()
    {
        //Destroys the existing popup/building creation object if the user attempts to create something else
        if (GameObject.FindWithTag("Popup") != null)
        {
            Destroy(GameObject.FindWithTag("Popup"));
        }
        GameObject constructObj = Instantiate(construct, Input.mousePosition, Quaternion.identity) as GameObject;
        constructObj.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
