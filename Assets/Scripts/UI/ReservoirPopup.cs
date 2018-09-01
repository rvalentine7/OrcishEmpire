using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReservoirPopup : MonoBehaviour {
    public GameObject reservoir;
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
        Reservoir reservoirScript = reservoir.GetComponent<Reservoir>();
        if (reservoirScript.getFilled())
        {
            status.text = "Supplying water access to nearby fountains.";
        }
        else
        {
            status.text = "This reservoir needs a connection to a filled reservoir in order to supply water access to nearby fountains";
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
     * Sets the reservoir object this popup is displaying information on.
     * @param reservoir the reservoir the popup is displaying information on
     */
    public void setReservoir(GameObject reservoir)
    {
        this.reservoir = reservoir;
    }
}
