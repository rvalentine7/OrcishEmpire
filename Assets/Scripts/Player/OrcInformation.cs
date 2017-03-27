using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Keeps track of information regarding an orc that the player might be interested
 * in knowing.
 */
public class OrcInformation : MonoBehaviour {
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
}
