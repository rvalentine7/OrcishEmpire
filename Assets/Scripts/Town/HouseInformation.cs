using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Contains information about the house this is attached to such as
 *  which orcs live in it and what kind of supplies the house has.
 *  
 *  TODO: OrcInhabitants should be handling more of the starting/quitting jobs instead of HouseInformation
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
    
    private List<OrcInhabitant> orcInhabitants;
    private List<OrcInhabitant> unemployedInhabitants;
    private int numIncomingOrcs;
    private Dictionary<GameObject, List<OrcInhabitant>> inhabWorkLocations;
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
    private List<GameObject> nearbyBaths;//Reduces likelihood of getting sick
    private List<GameObject> nearbyHospitals;//Hospitals treat orcs who are sick
    private int baseHealthPercentage;
    private int bathHealthPercentage;
    private int barberHealthPercentage;
    private int numSickInhabitantsAtHospital;
    private int minSickRecoveryWait;
    private int maxSickRecoveryWait;
    private int sickRecoveryChance;
    private List<OrcInhabitant> sickInhabitants;
    private List<OrcInhabitant> inhabitantsWithoutBarbers;
    private Vector2 housePosition;
    private GameObject[,] constructArr;
    private SpriteRenderer spriteRender;

    //add future resources here (weapons/furniture/currency)
    private World myWorld;

    private void Awake()
    {
        sickInhabitants = new List<OrcInhabitant>();
    }

    /**
     * Initializes the variables involving the house.
     */
    void Start() {
        orcInhabitants = new List<OrcInhabitant>();
        unemployedInhabitants = new List<OrcInhabitant>();
        inhabitantsWithoutBarbers = new List<OrcInhabitant>();
        numIncomingOrcs = 0;
        inhabWorkLocations = new Dictionary<GameObject, List<OrcInhabitant>>();//keeps a list of the gameObjects where inhabitants
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
        
        //Health buildings can only service so many people. Number of employees at health locations determines how many people can be serviced
        nearbyBarbers = new List<GameObject>();
        nearbyBaths = new List<GameObject>();
        nearbyHospitals = new List<GameObject>();
        baseHealthPercentage = 89;
        bathHealthPercentage = 5;
        barberHealthPercentage = 5;
        numSickInhabitantsAtHospital = 0;
        minSickRecoveryWait = 2;
        maxSickRecoveryWait = 5;
        sickRecoveryChance = 33;
        
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();
        constructArr = myWorld.constructNetwork.getConstructArr();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();

        nextPaymentTime = myWorld.getPaymentTime();
        //Check if tile already has water, update numWaterSources if so
        if (terrainArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<Tile>().hasWater())
        {
            addWaterSource();
        }

        //Seeing if any barbers/baths/hospitals are already around
        housePosition = gameObject.transform.position;
        int healthBuildingRadius = World.HEALTH_BUILDING_RADIUS;
        for (int i = -healthBuildingRadius; i < healthBuildingRadius; i++)
        {
            for (int j = -healthBuildingRadius; j < healthBuildingRadius; j++)
            {
                if (housePosition.x + i >= 0 && housePosition.y + j >= 0
                        && housePosition.x + i <= myWorld.mapSize - 1 && housePosition.y + j <= myWorld.mapSize - 1
                        && constructArr[(int)housePosition.x + i, (int)housePosition.y + j] != null
                        && constructArr[(int)housePosition.x + i, (int)housePosition.y + j].tag == World.BUILDING)
                {
                    if (constructArr[(int)housePosition.x + i, (int)housePosition.y + j].GetComponent<Barber>() != null)
                    {
                        /*
                         * Adding barbers to help reduce time complexity during updates.
                         * What actually matters from barbers is number of residents who are covered by the barber
                         */
                        nearbyBarbers.Add(constructArr[(int)housePosition.x + i, (int)housePosition.y + j]);
                    }
                    else if (constructArr[(int)housePosition.x + i, (int)housePosition.y + j].GetComponent<MudBath>() != null)
                    {
                        MudBath mudBath = constructArr[(int)housePosition.x + i, (int)housePosition.y + j].GetComponent<MudBath>();
                        if (mudBath.getWet())
                        {
                            nearbyBaths.Add(constructArr[(int)housePosition.x + i, (int)housePosition.y + j]);
                        }
                    }
                    else if (constructArr[(int)housePosition.x + i, (int)housePosition.y + j].GetComponent<Hospital>() != null)
                    {
                        nearbyHospitals.Add(constructArr[(int)housePosition.x + i, (int)housePosition.y + j]);
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
        //Reduces the entertainment level if it hasn't been given entertainment in the timeBeforeEntertainmentDecay time window
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

            updateStorage(storage);

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
                    if (/*numInhabitants*/orcInhabitants.Count > houseSize)
                    {
                        removeInhabitants(/*numInhabitants*/orcInhabitants.Count - houseSize);
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

            addBarbersForInhabitants();
            updateHealth();
        }

        findJobs();

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
     * Updates the houses' supply of goods
     * @param storage the storage of the house
     */
    private void updateStorage(Storage storage)
    {
        //TODO: update food information
        if (storage.getFoodCount() > 0 && storage.getFoodCount() >= orcInhabitants.Count * inhabitantFoodConsumption)
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
            if (storage.getFishCount() > 0)
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
            storage.removeResource(World.MEAT, orcInhabitants.Count * inhabitantFoodConsumption * (storage.getMeatCount() / storage.getFoodCount()));
            if (storage.getFoodCount() > 0)
            {
                storage.removeResource(World.WHEAT, orcInhabitants.Count * inhabitantFoodConsumption * (storage.getWheatCount() / storage.getFoodCount()));
            }
            if (storage.getFoodCount() > 0)
            {
                storage.removeResource(World.EGGS, orcInhabitants.Count * inhabitantFoodConsumption * (storage.getEggCount() / storage.getFoodCount()));
            }
            if (storage.getFoodCount() > 0)
            {
                storage.removeResource(World.FISH, orcInhabitants.Count * inhabitantFoodConsumption * (storage.getFishCount() / storage.getFoodCount()));
            }
        }
        else if (storage.getFoodCount() > 0)
        {
            storage.removeResource(World.MEAT, storage.getMeatCount());
            storage.removeResource(World.WHEAT, storage.getWheatCount());
            storage.removeResource(World.EGGS, storage.getEggCount());
            storage.removeResource(World.FISH, storage.getFishCount());
        }
    }

    /**
     * Updates the barber locations for the orc inhabitants
     */
    private void addBarbersForInhabitants()
    {
        int index = 0;
        while (inhabitantsWithoutBarbers.Count > 0 && index < nearbyBarbers.Count)
        {
            Barber barber = nearbyBarbers[index].GetComponent<Barber>();
            int numAvailableSpots = barber.getNumAvailableCustomerSpots();
            if (numAvailableSpots > 0)
            {
                int numGettingNewBarber = numAvailableSpots - inhabitantsWithoutBarbers.Count >= 0 ? inhabitantsWithoutBarbers.Count : numAvailableSpots;
                for (int i = 0; i < numGettingNewBarber; i++)
                {
                    inhabitantsWithoutBarbers[0].setBarber(barber);
                    inhabitantsWithoutBarbers.RemoveAt(0);
                }
            }

            index++;
        }
    }

    /**
     * Lets the house know an orc is not covered by a barber
     * @param orcInhabitant the orc that is not covered by a barber
     */
    public void removeOrcCoveredByBarber(OrcInhabitant orcInhabitant)
    {
        if (!inhabitantsWithoutBarbers.Contains(orcInhabitant))
        {
            inhabitantsWithoutBarbers.Add(orcInhabitant);
        }
    }

    /**
     * Gets the number of inhabitants who are covered by barbers
     * 
     * @return the number of inhabitants who are covered by barbers
     */
    public int getNumCoveredByBarbers()
    {
        return this.orcInhabitants.Count - inhabitantsWithoutBarbers.Count;
    }

    /**
     * Updates orc inhabitants getting sick, recovering, and visiting hospitals
     */
    private void updateHealth()
    {
        //Health (TODO: might need to slow this down later on by setting it to a different timer and giving a new house/new game some time before sickness becomes a factor)
        if (orcInhabitants.Count > 0)
        {
            //If an orc is sick, it has a chance to improve on its own
            List<int> recoveredOrcIndices = new List<int>();
            List<OrcInhabitant> sickOrcsNeedingHospitals = new List<OrcInhabitant>();
            for (int i = 0; i < sickInhabitants.Count; i++)
            {
                //Debug.Log("increasing wait time");
                sickInhabitants[i].increaseSickTime();
                //Check if the sick inhabitant has recovered
                if (sickInhabitants[i].getSickTime() > minSickRecoveryWait && (Random.value * 100 < sickRecoveryChance || sickInhabitants[i].getSickTime() > maxSickRecoveryWait))
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
            int index = 0;
            while (sickOrcsNeedingHospitals.Count > 0 && index < nearbyHospitals.Count)
            {
                Hospital hospital = nearbyHospitals[index].GetComponent<Hospital>();
                if (hospital.addSickOrc(sickOrcsNeedingHospitals[0]))
                {
                    sickOrcsNeedingHospitals.RemoveAt(0);
                }
                index++;
            }
            //Update orcs that have recovered from sickness
            recoveredOrcIndices.Sort();
            for (int i = 0; i < recoveredOrcIndices.Count; i++)
            {
                //Get the recovered orc index, but as it removes from sickInhabitants, the index will also lower
                OrcInhabitant recoveredOrc = sickInhabitants[recoveredOrcIndices[i] - i];
                GameObject recoveredOrcHospital = recoveredOrc.getHospital();
                if (recoveredOrcHospital != null)
                {
                    Hospital hospital = recoveredOrcHospital.GetComponent<Hospital>();
                    hospital.removeSickOrc(recoveredOrc);
                }
                recoveredOrc.setHealthy(true);
                sickInhabitants.Remove(recoveredOrc);
            }

            //Orcs getting sick - 100 represents 100%. Health percentages do not get to 100% because there is always a chance of sickness
            //Sick orcs that are not at a hospital have a greater chance of getting other orcs sick
            int chanceOfSickness = 100 - (baseHealthPercentage - (sickInhabitants.Count - numSickInhabitantsAtHospital * 5)
                + (nearbyBaths.Count > 0 ? bathHealthPercentage : 0)
                + barberHealthPercentage * ((orcInhabitants.Count - inhabitantsWithoutBarbers.Count) / orcInhabitants.Count));//(orcInhabitants.Count - inhabitantsWithoutBarbers.Count) is the number that have barbers
            if (sickInhabitants.Count < orcInhabitants.Count && Random.value * 100 <= chanceOfSickness)
            {
                //Finding one of the remaining healthy orcs and updating it to be sick
                OrcInhabitant sickOrc = null;
                bool foundSickInhabitant = false;
                int inhabIndex = 0;
                while (!foundSickInhabitant && inhabIndex < orcInhabitants.Count)
                {
                    if (orcInhabitants[inhabIndex].getHealthy())
                    {
                        sickOrc = orcInhabitants[inhabIndex];
                        sickInhabitants.Add(sickOrc);
                        sickOrc.setHealthy(false);
                        foundSickInhabitant = true;
                    }
                    inhabIndex++;
                }
            }
        }
    }

    /**
     * Finds jobs for unemployed inhabitants
     */
    private void findJobs()
    {
        //find places for unemployed inhabitants to work. if an orc is sick, it cannot find a job
        if (numEmployedInhabitants < orcInhabitants.Count - sickInhabitants.Count)
        {
            Vector2 housePosition = gameObject.transform.position;
            GameObject[,] constructArr = myWorld.constructNetwork.getConstructArr();
            int i = 0;
            while (i <= jobSearchRadius * 2 && numEmployedInhabitants < orcInhabitants.Count)
            {
                int j = 0;
                while (j <= jobSearchRadius * 2 && numEmployedInhabitants < orcInhabitants.Count)
                {
                    if (housePosition.x - jobSearchRadius + i >= 0 && housePosition.y - jobSearchRadius + j >= 0
                        && housePosition.x - jobSearchRadius + i <= myWorld.mapSize - 1 && housePosition.y - jobSearchRadius + j <= myWorld.mapSize - 1
                        && constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j] != null
                        && constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j].tag.Equals(World.BUILDING))
                    {
                        GameObject possibleEmployment = constructArr[(int)housePosition.x - jobSearchRadius + i, (int)housePosition.y - jobSearchRadius + j];
                        Employment employment = possibleEmployment.GetComponent<Employment>();
                        int numJobsAtEmployment = employment.numJobsAvailable();
                        if (employment.getOpenForBusiness() && numJobsAtEmployment > 0)
                        {
                            int numUnemployed = unemployedInhabitants.Count;
                            //Case #1: There are less job spots at the employment than the number of unemployed in this house
                            if (numJobsAtEmployment - numUnemployed < 0)
                            {
                                for (int k = 0; k < numJobsAtEmployment; k++)
                                {
                                    OrcInhabitant orcInhabitant = unemployedInhabitants[0];
                                    unemployedInhabitants.Remove(orcInhabitant);
                                    orcInhabitant.setWorkLocation(possibleEmployment);
                                    employment.addWorker(1, gameObject);
                                    if (!orcInhabitant.getHealthy())
                                    {
                                        employment.updateSickWorkers(1);
                                    }
                                    numEmployedInhabitants++;

                                    if (inhabWorkLocations.ContainsKey(possibleEmployment))
                                    {
                                        inhabWorkLocations[possibleEmployment].Add(orcInhabitant);
                                    }
                                    else
                                    {
                                        List<OrcInhabitant> inhabitantsAtWorkLocation = new List<OrcInhabitant>();
                                        inhabitantsAtWorkLocation.Add(orcInhabitant);
                                        inhabWorkLocations.Add(possibleEmployment, inhabitantsAtWorkLocation);
                                    }
                                }
                            }
                            //Case #2:  There are more than enough job spots at the employment to employ all unemployed members of this house
                            else
                            {
                                for (int k = 0; k < unemployedInhabitants.Count; k++)
                                {
                                    OrcInhabitant orcInhabitant = unemployedInhabitants[k];
                                    orcInhabitant.setWorkLocation(possibleEmployment);
                                    employment.addWorker(1, gameObject);
                                    if (!orcInhabitant.getHealthy())
                                    {
                                        employment.updateSickWorkers(1);
                                    }

                                    if (inhabWorkLocations.ContainsKey(possibleEmployment))
                                    {
                                        inhabWorkLocations[possibleEmployment].Add(orcInhabitant);
                                    }
                                    else
                                    {
                                        List<OrcInhabitant> inhabitantsAtWorkLocation = new List<OrcInhabitant>();
                                        inhabitantsAtWorkLocation.Add(orcInhabitant);
                                        inhabWorkLocations.Add(possibleEmployment, inhabitantsAtWorkLocation);
                                    }
                                }
                                numEmployedInhabitants += numUnemployed;
                                unemployedInhabitants = new List<OrcInhabitant>();
                                numUnemployed = 0;
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
     * Updates the sprite for the house based on the house level
     */
    public void updateHouseSprite()
    {
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
        if (orcInhabitants.Count == 0)
        {
            updateHouseSprite();
        }
        for (int i = 0; i < num; i++)
        {
            OrcInhabitant orcInhabitant = new OrcInhabitant(gameObject);
            orcInhabitants.Add(orcInhabitant);
            inhabitantsWithoutBarbers.Add(orcInhabitant);
            unemployedInhabitants.Add(orcInhabitant);
        }
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
     * @param num the number of inhabitants to remove
     */
    public void removeInhabitants(int num)//TODO: remove an OrcInhabitant
    {
        //Updating lists and other game objects relying on the orc inhabitants that are being removed
        for (int i = 0; i < num; i++)
        {
            OrcInhabitant orcInhabitant = orcInhabitants[orcInhabitants.Count - 1 - i];//treating like a stack in case I want to add something for orcs that live in a house longer
            orcInhabitants.Remove(orcInhabitant);
            if (inhabitantsWithoutBarbers.Contains(orcInhabitant))
            {
                inhabitantsWithoutBarbers.Remove(orcInhabitant);
            }
            if (sickInhabitants.Contains(orcInhabitant))
            {
                sickInhabitants.Remove(orcInhabitant);
            }
            orcInhabitant.evictFromHouse();
            //If the inhabitant had a work location, let the location know this orc can no longer work there
            if (orcInhabitant.getWorkLocation() != null)
            {
                Employment employment = orcInhabitant.getWorkLocation().GetComponent<Employment>();
                employment.removeWorkers(1, gameObject);
                inhabWorkLocations[orcInhabitant.getWorkLocation()].Remove(orcInhabitant);
            }
            if (unemployedInhabitants.Contains(orcInhabitant))
            {
                unemployedInhabitants.Remove(orcInhabitant);
            }
        }
        //populationChange = true;
        if (orcInhabitants.Count > 0)
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
        foreach (OrcInhabitant orcInhabitant in orcInhabitants)
        {
            orcInhabitant.evictFromHouse();
        }
        foreach (KeyValuePair<GameObject, List<OrcInhabitant>> entry in inhabWorkLocations)
        {
            Employment employment = entry.Key.GetComponent<Employment>();
            employment.removeWorkers(entry.Value.Count, gameObject);
        }
        if (orcInhabitants.Count > 0)
        {
            Instantiate(orcEmigrant, new Vector2(gameObject.transform.position.x,
                gameObject.transform.position.y + 0.4f), Quaternion.identity);
        }
        this.myWorld.updatePopulation(-orcInhabitants.Count);
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
        return orcInhabitants.Count;
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
        foreach (OrcInhabitant orcInhabitant in inhabWorkLocations[employment])
        {
            orcInhabitant.setWorkLocation(null);
            unemployedInhabitants.Add(orcInhabitant);
        }
        numEmployedInhabitants -= inhabWorkLocations[employment].Count;
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

    /**
     * Updates how many inhabitants are at a hospital
     * @param num the number of inhabitants to update by
     */
    public void updateNumInhabitantsAtHospital(int num)
    {
        numSickInhabitantsAtHospital += num;
        //Just for warning of an issue that might not get caught:
        if (numSickInhabitantsAtHospital < 0 || numSickInhabitantsAtHospital > orcInhabitants.Count)
        {
            Debug.Log("Issue with adding and removing inhabitants to/from hospitals");
        }
    }

    /**
     * Gets the number of inhabitants currently at a hospital
     */
    public int getNumInhabitantsAtHospital()
    {
        return numSickInhabitantsAtHospital;
    }
}
