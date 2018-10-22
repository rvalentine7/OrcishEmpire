using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Contains information about the house this is attached to such as
 *  which orcs live in it and what kind of supplies the house has.
 */
public class HouseInformation : MonoBehaviour {
    public float timeInterval;
    public float timeBeforeEntertainmentDecay;
    public GameObject orcImmigrant;
    public GameObject orcEmigrant;
    public int inhabitantWaterConsumption;
    public int inhabitantFoodConsumption;
    public int jobSearchRadius;
    public GameObject housePopupObject;
    public Sprite sign;
    public Sprite firstLevelHouse;
    public Sprite secondLevelHouse;
    public Sprite thirdLevelHouse;
    //add future house sprites here (aiming for at least 5 house levels)

    private int numInhabitants;
    private int numIncomingOrcs;
    private Dictionary<GameObject, int> inhabWorkLocations;
    private int numEmployedInhabitants;
    private int houseSize;
    private int houseLevel;
    private bool upgrading;
    private bool downgrading;
    //private bool populationChange;
    private int desirability;
    private bool multipleFoodTypes;
    private float checkTime;
    //private int taxAmount;
    private int food;//will need different types of food later on (meat, bread, etc)
    private int numWaterSources;
    private int entertainmentLevel;//this can be 0, 1, or 2.  If it just experiences a change,
                                   // there will need to be some sort of delay before the house reacts
    private float timeOfLastEntertainment;
    //add future resources here (weapons/furniture/currency)
    private GameObject world;
    private World myWorld;

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
        numWaterSources = 0;
        houseSize = 3;//houseSize updates based on factors such as food/water/luxury products
        houseLevel = 1;
        upgrading = false;
        downgrading = false;
        //AvailableHome will spawn orcs if the house desirability
        // is high enough and there is still room in the house (compare to houseSize)
        //populationChange = false;
        desirability = 100;
        multipleFoodTypes = false;
        checkTime = 0.0f;
        //taxAmount = 0;
        entertainmentLevel = 0;
        timeOfLastEntertainment = 0.0f;
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
        //Check if tile already has water, update numWaterSources if so
        if (terrainArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<Tile>().hasWater())
        {
            addWaterSource();
        }
    }

    /**
     * Updates the house appearance based on the number of inhabitants as well as updates
     *  the resources the house has stored/used.
     */
    void Update() {
        //if (populationChange)
        //{
        //    if (numInhabitants > 0 && numInhabitants <= 3)
        //    {
        //SpriteRenderer spriteRender = gameObject.GetComponent<SpriteRenderer>();
        //spriteRender.sprite = firstLevelHouse;
        //    }
        //checks for further house upgrades will go here
        //    populationChange = false;
        //}

        //Reduces the enteratinment level if it hasn't been given entertainment in the timeBeforeEntertainmentDecay time window
        if (Time.time > timeOfLastEntertainment + timeBeforeEntertainmentDecay && entertainmentLevel > 0)
        {
            entertainmentLevel--;
            timeOfLastEntertainment = Time.time;
        }

        //updates the resources of the household
        Storage storage = gameObject.GetComponent<Storage>();
        if (Time.time > checkTime)
        {
            checkTime = Time.time + timeInterval;

            //TODO: update food information
            if (storage.getFoodCount() > 0 && storage.getFoodCount() >= numInhabitants * inhabitantFoodConsumption)
            {
                int numFoodTypes = 0;
                if (storage.getMeatCount() > 0)
                {
                    numFoodTypes++;
                }
                if (storage.getWheatCount() > 0)
                {
                    numFoodTypes++;
                }
                if (storage.getEggCount() > 0)
                {
                    numFoodTypes++;
                }
                if (numFoodTypes > 1)
                {
                    multipleFoodTypes = true;
                }
                else
                {
                    multipleFoodTypes = false;
                }
                storage.removeResource("Meat", numInhabitants * inhabitantFoodConsumption * (storage.getMeatCount() / storage.getFoodCount()));
                if (storage.getFoodCount() > 0)
                {
                    storage.removeResource("Wheat", numInhabitants * inhabitantFoodConsumption * (storage.getWheatCount() / storage.getFoodCount()));
                }
                if (storage.getFoodCount() > 0)
                {
                    storage.removeResource("Eggs", numInhabitants * inhabitantFoodConsumption * (storage.getEggCount() / storage.getFoodCount()));
                }
            }
            else if (storage.getFoodCount() > 0)
            {
                storage.removeResource("Meat", storage.getMeatCount());
                storage.removeResource("Wheat", storage.getWheatCount());
                storage.removeResource("Eggs", storage.getEggCount());
            }

            //TODO: Add text here for the house popup to pull on the describe why a house is upgrading/downgrading
            //Upgrade
            if ((houseLevel >= 1 && storage.getFoodCount() > 0 && numWaterSources > 0))
            {
                if (houseLevel == 1)
                {
                    if (upgrading)
                    {
                        houseLevel = 2;
                        updateHouseSprite();
                        houseSize *= 2;
                        gameObject.AddComponent<AvailableHome>();
                        AvailableHome availableHome = gameObject.GetComponent<AvailableHome>();
                        availableHome.immigrant = orcImmigrant;
                        upgrading = false;
                    }
                    else
                    {
                        upgrading = true;
                    }
                    downgrading = false;
                }
                else if (houseLevel == 2 && entertainmentLevel > 0)
                {
                    if (upgrading)
                    {
                        houseLevel = 3;
                        updateHouseSprite();
                        houseSize *= 2;
                        gameObject.AddComponent<AvailableHome>();
                        AvailableHome availableHome = gameObject.GetComponent<AvailableHome>();
                        availableHome.immigrant = orcImmigrant;
                        upgrading = false;
                    }
                    else
                    {
                        upgrading = true;
                    }
                    downgrading = false;
                }
            }
            //Downgrade
            else if ((houseLevel > 1 && (storage.getFoodCount() == 0 || numWaterSources == 0))
                || (houseLevel > 2 && entertainmentLevel == 0))
            {

                if (downgrading)
                {
                    houseLevel--;
                    updateHouseSprite();
                    houseSize /= 2;
                    if (numInhabitants > houseSize)
                    {
                        removeInhabitants(numInhabitants - houseSize);
                    }
                }
                else
                {
                    downgrading = true;
                }
                upgrading = false;
            }
            //Stop downgrading
            else if (downgrading && (storage.getFoodCount() > 0 && numWaterSources > 0 && houseLevel > 1))
            {
                if (entertainmentLevel > 0 && houseLevel > 2)
                {
                    downgrading = false;
                }
                else
                {
                    downgrading = false;
                }
            }

            //TODO: should not be able to downgrade twice in one time tick
            if (multipleFoodTypes && houseLevel == 2)
            {
                //will be used for upgrading to tier 3
            }
            else if (!multipleFoodTypes && houseLevel > 2)
            {

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
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag("Popup") == null)
        {
            GameObject popup = Instantiate(housePopupObject) as GameObject;
            HousePopup housePopup = popup.GetComponent<HousePopup>();
            housePopup.setHouse(gameObject);
        }
    }

    /**
     * Updates the sprite for the house based on the house level
     */
    public void updateHouseSprite()
    {
        SpriteRenderer spriteRender = gameObject.GetComponent<SpriteRenderer>();
        if (houseLevel == 1)
        {
            spriteRender.sprite = firstLevelHouse;
        }
        else if (houseLevel == 2)
        {
            spriteRender.sprite = secondLevelHouse;
        }
        else if (houseLevel == 3)
        {
            spriteRender.sprite = thirdLevelHouse;
        }
    }

    /**
     * Adds to the inhabitant count in the house.
     * @param num the number of inhabitants to add
     */
    public void addInhabitants(int num)
    {
        if (numInhabitants == 0)
        {
            updateHouseSprite();
        }
        numInhabitants += num;
        //populationChange = true;
        this.myWorld.updatePopulation(num);
    }

    /**
     * Updates the number of orcs on their way to enter the house.
     * @param num the number of orcs on their way
     */
    public void orcsMovingIn(int num)
    {
        numIncomingOrcs = num;
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
     * Removes inhabitants from the house.
     */
    public void removeInhabitants(int num)
    {
        numInhabitants -= num;
        int numRemovedSoFar = 0;
        List<GameObject> keys = new List<GameObject>(inhabWorkLocations.Keys);
        foreach (GameObject key in keys)
        {
            if (numRemovedSoFar < num)
            {
                Employment employment = key.GetComponent<Employment>();
                int numInhabsAtEmployment = inhabWorkLocations[key];
                if (numRemovedSoFar + numInhabsAtEmployment <= num)
                {
                    employment.removeWorkers(inhabWorkLocations[key], gameObject);
                    numRemovedSoFar += inhabWorkLocations[key];
                    removeWorkLocation(key);
                }
                else
                {
                    employment.removeWorkers(num - numRemovedSoFar, gameObject);
                    inhabWorkLocations[key] -= num - numRemovedSoFar;
                    numRemovedSoFar = num;
                }
            }
        }
        //populationChange = true;
        //TODO: create orc gameobjects leaving the house and exiting the map
        if (numInhabitants > 0)
        {
            Instantiate(orcEmigrant, new Vector2(gameObject.transform.position.x,
                gameObject.transform.position.y + 0.4f), Quaternion.identity);
        }
        this.myWorld.updatePopulation(-num);
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
        if (numInhabitants > 0)
        {
            Instantiate(orcEmigrant, new Vector2(gameObject.transform.position.x,
                gameObject.transform.position.y + 0.4f), Quaternion.identity);
        }
        this.myWorld.updatePopulation(-numInhabitants);
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
        /*if (numInhabitants > 0)
        {
            if (water + num <= 100)
            {
                water += num;
            }
            else
            {
                water = 100;
            }
        }*/
    }

    /**
     * Adds a source supplying water to the house
     */
    public void addWaterSource()
    {
        numWaterSources++;
    }

    /**
     * Removes a source supplying water to the house
     */
    public void removeWaterSource()
    {
        numWaterSources--;
    }

    /**
     * Gets the water count at the house.
     * @return water the amount of water the house holds
     */
    public int getWaterCount()
    {
        return (numWaterSources > 0 ? 100 : 0);//TODO: change this when fully removed water count
    }

    /**
     * Gets the food count at the house.
     * @return food the amount of food the house holds
     */
    public int getFoodCount()
    {
        return food;
    }

    /**
     * Sets the current entertainment level of the house
     * @param level the lowest entertainment level this house should be at
     */
    public void setEntertainmentLevel(int level)
    {
        if (level > entertainmentLevel)
        {
            entertainmentLevel = level;
            timeOfLastEntertainment = Time.time;
        }
    }

    /**
     * Gets the current entertainment level of the household
     * @return entertainmentLevel the current entertainment level of the household
     */
    public int getEntertainmentLevel()
    {
        return entertainmentLevel;
    }
}
