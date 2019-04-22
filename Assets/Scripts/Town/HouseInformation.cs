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
    public int upkeep;
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
    //add future house sprites here (aiming for at least 5 house levels... ideally more like 9)

    private int numInhabitants;
    private int numIncomingOrcs;
    private Dictionary<GameObject, int> inhabWorkLocations;
    private Dictionary<GameObject, int> sickInhabWorkLocations;
    private int numEmployedInhabitants;
    private int houseSize;
    private int houseLevel;
    private bool upgrading;
    private bool downgrading;
    //private bool populationChange;
    private int desirability;
    private bool multipleFoodTypes;
    private float checkTime;
    private int householdCurrency;
    private int goldSinceLastTax;
    private float nextPaymentTime;
    private int food;//will need different types of food later on (meat, bread, etc)
    private int numWaterSources;
    private int entertainmentLevel;//this can be 0, 1, or 2.  If it just experiences a change,
                                   // there will need to be some sort of delay before the house reacts
    private float timeOfLastEntertainment;

    private List<GameObject> nearbyBarbers;//Reduces likelihood of getting sick
    private int numCoveredByBarbers;
    private List<GameObject> nearbyBaths;//Reduces likelihood of getting sick
    private List<GameObject> nearbyHospitals;//Hospitals treat orcs who are sick
    private int baseHealthPercentage;
    private int bathHealthPercentage;
    private int barberHealthPercentage;
    private int numSickInhabitantsAtHospital;
    private int minSickRecoveryWait;
    private int maxSickRecoveryWait;
    private int sickRecoveryChance;
    private List<SickOrc> sickInhabitants;
    private Vector2 housePosition;
    private GameObject[,] constructArr;

    //add future resources here (weapons/furniture/currency)
    private GameObject world;
    private World myWorld;

    private void Awake()
    {
        sickInhabitants = new List<SickOrc>();
    }

    /**
     * Initializes the variables involving the house.
     */
    void Start() {
        numInhabitants = 0;
        numIncomingOrcs = 0;
        inhabWorkLocations = new Dictionary<GameObject, int>();//keeps a list of the gameObjects where inhabitants
        sickInhabWorkLocations = new Dictionary<GameObject, int>();//keeps a list of where sick inhabitants work
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
        householdCurrency = 0;
        goldSinceLastTax = 0;
        entertainmentLevel = 0;
        timeOfLastEntertainment = 0.0f;

        //TODO: should look for nearby health buildings.  should also have public methods for adding health buildings (when a health building is added after the house is built)
        /*Health buildings can only service so many people... need to account for this... number of employees at health locations determines how many people can be serviced
         Structure it like employing inhabitants?  Baths already have a restriction so maybe just hospital beds and how many people can be taken care of at barbers?*/
        nearbyBarbers = new List<GameObject>();
        numCoveredByBarbers = 0;
        nearbyBaths = new List<GameObject>();
        nearbyHospitals = new List<GameObject>();
        baseHealthPercentage = 39;
        bathHealthPercentage = 30;
        barberHealthPercentage = 30;
        numSickInhabitantsAtHospital = 0;
        minSickRecoveryWait = 2;
        maxSickRecoveryWait = 5;
        sickRecoveryChance = 25;

        world = GameObject.Find(World.WORLD_INFORMATION);
        myWorld = world.GetComponent<World>();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
        constructArr = myWorld.constructNetwork.getConstructArr();
        nextPaymentTime = myWorld.getPaymentTime();
        //Check if tile already has water, update numWaterSources if so
        if (terrainArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<Tile>().hasWater())
        {
            addWaterSource();
        }

        //Seeing if any barbers/baths/hospitals are already around
        housePosition = gameObject.transform.position;
        for (int i = -jobSearchRadius; i < jobSearchRadius; i++)
        {
            for (int j = -jobSearchRadius; j < jobSearchRadius; j++)
            {
                if (housePosition.x - jobSearchRadius + i >= 0 && housePosition.y - jobSearchRadius + j >= 0
                        && housePosition.x - jobSearchRadius + i <= myWorld.mapSize && housePosition.y - jobSearchRadius + j <= myWorld.mapSize
                        && constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j] != null
                        && constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j].tag == World.BUILDING)
                {
                    if (constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j].GetComponent<Barber>() != null)
                    {
                        //TODO: this should only actually happen when adding inhabitants because a barber can only take so many clients
                    }
                    else if (constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j].GetComponent<MudBath>() != null)
                    {
                        //TODO: this should only be added if the mudbath is wet
                    }
                    else if (constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j].GetComponent<Hospital>() != null)
                    {
                        nearbyHospitals.Add(constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j]);
                    }
                }
            }
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
        if (Time.time > checkTime)//TODO: break these up into separate methods for better clarity (like updateHealth() and updateStorage())
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

            //TODO: should not be able to downgrade twice in one time tick... is this still needed? don't recall houses downgrading twice in one tick
            if (multipleFoodTypes && houseLevel == 2)
            {
                //will be used for upgrading to tier 3
            }
            else if (!multipleFoodTypes && houseLevel > 2)
            {

            }

            //Health (TODO: might need to slow this down later on by setting it to a different timer)
            if (numInhabitants > 0)
            {
                //If an orc is sick, it has a chance to improve on its own
                List<int> recoveredOrcIndices = new List<int>();
                List<SickOrc> sickOrcsNeedingHospitals = new List<SickOrc>();
                for (int i = 0; i < sickInhabitants.Count; i++)
                {
                    //Debug.Log("increasing wait time");
                    sickInhabitants[i].increaseWaitTime();
                    //Check if the sick inhabitant has recovered
                    if (sickInhabitants[i].getWaitTime() > minSickRecoveryWait && (Random.value * 100 < sickRecoveryChance || sickInhabitants[i].getWaitTime() > maxSickRecoveryWait))
                    {
                        recoveredOrcIndices.Add(i);
                    }
                    //If the sick inhabitant is not at a hospital, check if it can go to one
                    else if (sickInhabitants[i].getHospital() == null)//TODO: should I save this for later once I determine who all is sick?... might be best to leave here so there is a "time" before it arrives at the hospital
                    {
                        sickOrcsNeedingHospitals.Add(sickInhabitants[i]);
                    }
                }
                //Get sick orcs into hospitals
                int xIndex = 0;
                while (sickOrcsNeedingHospitals.Count > 0 && xIndex < jobSearchRadius * 2)//TODO: can speed this up by updating hospitals this has access to when hospitals are created and when houses are created
                {
                    int yIndex = 0;
                    while (sickOrcsNeedingHospitals.Count > 0 && yIndex < jobSearchRadius * 2)
                    {
                        if (housePosition.x - jobSearchRadius + xIndex >= 0 && housePosition.y - jobSearchRadius + yIndex >= 0
                        && housePosition.x - jobSearchRadius + xIndex <= myWorld.mapSize - 1 && housePosition.y - jobSearchRadius + yIndex <= myWorld.mapSize - 1
                        && constructArr[(int)housePosition.x - jobSearchRadius + xIndex, (int)housePosition.y - jobSearchRadius + yIndex] != null
                        && constructArr[(int)housePosition.x - jobSearchRadius + xIndex, (int)housePosition.y - jobSearchRadius + yIndex].tag == World.BUILDING
                        && constructArr[(int)housePosition.x - jobSearchRadius + xIndex, (int)housePosition.y - jobSearchRadius + yIndex].GetComponent<Hospital>() != null)
                        {
                            Hospital hospital = constructArr[(int)housePosition.x - jobSearchRadius + xIndex, (int)housePosition.y - jobSearchRadius + yIndex].GetComponent<Hospital>();
                            if (hospital.addSickOrc(sickOrcsNeedingHospitals[0]))
                            {
                                sickOrcsNeedingHospitals.RemoveAt(0);
                            }
                        }
                        yIndex++;
                    }
                    xIndex++;
                }
                //Update orcs that have recovered from sickness
                recoveredOrcIndices.Sort();
                for (int i = 0; i < recoveredOrcIndices.Count; i++)
                {
                    //Get the recovered orc index, but as it removes from sickInhabitants, the index will also lower
                    SickOrc recoveredOrc = sickInhabitants[recoveredOrcIndices[i] - i];
                    GameObject recoveredOrcHospital = recoveredOrc.getHospital();
                    if (recoveredOrcHospital != null)
                    {
                        Hospital hospital = recoveredOrcHospital.GetComponent<Hospital>();
                        hospital.removeSickOrc(recoveredOrc);
                    }
                    //Debug.Log("recovered");
                    recoveredOrc.informWorkLocationHealthy();
                    //Updating sickInhabWorkLocations to remove this orc so that the house does not believe the location is unavailable
                    //TODO: does this cause issues between houses?
                    if (recoveredOrc.getWorkLocation() != null && sickInhabWorkLocations[recoveredOrc.getWorkLocation()] > 1)
                    {
                        sickInhabWorkLocations[recoveredOrc.getWorkLocation()]--;
                    }
                    else if (recoveredOrc.getWorkLocation() != null)
                    {
                        sickInhabWorkLocations.Remove(recoveredOrc.getWorkLocation());
                    }
                    sickInhabitants.Remove(recoveredOrc);
                }

                //Orcs getting sick - 100 represents 100%. Health percentages add up to 99% because there is always a chance of sickness
                //TODO: orcs that are already sick have make it more likely other orcs get sick... unless they are in a hospital
                //TODO: the chance of sickness should be lower... way too many orcs getting sick... there should also probably be some sort of buffer at the beginning of a game
                int chanceOfSickness = 100 - (baseHealthPercentage + bathHealthPercentage + barberHealthPercentage * (numCoveredByBarbers / numInhabitants));//TODO: these percentages only get used if the buildings are usable
                if (sickInhabitants.Count < numInhabitants && Random.value * 100 <= chanceOfSickness)
                {
                    SickOrc sickOrc = new SickOrc();
                    sickInhabitants.Add(sickOrc);
                    //Inform work location the inhabitant can't work right now
                    List<GameObject> workLocations = new List<GameObject>(inhabWorkLocations.Keys);
                    bool foundWorkLocationOfSickWorker = false;
                    int i = 0;
                    while (!foundWorkLocationOfSickWorker && i < workLocations.Count)
                    {
                        //If there are still healthy workers from this household at the work location
                        if (!sickInhabWorkLocations.ContainsKey(workLocations[i]) || sickInhabWorkLocations[workLocations[i]] < inhabWorkLocations[workLocations[i]])//<=?
                        {
                            //sickOrc works at this work location
                            Employment workLocationEmployment = workLocations[i].GetComponent<Employment>();
                            workLocationEmployment.updateSickWorkers(1);
                            if (sickInhabWorkLocations.ContainsKey(workLocations[i]))
                            {
                                sickInhabWorkLocations[workLocations[i]] += 1;
                            }
                            else
                            {
                                sickInhabWorkLocations.Add(workLocations[i], 1);
                            }
                            sickOrc.setWorkLocation(workLocations[i]);
                            foundWorkLocationOfSickWorker = true;
                        }
                        i++;
                    }
                }
            }
        }

        //find places for unemployed inhabitants to work. if an orc is sick, it cannot find a job
        if (numEmployedInhabitants < numInhabitants - sickInhabitants.Count)
        {
            Vector2 housePosition = gameObject.transform.position;
            GameObject world = GameObject.Find(World.WORLD_INFORMATION);
            World myWorld = world.GetComponent<World>();
            GameObject[,] constructArr = myWorld.constructNetwork.getConstructArr();
            int i = 0;
            while (i <= jobSearchRadius * 2 && numEmployedInhabitants < numInhabitants)
            {
                int j = 0;
                while (j <= jobSearchRadius * 2 && numEmployedInhabitants < numInhabitants)
                {
                    if (housePosition.x - jobSearchRadius + i >= 0 && housePosition.y - jobSearchRadius + j >= 0
                        && housePosition.x - jobSearchRadius + i <= myWorld.mapSize - 1 && housePosition.y - jobSearchRadius + j <= myWorld.mapSize - 1
                        && constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j] != null
                        && constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j].tag == World.BUILDING)
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

        //Upkeep
        if (myWorld.getPaymentTime() > 0.0f && Mathf.Abs(myWorld.getPaymentTime() - nextPaymentTime) > 0.1f)
        {
            nextPaymentTime = myWorld.getPaymentTime();
            myWorld.updateCurrency(-upkeep);
        }
    }

    /**
     * Click the object to see information about it
     */
    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameObject.FindWithTag(World.BUILD_OBJECT) == null)
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

    /**
     * Gets the currency currently held by the household
     * @return the currency the household currently has
     */
    public int getHouseholdCurrency()
    {
        return householdCurrency;
    }

    /**
     * Adds a Mud Bath to the possible Mud Baths this household can visit
     * @param mudBath a nearby mud bath this household can visit
     */
    public void addMudBath(GameObject mudBath)
    {
        if (!nearbyBaths.Contains(mudBath))
        {
            nearbyBaths.Add(mudBath);
        }
    }

    /**
     * Removes a Mud Bath from the possible Mud Baths this household can visit
     * @param mudBath a mud bath this household can no longer visit
     */
    public void removeMudBath(GameObject mudBath)
    {
        if (nearbyBaths.Contains(mudBath))
        {
            nearbyBaths.Remove(mudBath);
        }
    }

    /**
     * Adds a Barber to the possible Barbers this household can visit
     * @param barber a nearby barber this household can visit
     */
    public void addBarber(GameObject barber)
    {
        if (!nearbyBarbers.Contains(barber))
        {
            nearbyBarbers.Add(barber);
        }
    }

    /**
     * Removes a Barber from the possible Barbers this household can visit
     * @param barber a barber this household can no longer visit
     */
    public void removeBarber(GameObject barber)
    {
        if (nearbyBarbers.Contains(barber))
        {
            nearbyBarbers.Remove(barber);
        }
    }

    /**
     * Adds a Hospital to the possible Hospitals this household can visit
     * @param hospital a nearby hospital this household can visit
     */
    public void addHospital(GameObject hospital)
    {
        if (!nearbyHospitals.Contains(hospital))
        {
            nearbyHospitals.Add(hospital);
        }
    }

    /**
     * Removes a Hospital from the possible Hospitals this household can visit
     * @param hospital a hospital this household can no longer visit
     */
    public void removeHospital(GameObject hospital)
    {
        if (nearbyHospitals.Contains(hospital))
        {
            nearbyHospitals.Remove(hospital);
        }
    }

    /**
     * Updates the currency the houshold has
     * @param currencyChange how much the household's currency should change by
     */
    public void updateHouseholdCurrency(string reason, int currencyChange)
    {
        if (reason.Equals(World.TAX))
        {
            goldSinceLastTax = 0;
        }
        else if (reason.Equals(World.JOB_PAYMENT))
        {
            goldSinceLastTax += currencyChange;
        }
        householdCurrency += currencyChange;
    }

    /**
     * Returns the amount of gold gained from jobs since the last time the house was taxed
     */
    public int getGoldSinceLastTax()
    {
        return goldSinceLastTax;
    }

    /**
     * Returns the sprite currently being used by the house
     */
    public Sprite getHouseSprite()
    {
        return gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    /**
     * Gets the current level of the house
     */
    public int getHouseLevel()
    {
        return houseLevel;
    }

    /**
     * Gets the number of sick inhabitants dwelling in this house
     */
    public int getNumSickInhabitants()
    {
        return sickInhabitants.Count;
    }
}
