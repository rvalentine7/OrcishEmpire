using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Contains information about the house this is attached to such as
 *  which orcs live in it and what kind of supplies the house has.
 */
public class HouseInformation : MonoBehaviour {
    public Sprite sign;
    public Sprite firstLevelHouse;
    //add future house sprites here

    private int numInhabitants;
    //private List<GameObject> inhabWorkLocations;
    //private int houseSize;
    private bool populationChange;
    //private int desirability;
    //private int taxAmount;
    //private int food;
    //private int water;
    //add future resources here (weapons/furniture/currency)

	/**
     * Initializes the variables involving the house.
     */
	void Start () {
        numInhabitants = 0;
        //inhabWorkLocations = new List<GameObject>();
        //food = 0;
        //water = 0;
        //houseSize = 3;
        //AvailableHome will spawn orcs if the house desirability
        // is high enough and there is still room in the house (compare to inhabitants.Count)
        populationChange = false;
        //desirability = 75;
        //taxAmount = 0;
    }
	
	/**
     * Updates the house appearance based on the number of inhabitants.
     */
	void Update () {
        //change the sprite if needed when the house's population changes
		if (populationChange)
        {
            if (numInhabitants > 0 && numInhabitants <= 3)
            {
                SpriteRenderer spriteRender = gameObject.GetComponent<SpriteRenderer>();
                spriteRender.sprite = firstLevelHouse;
            }
            populationChange = false;
        }
	}

    /**
     * Adds to the inhabitant count in the house.
     */
    public void addInhabitant()
    {
        numInhabitants++;
        populationChange = true;
    }
}
