using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Contains information about the house this is attached to such as
 *  which orcs live in it and what kind of supplies the house has.
 */
public class HouseInformation : MonoBehaviour {
    public float timeInterval;
    public int inhabitantWaterConsumption;
    public int jobSearchRadius;
    public GameObject housePopupObject;
    public Sprite sign;
    public Sprite firstLevelHouse;
    //add future house sprites here

    private int numInhabitants;
    private int numIncomingOrcs;
    private Dictionary<GameObject, int> inhabWorkLocations;
    private int numEmployedInhabitants;
    private int houseSize;
    private bool populationChange;
    private int desirability;
    private float checkTime;
    //private int taxAmount;
    private int food;//will need different types of food later on (meat, bread, etc)
    private int water;
    //add future resources here (weapons/furniture/currency)

	/**
     * Initializes the variables involving the house.
     */
	void Start () {
        numInhabitants = 0;
        numIncomingOrcs = 0;
        inhabWorkLocations = new Dictionary<GameObject, int>();//keeps a list of the gameObjects where inhabitants
        // are working
        numEmployedInhabitants = 0;
        food = 0;
        water = 0;
        houseSize = 3;//houseSize updates based on factors such as food/water/luxury products
        //AvailableHome will spawn orcs if the house desirability
        // is high enough and there is still room in the house (compare to houseSize)
        populationChange = false;
        desirability = 100;
        checkTime = 0.0f;
        //taxAmount = 0;
    }
	
