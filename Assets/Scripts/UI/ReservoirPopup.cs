using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReservoirPopup : MonoBehaviour {
    public GameObject reservoir;
    public Text status;
    private bool initialClick;

    // Use this for initialization
    void Start () {
        GameObject panel = GameObject.FindWithTag(World.BUILD_PANEL);
        if (panel != null)
        {
            panel.SetActive(false);
        }
        initialClick = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (initialClick && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
        {
            initialClick = false;
        }
        if (Input.GetKey(KeyCode.Escape) || (!initialClick
            && !EventSystem.current.IsPointerOverGameObject()
            && (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))))
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
