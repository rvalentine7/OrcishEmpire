using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides entertainment to nearby houses
/// </summary>
public class Arena : Building {
    private bool active;
    private World myWorld;
    private GameObject[,] structureArr;
    private Employment employment;
    private bool ongoingFight;
    private bool findingGladiators;
    private int numGladiators;
    private int numIncomingGladiators;
    private float fightEndTime;
    private float setupProgress;
    private float prevUpdateTime;
    /*The places from which the gladiators are coming.
     If the building no longer needs them and numIncomingGladiators > 0,
     will need to have them use this dictionary to return to their origin.*/
    private Dictionary<GameObject, int> gladiatorPitsAndNumbers;
    
    public int fightDuration;
    public float timeInterval;
    public float timeToSetup;
    public int numGladiatorsRequired;
    public int gladiatorSearchRadius;
    public int housingSearchRadius;


	/// <summary>
    /// Initializes the arena
    /// </summary>
	void Start () {
        active = true;
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();
        employment = gameObject.GetComponent<Employment>();
        ongoingFight = false;
        numGladiators = 0;
        numIncomingGladiators = 0;
        fightEndTime = 0.0f;
        setupProgress = 0.0f;
        prevUpdateTime = 0.0f;
        gladiatorPitsAndNumbers = new Dictionary<GameObject, int>();
	}
	
	/// <summary>
    /// Logic for the arena
    /// </summary>
	void Update () {
        if (active)
        {
            int numHealthyWorkers = employment.getNumHealthyWorkers();
            if (numHealthyWorkers == 0)
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
                if (setupProgress < 100 && !ongoingFight)
                {
                    float progressedTime = Time.unscaledTime - prevUpdateTime;
                    float effectiveTimeToFinish = timeToSetup / (employment.getNumWorkers() / numHealthyWorkers);
                    setupProgress += progressedTime / effectiveTimeToFinish * 100;
                    if (setupProgress >= 100)
                    {
                        setupProgress = 100;
                    }
                }

                //If we have all the required gladiators, the arena is set up, and a fight hasn't started, start the fight
                if (numGladiators == numGladiatorsRequired && !ongoingFight && setupProgress >= 100)
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
                                Mathf.RoundToInt(gameObject.transform.position.y) - gladiatorSearchRadius + j].tag.Equals(World.BUILDING)
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
                                Mathf.RoundToInt(gameObject.transform.position.y) - housingSearchRadius + j].tag.Equals(World.HOUSE))
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

        prevUpdateTime = Time.unscaledTime;
    }

    /// <summary>
    /// Toggles whether the building is active
    /// </summary>
    public void toggleActive()
    {
        active = !active;
    }

    /// <summary>
    /// Used by gladiators when they arrive at this building
    /// </summary>
    /// <param name="location">the location from which the gladiator arrived</param>
    /// <param name="num">how many gladiators are a part of this gladiator gameObject which arrived</param>
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

    /// <summary>
    /// Removes a number of gladiators that were coming to the arena
    /// </summary>
    /// <param name="num">the number of incoming gladiators to remove</param>
    public void removeIncomingGladiators(int num)
    {
        numIncomingGladiators -= num;
    }

    /// <summary>
    /// Gets the number of gladiators at the arena
    /// </summary>
    /// <returns>The number of gladiators at the arena</returns>
    public int getNumGladiators()
    {
        return numGladiators;
    }

    /// <summary>
    /// Gets the number of gladiators required for a fight to begin
    /// </summary>
    /// <returns>The number of gladiators required for a fight to begin</returns>
    public int getNumGladiatorsRequired()
    {
        return numGladiatorsRequired;
    }

    /// <summary>
    /// Gets the current progress in setting up the arena for a fight
    /// </summary>
    /// <returns>The current progress in setting up the arena for a fight</returns>
    public int getSetupProgress()
    {
        return Mathf.FloorToInt(setupProgress);
    }

    /// <summary>
    /// Gets the current time left in the fight
    /// </summary>
    /// <returns>The current time left in the fight</returns>
    public float getRemainingFightTime()
    {
        return fightEndTime - Time.time;
    }
}
