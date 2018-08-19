using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {
    private bool active;
    private GameObject world;
    private World myWorld;
    private GameObject[,] structureArr;
    private bool ongoingFight;
    private bool findingGladiators;
    private int numGladiators;
    private int numIncomingGladiators;
    private float fightEndTime;
    private int setupProgress;
    private float checkTime;
    /*The places from which the gladiators are coming.
     If the building no longer needs them and numIncomingGladiators > 0,
     will need to have them use this dictionary to return to their origin.*/
    private Dictionary<GameObject, int> gladiatorPitsAndNumbers;
    
    public int fightDuration;
    public float timeInterval;
    public int numGladiatorsRequired;
    public int gladiatorSearchRadius;
    public int housingSearchRadius;


	// Use this for initialization
	void Start () {
        active = true;
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        ongoingFight = false;
        numGladiators = 0;
        numIncomingGladiators = 0;
        fightEndTime = 0.0f;
        setupProgress = 0;
        checkTime = 0.0f;
        gladiatorPitsAndNumbers = new Dictionary<GameObject, int>();
	}
	
	// Update is called once per frame
	void Update () {
        if (active)
        {
            Employment employment = gameObject.GetComponent<Employment>();
            if (employment.getNumWorkers() == 0)
            {
                if (ongoingFight)
                {
                    ongoingFight = false;
                }
                setupProgress = 0;
                findingGladiators = false;
            }
            else
            {
                //Make progress towards setting up a fight
                if (setupProgress < 100 && !ongoingFight && Time.time > checkTime)
                {
                    setupProgress += employment.getNumWorkers() * employment.getWorkerValue();
                    if (setupProgress > 100)
                    {
                        setupProgress = 100;
                    }
                    checkTime = Time.time + timeInterval;
                }

                //If we have all the required gladiators, the arena is set up, and a fight hasn't started, start the fight
                if (numGladiators == numGladiatorsRequired && !ongoingFight && setupProgress == 100)
                {
                    ongoingFight = true;
                    fightEndTime = Time.time + fightDuration;
                }

                //how often fights can happen is based off of a progress variable which is impacted by number of employees
                if (!ongoingFight && !findingGladiators)
                {
                    findingGladiators = true;
                }

                //need to get gladiators from a gladiator pit
                //call requestGladiators(int numRequested, GameObject destination)
                if (findingGladiators && numGladiators + numIncomingGladiators < numGladiatorsRequired)
                {
                    //keep a list of places the gladiators are coming from
                    //search for places to get gladiators from
                    int i = 0;
                    while (i < gladiatorSearchRadius * 2 && (findingGladiators && numGladiators + numIncomingGladiators < numGladiatorsRequired))
                    {
                        int j = 0;
                        while (j < gladiatorSearchRadius * 2 && (findingGladiators && numGladiators + numIncomingGladiators < numGladiatorsRequired))
                        {
                            if (Mathf.RoundToInt(gameObject.transform.position.x) - gladiatorSearchRadius + i >= 0
                                && Mathf.RoundToInt(gameObject.transform.position.y) - gladiatorSearchRadius + j >= 0
                                && Mathf.RoundToInt(gameObject.transform.position.x) - gladiatorSearchRadius + i <= 39
                                && Mathf.RoundToInt(gameObject.transform.position.y) - gladiatorSearchRadius + j <= 39
                                && structureArr[Mathf.RoundToInt(gameObject.transform.position.x) - gladiatorSearchRadius + i,
                                Mathf.RoundToInt(gameObject.transform.position.y) - gladiatorSearchRadius + j] != null
                                && structureArr[Mathf.RoundToInt(gameObject.transform.position.x) - gladiatorSearchRadius + i,
                                Mathf.RoundToInt(gameObject.transform.position.y) - gladiatorSearchRadius + j].tag == "Building"
                                && structureArr[Mathf.RoundToInt(gameObject.transform.position.x) - gladiatorSearchRadius + i,
                                Mathf.RoundToInt(gameObject.transform.position.y) - gladiatorSearchRadius + j].GetComponent<GladiatorPit>() != null)
                            {
                                //if it is a gladiator pit, request gladiators
                                GameObject gladiatorPit = structureArr[Mathf.RoundToInt(gameObject.transform.position.x) - gladiatorSearchRadius + i,
                                    Mathf.RoundToInt(gameObject.transform.position.y) - gladiatorSearchRadius + j];
                                numIncomingGladiators += gladiatorPit.GetComponent<GladiatorPit>().requestGladiators(numGladiatorsRequired - (numGladiators + numIncomingGladiators), gameObject);
                            }
                            j++;
                        }
                        i++;
                    }
                }

                //End the fighting if it surpasses the time limit
                if (ongoingFight && Time.time > fightEndTime)
                {
                    numGladiators = 0;
                    ongoingFight = false;
                    setupProgress = 0;
                }
                //whenever a fight happens, give entertainment to nearby houses
                //this is a tier 1 entertainment building so it can only increase entertainment in nearby houses to tier 1.  plan is to have 2 tiers
                if (ongoingFight)
                {
                    for (int i = 0; i < housingSearchRadius * 2; i++)
                    {
                        for (int j = 0; j < housingSearchRadius * 2; j++)
                        {
                            if (Mathf.RoundToInt(gameObject.transform.position.x) - housingSearchRadius + i >= 0
                                && Mathf.RoundToInt(gameObject.transform.position.y) - housingSearchRadius + j >= 0
                                && Mathf.RoundToInt(gameObject.transform.position.x) - housingSearchRadius + i <= 39
                                && Mathf.RoundToInt(gameObject.transform.position.y) - housingSearchRadius + j <= 39
                                && structureArr[Mathf.RoundToInt(gameObject.transform.position.x) - housingSearchRadius + i,
                                Mathf.RoundToInt(gameObject.transform.position.y) - housingSearchRadius + j] != null
                                && structureArr[Mathf.RoundToInt(gameObject.transform.position.x) - housingSearchRadius + i,
                                Mathf.RoundToInt(gameObject.transform.position.y) - housingSearchRadius + j].tag == "House")
                            {
                                GameObject house = structureArr[Mathf.RoundToInt(gameObject.transform.position.x) - housingSearchRadius + i,
                                    Mathf.RoundToInt(gameObject.transform.position.y) - housingSearchRadius + j];
                                house.GetComponent<HouseInformation>().setEntertainmentLevel(1);
                            }
                        }
                    }
                }
                
                
            }
        }
	}

    /**
     * Toggles whether the building is active
     */
    public void toggleActive()
    {
        active = !active;
    }

    /**
     * Used by gladiators when they arrive at this building
     * @param location the location from which the gladiator arrived
     * @param num how many gladiators are a part of this gladiator gameObject which arrived
     */
    public void gladiatorArrived(GameObject location, int num)
    {
        //TODO: don't think I need the dictionary gladiatorPitsAndNumbers anymore... having gladiators check if this location still has employees instead
        //  I do need to update numIncomingGladiators, though
        if (gladiatorPitsAndNumbers.ContainsKey(location))
        {
            gladiatorPitsAndNumbers[location] += num;
        }
        else
        {
            gladiatorPitsAndNumbers.Add(location, num);
        }
        numGladiators += num;
        numIncomingGladiators -= num;
        if (numGladiators == numGladiatorsRequired)
        {
            findingGladiators = false;
        }
    }

    /**
     * Removes a number of gladiators that were coming to the arena
     * @param num the number of incoming gladiators to remove
     */
    public void removeIncomingGladiators(int num)
    {
        numIncomingGladiators -= num;
    }

    /**
     * Gets the number of gladiators at the arena
     */
    public int getNumGladiators()
    {
        return numGladiators;
    }

    /**
     * Gets the number of gladiators required for a fight to begin
     */
    public int getNumGladiatorsRequired()
    {
        return numGladiatorsRequired;
    }

    /**
     * Gets the current progress in setting up the arena for a fight
     */
    public float getSetupProgress()
    {
        return setupProgress;
    }

    /**
     * Gets the current time left in the fight
     */
    public float getRemainingFightTime()
    {
        return fightEndTime - Time.time;
    }
}