	/**
     * Updates the house appearance based on the number of inhabitants as well as updates
     *  the resources the house has stored/used.
     */
	void Update () {
		if (populationChange)
        {
            if (numInhabitants > 0 && numInhabitants <= 3)
            {
                SpriteRenderer spriteRender = gameObject.GetComponent<SpriteRenderer>();
                spriteRender.sprite = firstLevelHouse;
            }
            //checks for further house upgrades will go here
            populationChange = false;
        }
        //updates the resources of the household
        if (Time.time > checkTime)
        {
            checkTime = Time.time + timeInterval;
            if (water >= numInhabitants * inhabitantWaterConsumption)
            {
                water -= numInhabitants * inhabitantWaterConsumption;
            } else
            {
                water = 0;
            }
        }
        //find places for unemployed inhabitants to work
        if (numEmployedInhabitants < numInhabitants)
        {
            Vector2 housePosition = gameObject.transform.position;
            GameObject world = GameObject.Find("WorldInformation");
            World myWorld = world.GetComponent<World>();
            GameObject[,] constructArr = myWorld.constructNetwork.getConstructArr();
            int i = 0;
            while (i <= jobSearchRadius * 2 && numEmployedInhabitants < numInhabitants)
            {
                int j = 0;
                while (j <= jobSearchRadius * 2 && numEmployedInhabitants < numInhabitants)
                {
                    if (housePosition.x - jobSearchRadius + i >= 0 && housePosition.y - jobSearchRadius + j >= 0
                        && housePosition.x - jobSearchRadius + i <= 39 && housePosition.y - jobSearchRadius + j <= 39
                        && constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j] != null
                        && constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j].tag == "Building")
                    {
                        GameObject possibleEmployment = constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j];
                        Employment employment = possibleEmployment.GetComponent<Employment>();
                        int numJobsAtEmployment = employment.numJobsAvailable();
                        if (employment.getOpenForBusiness() && numJobsAtEmployment > 0)
                        {
                            int numUnemployed = numInhabitants - numEmployedInhabitants;
                            int residentsToEmploy = numJobsAtEmployment - numUnemployed;
                            //Case #1: There are less job spots at the employment than the number of unemployed in this house
                            if (residentsToEmploy < 0)
                            {
                                employment.addWorker(numUnemployed + residentsToEmploy, gameObject);
                                if (inhabWorkLocations.ContainsKey(possibleEmployment))
                                {
                                    inhabWorkLocations[possibleEmployment] += numUnemployed + residentsToEmploy;
                                }
                                else
                                {
                                    inhabWorkLocations.Add(possibleEmployment, numUnemployed + residentsToEmploy);
                                }
                                numEmployedInhabitants += numUnemployed + residentsToEmploy;
                            }
                            //Case #2:  There are more than enough job spots at the employment to employ all unemployed members of this house
                            else
                            {
                                employment.addWorker(numUnemployed, gameObject);
                                if (inhabWorkLocations.ContainsKey(possibleEmployment))
                                {
                                    inhabWorkLocations[possibleEmployment] += numUnemployed;
                                }
                                else
                                {
                                    inhabWorkLocations.Add(possibleEmployment, numUnemployed);
                                }
                                numEmployedInhabitants += numUnemployed;
                            }
                        }
                    }
                    j++;
                }
                i++;
            }
        }
	}

    /**
     * Click the object to see information about it
     */
    void OnMouseDown()
    {
        //Need to have this create a pop-up that displays number of inhabitants,
        // food, water, furniture, other resources of the house
        /*Debug.Log("Number of inhabitants: " + numInhabitants);
        Debug.Log("Number of employed inhabitants: " + numEmployedInhabitants + "/" + numInhabitants);
        Debug.Log("Total water: " + water);
        Debug.Log("Total food: " + food);*/
        if (GameObject.FindWithTag("Popup") == null)
        {
            GameObject popup = Instantiate(housePopupObject) as GameObject;
            HousePopup housePopup = popup.GetComponent<HousePopup>();
            housePopup.setHouse(gameObject);
        }
    }

    /**
     * Adds to the inhabitant count in the house.
     * @param num the number of inhabitants to add
     */
    public void addInhabitants(int num)
    {
        numInhabitants += num;
        populationChange = true;
    }

    /**
     * Updates the number of orcs on their way to enter the house.
     * @param num the number of orcs on their way
     */
    public void orcsMovingIn(int num)
    {
        numIncomingOrcs += num;
    }

    /**
     * Returns the number of orcs currently moving in to this house.
     * @return the number of orcs moving in
     */
    public int getNumOrcsMovingIn()
    {
        return numIncomingOrcs;
    }

    /**
     * Removes an inhabitant from the house.
     */
    public void removeInhabitant()
    {
        numInhabitants--;
        populationChange = true;
    }

    /**
     * Destroys the house and forces the inhabitants to move.
     */
    public void destroyHouse()
    {
        //TODO: create orc gameobjects leaving the house and exiting the map
        foreach (KeyValuePair<GameObject, int> entry in inhabWorkLocations)
        {
            Employment employment = entry.Key.GetComponent<Employment>();
            employment.removeWorkers(entry.Value, gameObject);
        }
        Destroy(gameObject);
    }

    /**
     * Gets the number for how desirable this house is to live in.
     * @return the desirability value
     */
    public int getDesirability()
    {
        return desirability;
    }

    /**
     * Gets the maximum occupancy currently allowed in the house.
     * @return the maximum number of inhabitants capable of living in this house
     */
    public int getHouseSize()
    {
        return houseSize;
    }

    /**
     * Gets the number of inhabitants living in the house.
     * @return the number of inhabitants currently in the house
     */
    public int getNumInhabitants()
    {
        return numInhabitants;
    }

    /**
     * Tells how many inhabitants are currently employed.
     * @return numEmployedInhabitants the number of inhabitants currently employed
     */
    public int getNumEmployedInhabs()
    {
        return numEmployedInhabitants;
    }

    /**
     * Removes a work location from the inhabWorkLocations dictionary and
     * unemploys the inhabitants who worked there.
     * @param employment the work location to be removed
     */
    public void removeWorkLocation(GameObject employment)
    {
        numEmployedInhabitants -= inhabWorkLocations[employment];
        if (numEmployedInhabitants < 0)
        {
            Debug.Log("A house has negative employed inhabitants.");
        }
        inhabWorkLocations.Remove(employment);
    }

    /**
     * Adds water to the household's supplies.
     * @param num the amount of water to add to the house's supplies
     */
    public void addWater(int num)
    {
        if (numInhabitants > 0)
        {
            if (water + num <= 100)
            {
                water += num;
            }
            else
            {
                water = 100;
            }
        }
    }

    /**
     * Gets the water count at the house.
     * @return water the amount of water the house holds
     */
    public int getWaterCount()
    {
        return water;
    }

    /**
     * Gets the food count at the house.
     * @return food the amount of food the house holds
     */
    public int getFoodCount()
    {
        return food;
    }
}
