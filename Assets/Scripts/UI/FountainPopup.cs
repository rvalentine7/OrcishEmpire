using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FountainPopup : MonoBehaviour {
    public GameObject fountain;
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
