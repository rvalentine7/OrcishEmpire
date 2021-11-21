using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides bathing services for the health of orcs
/// TODO: there should probably be a water-type employment class that this and fountain extend from (methods dealing with filling)
/// </summary>
public class MudBath : Building
{
    public Sprite dryBath;
    public Sprite wetBath;
    public int bathRadius;
    public int timeToKeepWet;
    
    private World myWorld;
    private GameObject[,] terrainArr;
    private Employment employment;
    private bool hasWaterAccess;
    private Condition currentCondition;
    private enum Condition {WET, DRY};
    private bool hadPriorWorkers;
    private float timeOfLastRefill;
    private float timeLeftToRefill;
    private float timeUntilDry;
    private float amountLeftToRefill;
    private float previousCheckTime;

    /// <summary>
    /// Initializes objects that must be initialized before anything else
    /// </summary>
    private void Awake()
    {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
    }

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        currentCondition = Condition.DRY;
        hadPriorWorkers = false;
        amountLeftToRefill = timeToKeepWet;
        previousCheckTime = 0f;
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        employment = gameObject.GetComponent<Employment>();
        if (terrainArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y].GetComponent<Tile>().hasPipes())
        {
            hasWaterAccess = true;
        }
        else
        {
            hasWaterAccess = false;
        }
    }

    /// <summary>
    /// Updates information managed by the mud bath
    /// </summary>
    void Update()
    {
        //If there are no workers, the bath should dry up
        if (employment.getNumHealthyWorkers() == 0 || !hasWaterAccess)
        {
            //If no workers or no workers able to refill (lack of water), time left to refill should increase to timeToKeepWet's value
            if (hadPriorWorkers)
            {
                hadPriorWorkers = false;
                handleServices(false);//doing this here because no worker to provide services even if the bath was still wet
            }
            timeLeftToRefill = Mathf.Infinity;
            //Everything should be drying up
            if (currentCondition != Condition.DRY)
            {
                timeUntilDry = timeToKeepWet - (Time.time - timeOfLastRefill);
                if (timeUntilDry <= 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = dryBath;
                    timeUntilDry = 0;
                    currentCondition = Condition.DRY;
                }
            }
        }
        //If it is not already wet or the number of workers is not at its max, we need to keep track of refilling
        else if (hasWaterAccess && (currentCondition == Condition.DRY || employment.getNumHealthyWorkers() != employment.getWorkerCap()))
        {
            if (!hadPriorWorkers)
            {
                timeOfLastRefill = Time.time;
            }
            hadPriorWorkers = true;//there are workers here now
            //If there were previously no workers or water, start with the time required to refill
            if (timeLeftToRefill == Mathf.Infinity)
            {
                amountLeftToRefill = timeToKeepWet;
            }
            else
            {
                amountLeftToRefill -= (Time.time - previousCheckTime) * ((employment.getNumHealthyWorkers() * 1.0f) / employment.getWorkerCap());
            }
            previousCheckTime = Time.time;
            timeLeftToRefill = amountLeftToRefill / ((employment.getNumHealthyWorkers() * 1.0f) / employment.getWorkerCap());
            //Update progress on refilling
            if (timeLeftToRefill < 0)
            {
                timeLeftToRefill = 0;
            }
            if (timeLeftToRefill == 0 && currentCondition == Condition.DRY)
            {
                timeOfLastRefill = Time.time;
                currentCondition = Condition.WET;
                amountLeftToRefill = timeToKeepWet;
                timeLeftToRefill = amountLeftToRefill;
                timeUntilDry = timeToKeepWet;
                gameObject.GetComponent<SpriteRenderer>().sprite = wetBath;
                handleServices(true);
            }
            //If there are not a max number of workers or it just got back to a max number of workers, it should be drying up
            if (timeUntilDry != 0 && (employment.getNumHealthyWorkers() != employment.getWorkerCap() || timeLeftToRefill > timeUntilDry))
            {
                timeUntilDry = timeToKeepWet - (Time.time - timeOfLastRefill);
                if (timeUntilDry <= 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = dryBath;
                    timeUntilDry = 0;
                    currentCondition = Condition.DRY;
                    handleServices(false);
                }
            }
        }
    }

    /// <summary>
    /// Updates whether the mud bath has water access
    /// </summary>
    /// <param name="hasWaterAccess">whether the mud bath has water access</param>
    public void updateFilled(bool hasWaterAccess)
    {
        this.hasWaterAccess = hasWaterAccess;
    }

    /// <summary>
    /// Lets households within the bath radius know whether this bath is providing services to them
    /// </summary>
    /// <param name="providing">whether the bath is providing its services to nearby houses</param>
    public void handleServices(bool providing)
    {
        GameObject[,] constructArr = myWorld.constructNetwork.getConstructArr();
        Vector2 mudBathPosition = gameObject.transform.position;
        //search for nearby houses
        for (int i = 0; i <= bathRadius * 2; i++)
        {
            for (int j = 0; j <= bathRadius * 2; j++)
            {
                if (mudBathPosition.x - bathRadius + i >= 0 && mudBathPosition.y - bathRadius + j >= 0
                        && mudBathPosition.x - bathRadius + i <= myWorld.mapSize && mudBathPosition.y - bathRadius + j <= myWorld.mapSize
                        && constructArr[(int)mudBathPosition.x - bathRadius + i, (int)mudBathPosition.y - bathRadius + j] != null
                        && constructArr[(int)mudBathPosition.x - bathRadius + i, (int)mudBathPosition.y - bathRadius + j].tag == World.HOUSE)
                {
                    HouseInformation houseInformation = constructArr[(int)mudBathPosition.x - bathRadius + i,
                        (int)mudBathPosition.y - bathRadius + j].GetComponent<HouseInformation>();
                    if (providing)
                    {
                        houseInformation.addMudBath(gameObject);
                    }
                    else
                    {
                        houseInformation.removeMudBath(gameObject);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the time left to refill the mud bath to keep it wet
    /// </summary>
    /// <returns>the time left to refill the mud bath</returns>
    public int getTimeLeftToRefill()
    {
        return Mathf.RoundToInt(timeLeftToRefill);
    }

    /// <summary>
    /// Gets the time until this mud bath will dry up
    /// </summary>
    /// <returns>the time until the mad bath dries up</returns>
    public int getTimeUntilDry()
    {
        return Mathf.RoundToInt(timeUntilDry);
    }

    /// <summary>
    /// Gets whether the mud bath is currently wet
    /// </summary>
    /// <returns>whether the mud bath is currently wet</returns>
    public bool getWet()
    {
        return currentCondition == Condition.WET;
    }
}
