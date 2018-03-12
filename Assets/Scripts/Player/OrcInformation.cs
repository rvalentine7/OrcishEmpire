using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Keeps track of information regarding an orc that the player might be interested
 * in knowing.
 */
public class OrcInformation : MonoBehaviour {
    public GameObject orcPopupObject;
    private int orcCount;
    //will want to keep track of where an orc is working

	/**
     * Initializes the number of orcs contained in this game object.
     */
	void Start () {
        //orcCount = 0;
	}
	
    /**
     * Sets the number of orcs contained in this game object.
     */
	public void setOrcCount(int num)
    {
        orcCount = num;
    }

    /**
     * Gets the number of orcs contained in this game object.
     */
    public int getOrcCount()
    {
        return orcCount;
    }

    /**
     * Click the object to see information about it
     */
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag("Popup") == null)
        {
            //might want the object creating this to determine if the orc is emmigrating/immigrating for
            // the popup status to change and to avoid creating even more popups
            Instantiate(orcPopupObject);
        }
    }
}
