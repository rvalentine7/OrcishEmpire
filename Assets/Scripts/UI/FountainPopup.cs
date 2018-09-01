using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FountainPopup : MonoBehaviour {
    public GameObject fountain;
    public Text status;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
        Fountain fountainScript = fountain.GetComponent<Fountain>();
        if (fountainScript.getFilled())
        {
            status.text = "Supplying water to nearby locations";
        }
        else
        {
            status.text = "This fountain needs access to a filled reservoir to supply water to nearby locations";
        }
    }

    /**
     * Removes the game object from the game.
     */
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    /**
     * Sets the fountain object this popup is displaying information on.
     * @param fountain the fountain object this popup is displaying information on
     */
    public void setFountain(GameObject fountain)
    {
        this.fountain = fountain;
    }
}
