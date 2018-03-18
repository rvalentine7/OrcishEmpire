using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Removes the popup the script is attached to if the user clicks off of it.
 */
public class PopupRemoval : MonoBehaviour {
    double time;

	// Use this for initialization
	void Start () {
        time = 0;
	}
	
	/**
     * Checks if the popup canvas should be deleted
     */
	void Update () {
        if (time > 1 && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
        {
            //Debug.Log("test");
            Destroy(GameObject.FindGameObjectWithTag("Popup"));
        }
        else if (time <= 1)
        {
            time += 0.1;
        }
    }
}
